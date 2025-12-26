using Dto.PromptFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using ProjectsWebApp.DataAccsess.Services.Calsses;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using ProjectsWebApp.Models;
using ProjectsWebApp.Utility;
using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectsWebApp.DataAccsess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ProjectsWebApp.Areas.User.Controllers
{
    [Area("User")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public sealed class PromptSmartSelectionController : ControllerBase
    {
        private readonly IPromptSmartSelectionService _svc;
        private readonly ISmartSelectionProgressStore _progress;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _um;
        private readonly IUserActivityLogger _activityLogger;

        public PromptSmartSelectionController(IPromptSmartSelectionService svc,
                                              ISmartSelectionProgressStore progress,
                                              ApplicationDbContext db,
                                              UserManager<IdentityUser> um,
                                              IUserActivityLogger activityLogger)
        {
            _svc = svc;
            _progress = progress;
            _db = db;
            _um = um;
            _activityLogger = activityLogger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GenerateFiltersRequestDto req, CancellationToken ct)
        {
            if (req is null || req.Form is null)
                return BadRequest("Ungültige Anfrage.");

            var targetType = Enum.TryParse<PromptType>(req.TargetType, ignoreCase: true, out var parsed)
                ? parsed
                : PromptType.Text;

            // feature toggle: per‑group -> global -> static; Admins override
            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            bool enabledSS = FeatureFlags.EnableSmartSelection; // fallback
            var userId = _um.GetUserId(User)!;
            string? groupName = null;
            try
            {
                var globalRec = await _db.PromptFeatureSettings
                                         .OrderByDescending(x => x.UpdatedAt)
                                         .FirstOrDefaultAsync(ct);
                if (globalRec != null)
                    enabledSS = globalRec.EnableSmartSelection;

                groupName = (req.GroupName ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(groupName))
                {
                    var grp = await _db.UserGroupMemberships
                                       .Where(m => m.UserId == userId)
                                       .OrderByDescending(m => m.CreatedAt)
                                       .Select(m => m.Group)
                                       .FirstOrDefaultAsync(ct);
                    groupName = string.IsNullOrWhiteSpace(grp) ? "Ohne Gruppe" : grp!.Trim();
                }

                if (!string.IsNullOrWhiteSpace(groupName))
                {
                    var gfs = await _db.GroupFeatureSettings
                                       .Where(g => g.Group == groupName)
                                       .OrderByDescending(g => g.UpdatedAt)
                                       .FirstOrDefaultAsync(ct);
                    if (gfs != null)
                        enabledSS = gfs.EnableSmartSelection;

                    // Expose effective group to downstream services (SmartSelection system prompts)
                    HttpContext.Items["PromptAiGroupOverride"] = groupName;
                }
            }
            catch { /* tolerate missing tables before migration */ }

            if (!enabledSS && !isAdmin)
                return Forbid("Smart Selection ist deaktiviert.");

            // optional operation id from header for progress reporting
            Request.Headers.TryGetValue("X-Op-Id", out var opIdHeader);
            var opId = (string?)opIdHeader.ToString();

            // optional deep-select flag (header or query)
            bool deep = false;
            try
            {
                if (Request.Headers.TryGetValue("X-Deep-Select", out var deepHeader))
                {
                    var s = deepHeader.ToString();
                    deep = string.Equals(s, "1") || string.Equals(s, "true", StringComparison.OrdinalIgnoreCase);
                }
                else if (Request.Query.TryGetValue("deep", out var deepQuery))
                {
                    var s = deepQuery.ToString();
                    deep = string.Equals(s, "1") || string.Equals(s, "true", StringComparison.OrdinalIgnoreCase);
                }
            }
            catch { }

            try
            {
                SmartSelectionResultDto result;
                if (!string.IsNullOrWhiteSpace(opId))
                    result = await _svc.SelectWithModelAsync(req.Form, targetType, deep, req.ModelId, opId!, ct);
                else
                    result = await _svc.SelectWithModelAsync(req.Form, targetType, deep, req.ModelId, ct);

                // Analytics: log successful Smart‑Selection call
                try
                {
                    if (ProjectsWebApp.Utility.FeatureFlags.EnableAnalytics)
                    {
                        string? analyticsGroup = null;
                        try
                        {
                            var grp = await _db.UserGroupMemberships
                                               .Where(m => m.UserId == userId)
                                               .OrderByDescending(m => m.CreatedAt)
                                               .Select(m => m.Group)
                                               .FirstOrDefaultAsync(ct);
                            analyticsGroup = string.IsNullOrWhiteSpace(grp) ? null : grp!.Trim();
                        }
                        catch { analyticsGroup = null; }

                        await _activityLogger.LogAsync(
                            userId,
                            string.IsNullOrWhiteSpace(analyticsGroup) ? null : analyticsGroup,
                            "smart_selection",
                            null,
                            new { targetType = targetType.ToString(), modelId = req.ModelId, deep },
                            ct);
                    }
                }
                catch { /* analytics must never break main flow */ }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                if (deep)
                {
                    return BadRequest(ex.Message);
                }
                throw;
            }
            catch (Exception)
            {
                if (deep)
                {
                    return BadRequest("Aktion konnte nicht ausgeführt werden. Bitte prüfen Sie die API‑Schlüssel‑Konfiguration der Gruppe.");
                }
                throw;
            }
        }

        [HttpGet("progress/{opId}")]
        public IActionResult GetProgress(string opId)
        {
            if (string.IsNullOrWhiteSpace(opId)) return BadRequest();
            var p = _progress.Get(opId);
            if (p is null) return NotFound();
            return Ok(p);
        }
    }
}
