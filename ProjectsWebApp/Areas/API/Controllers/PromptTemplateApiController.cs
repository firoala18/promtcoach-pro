using Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using ProjectsWebApp.Dto;
using ProjectsWebApp.Models;
using System.Text.Json;
using System.Text.RegularExpressions;
using DbGroup = ProjectsWebApp.Models.Group;

namespace ProjectsWebApp.Areas.Api.Controllers
{
    [Area("Api")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]                         // alle angemeldeten Benutzer
    public class PromptTemplateApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userMgr;
        private readonly ILogger<PromptTemplateApiController> _logger;
        private readonly IUserActivityLogger _activityLogger;

        public PromptTemplateApiController(
            ApplicationDbContext db,
            UserManager<IdentityUser> userMgr,
            ILogger<PromptTemplateApiController> logger,
            IUserActivityLogger activityLogger)
        {
            _db = db;
            _userMgr = userMgr;
            _logger = logger;
            _activityLogger = activityLogger;
        }

        [HttpGet]
        public async Task<IActionResult> AllowedGroups()
        {
            try
            {
                var groups = await GetAllowedGroupsForCurrentUserAsync();
                return Ok(new { groups });
            }
            catch (Exception ex)
            {
                return Ok(new { groups = new List<string>() });
            }
        }

        private async Task<List<string>> GetAllowedGroupsForCurrentUserAsync()
        {
            var uid = _userMgr.GetUserId(User);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("Coach");
            var isDozent = User.IsInRole("Dozent");

            List<string> existing;
            try { existing = _db.Groups.AsEnumerable().Select(g => g?.Name?.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!).Distinct(StringComparer.OrdinalIgnoreCase).ToList(); }
            catch { existing = new List<string>(); }

            if (isAdmin)
            {
                return existing.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToList();
            }

            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var myGroups = _db.UserGroupMemberships
                    .Where(m => m.UserId == uid)
                    .Select(m => m.Group)
                    .AsEnumerable()
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s!.Trim());
                foreach (var g in myGroups) set.Add(g);
            }
            catch { }

            if (isDozent)
            {
                try
                {
                    var owned = _db.DozentGroupOwnerships
                        .Where(o => o.DozentUserId == uid)
                        .Select(o => o.Group)
                        .AsEnumerable()
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s!.Trim());
                    foreach (var g in owned) set.Add(g);
                }
                catch { }
            }

            // Intersect with existing groups to avoid deleted ones
            var allowed = set.Where(s => existing.Contains(s)).OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToList();
            return allowed;
        }

        /*──────────────────────────────────────────────
          0)  Prompt in die Bibliothek übernehmen
        ──────────────────────────────────────────────*/
        [HttpPost]                               // POST /api/PromptTemplateApi/Publish
        public async Task<IActionResult> Publish([FromBody] PromptTemplateDto dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.Akronym) || string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Akronym und Titel sind Pflichtfelder.");

            // 1) gibt es den Prompt bereits?
            var entity = await _db.PromptTemplate
                                  .FirstOrDefaultAsync(p => p.Akronym == dto.Akronym.Trim());

            var currentUserId = _userMgr.GetUserId(User);
            if (entity == null)
            {
                /* ── neu anlegen ───────────────────────────── */
                entity = new PromptTemplate
                {
                    Akronym = dto.Akronym.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    UserId = currentUserId
                };
                _db.PromptTemplate.Add(entity);
            }

            /* ── in beiden Fällen die Felder füllen / aktualisieren ── */
            // Always stamp the current user as owner on publish/update
            entity.UserId = currentUserId;
            entity.Title = dto.Title;
            entity.Beschreibung = dto.Beschreibung;
            entity.Schluesselbegriffe = dto.Schluesselbegriffe;
            entity.Thema = dto.Thema;
            entity.Ziele = dto.Ziele;
            entity.PromptHtml = dto.PromptHtml;
            entity.PromptType = dto.PromptType;
            // Merge existing FilterJson with techniques parsed from the generated PromptHtml
            entity.FilterJson = MergeWithTechniqueTags(dto.FilterJson, dto.PromptHtml);
            entity.Temperatur = dto.Temperatur;
            entity.MaxZeichen = dto.MaxZeichen ?? entity.MaxZeichen;
            entity.UsedModels = dto.UsedModels ?? entity.UsedModels ?? string.Empty;
            entity.GeneratedImagePath = dto.GeneratedImagePath;
            entity.MetaHash = dto.MetaHash;
            await _db.SaveChangesAsync();

            // Update group associations (optional) – use Groups table (stable IDs)
            try
            {
                var existing = _db.PromptTemplateGroups.Where(g => g.PromptTemplateId == entity.Id);
                _db.PromptTemplateGroups.RemoveRange(existing);

                var allowed = await GetAllowedGroupsForCurrentUserAsync();
                var allowedSet = new HashSet<string>(allowed.Select(s => s.Trim()), StringComparer.OrdinalIgnoreCase);
                var selected = (dto.Groups ?? new List<string>())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Where(s => allowedSet.Contains(s))
                    .ToList();

                // Resolve or create Group rows
                var existingGroups = _db.Groups.ToList();
                var index = existingGroups
                    .GroupBy(x => (x.Name ?? string.Empty).Trim())
                    .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
                foreach (var name in selected)
                {
                    if (!index.TryGetValue(name, out var grp))
                    {
                        grp = new DbGroup { Name = name };
                        _db.Groups.Add(grp);
                        await _db.SaveChangesAsync();
                        index[name] = grp;
                    }
                    _db.PromptTemplateGroups.Add(new PromptTemplateGroup
                    {
                        PromptTemplateId = entity.Id,
                        GroupId = grp.Id,
                        Group = name
                    });
                }
                await _db.SaveChangesAsync();
            }
            catch { }

            // Analytics: prompt published to shared library (Bibliothek)
            try
            {
                if (ProjectsWebApp.Utility.FeatureFlags.EnableAnalytics)
                {
                    string? groupName = null;
                    try
                    {
                        groupName = await _db.UserGroupMemberships
                            .Where(m => m.UserId == currentUserId)
                            .OrderByDescending(m => m.CreatedAt)
                            .Select(m => m.Group)
                            .FirstOrDefaultAsync();
                    }
                    catch { }

                    await _activityLogger.LogAsync(
                        currentUserId ?? string.Empty,
                        string.IsNullOrWhiteSpace(groupName) ? null : groupName!.Trim(),
                        "prompt_publish_library",
                        null,
                        new { dto.Akronym, dto.Title, dto.PromptType },
                        CancellationToken.None);
                }
            }
            catch { /* analytics must never break main flow */ }

            return Ok(new { success = true });
        }

        /*──────────────────────────────────────────────
          1)  Existiert das Akronym?
        ──────────────────────────────────────────────*/
        [HttpGet]
        public async Task<IActionResult> Exists(string akronym)
        {
            if (string.IsNullOrWhiteSpace(akronym))
                return BadRequest(new { exists = false, message = "Akronym fehlt." });

            bool exists = await _db.PromptTemplate
                                   .AnyAsync(t => t.Akronym == akronym.Trim());
            return Ok(new { exists });
        }

        /*──────────────────────────────────────────────
          2)  Kompletten Prompt speichern / überschreiben
              (wird vom Admin‑Backend genutzt)
        ──────────────────────────────────────────────*/
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] PromptTemplate dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Akronym) ||
                string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Akronym und Titel sind Pflicht.");

            var userId = _userMgr.GetUserId(User);
            var entry = await _db.PromptTemplate
                                 .FirstOrDefaultAsync(p => p.Akronym == dto.Akronym.Trim());

            if (entry == null)
            {
                dto.CreatedAt = DateTime.UtcNow;
                // Stamp current user as owner
                dto.UserId = userId;
                // ensure FilterJson includes parsed techniques
                dto.FilterJson = MergeWithTechniqueTags(dto.FilterJson, dto.PromptHtml);
                _db.PromptTemplate.Add(dto);
            }
            else
            {
                // Felder übernehmen
                entry.Title = dto.Title;
                entry.Beschreibung = dto.Beschreibung;
                entry.Schluesselbegriffe = dto.Schluesselbegriffe;
                entry.Thema = dto.Thema;
                entry.Ziele = dto.Ziele;
                entry.PromptHtml = dto.PromptHtml;
                entry.PromptType = dto.PromptType;
                entry.FilterJson = MergeWithTechniqueTags(dto.FilterJson, dto.PromptHtml);
                entry.Temperatur = dto.Temperatur;
                entry.MaxZeichen = dto.MaxZeichen;
                entry.MetaHash = dto.MetaHash;
                entry.UsedModels = dto.UsedModels ?? entry.UsedModels ?? string.Empty;
                entry.GeneratedImagePath = dto.GeneratedImagePath;

                // Always stamp current user as owner on save/overwrite
                entry.UserId = userId;

            }



            await _db.SaveChangesAsync();
            return Ok();
        }

        private static string MergeWithTechniqueTags(string? filterJson, string? promptHtml)
        {
            var items = new List<Dictionary<string, string>>();
            var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 1) existing items
            if (!string.IsNullOrWhiteSpace(filterJson))
            {
                try
                {
                    using var doc = JsonDocument.Parse(filterJson);
                    if (doc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var el in doc.RootElement.EnumerateArray())
                        {
                            var val = el.TryGetProperty("value", out var v) ? (v.GetString() ?? string.Empty) : string.Empty;
                            var ins = el.TryGetProperty("instruction", out var i) ? (i.GetString() ?? val) : val;
                            if (string.IsNullOrWhiteSpace(val)) continue;

                            var key = val.Trim();
                            if (!existing.Add(key)) continue; // skip duplicates

                            items.Add(new Dictionary<string, string>
                            {
                                { "value", key },
                                { "instruction", ins.Trim() }
                            });
                        }
                    }
                }
                catch { }
            }

            // 2) extract techniques like (Role Prompting): from promptHtml
            var text = Regex.Replace(promptHtml ?? string.Empty, "<.*?>", string.Empty);
            var matches = Regex.Matches(text, "\\(([^)]+)\\)\\s*:");
            foreach (Match m in matches.Cast<Match>())
            {
                var name = (m.Groups[1].Value ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(name)) continue;
                if (existing.Add(name))
                    items.Add(new Dictionary<string, string> { { "value", name }, { "instruction", name } });
            }

            return JsonSerializer.Serialize(items);
        }

        /*──────────────────────────────────────────────
          3)  Variation anhängen
        ──────────────────────────────────────────────*/
        [HttpPost]
        public async Task<IActionResult> AddVariation([FromBody] AddVariationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Akronym))
                return BadRequest("Akronym fehlt.");

            var basePrompt = await _db.PromptTemplate
                                      .FirstOrDefaultAsync(p => p.Akronym == dto.Akronym.Trim());

            if (basePrompt == null)
                return NotFound($"Kein Prompt mit Akronym “{dto.Akronym}” gefunden.");

            _db.PromptVariations.Add(new PromptVariation
            {
                PromptTemplateId = basePrompt.Id,
                VariationJson = JsonSerializer.Serialize(dto.Data)
            });

            await _db.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}
