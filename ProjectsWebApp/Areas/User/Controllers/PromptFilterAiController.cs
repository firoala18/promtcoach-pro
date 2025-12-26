using Dto.PromptFilters;
using Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.DataAccsess.Data;
using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using ProjectsWebApp.Models;
using ProjectsWebApp.Utility;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ProjectsWebApp.Areas.User.Controllers
{
    [Area("User")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public sealed class PromptFilterAiController : ControllerBase
    {
        private readonly IPromptFilterAiService _ai;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<IdentityUser> _um;
        private readonly ApplicationDbContext _db;
        private readonly IUserActivityLogger _activityLogger;

        public PromptFilterAiController(IPromptFilterAiService ai,
                                        IUnitOfWork uow,
                                        UserManager<IdentityUser> um,
                                        ApplicationDbContext db,
                                        IUserActivityLogger activityLogger)
            => (_ai, _uow, _um, _db, _activityLogger) = (ai, uow, um, db, activityLogger);
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GenerateFiltersRequestDto req, CancellationToken ct)
        {
            if (req is null || req.Form is null)
                return BadRequest("Ungültige Anfrage.");

            // map incoming TargetType string → PromptType enum (default Text)
            var targetType = Enum.TryParse<PromptType>(req.TargetType, ignoreCase: true, out var parsed)
                ? parsed
                : PromptType.Text;

            // role restriction + feature toggle
            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            var userId = _um.GetUserId(User)!;

            // Determine effective feature flag (per-group -> global -> static)
            bool enabledFG = FeatureFlags.EnableFilterGeneration; // fallback
            string? groupName = null;
            try
            {
                var globalRec = await _db.PromptFeatureSettings
                                         .OrderByDescending(x => x.UpdatedAt)
                                         .FirstOrDefaultAsync(ct);
                if (globalRec != null)
                    enabledFG = globalRec.EnableFilterGeneration;

                // Prefer explicit group from request; fall back to latest membership
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
                        enabledFG = gfs.EnableFilterGeneration;

                    // Expose effective group to downstream services (Filter-AI system prompts)
                    HttpContext.Items["PromptAiGroupOverride"] = groupName;
                }
            }
            catch { /* tolerate missing tables before migration */ }

            // If disabled, still allow Admins, block others
            if (!enabledFG && !isAdmin)
                return Forbid("Filter-Generierung ist deaktiviert.");

            // role restriction: only admins can generate for non-Eigenfilter types
            if (!isAdmin && targetType != PromptType.Eigenfilter)
                return Forbid("Nur im Eigenfilter verfügbar.");

            PromptFilterPayloadDto payload;
            try
            {
                payload = await _ai.GenerateAsync(req.Form, targetType, req.ModelId, ct);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Surface as 400 to allow UI to show message and stop loading
                return BadRequest($"Fehler bei der Filter-Generierung: {ex.Message}");
            }
            // userId already computed above
            var batchId = Guid.NewGuid(); // group this generation

            // Enforce: only ONE category, with up to 8 items (3–8 expected from the model)
            var catDto = payload.Data.OrderBy(c => c.DisplayOrder).FirstOrDefault();
            if (catDto == null)
                return BadRequest("KI hat keine Kategorie geliefert.");

            // Use Items (mapped from filterItems) and clamp to max 8, keep original order
            var items = (catDto.Items ?? new List<PromptFilterItemDto>())
                        .OrderBy(i => i.SortOrder)
                        .Take(8)
                        .ToList();

            if (items.Count < 3)
                return BadRequest("KI hat weniger als 3 Filter‑Items geliefert.");

            // Decide on category name: manual override or AI suggestion (fallback if empty)
            var manualMode = string.Equals(req.CategoryTitleMode, "manual", StringComparison.OrdinalIgnoreCase);
            var chosenName = (manualMode ? (req.CategoryTitle ?? string.Empty) : string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(chosenName))
                chosenName = (catDto.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(chosenName))
                chosenName = "Strukturiert"; // safe fallback to prevent DB errors

            var cat = new FilterCategory
            {
                UserId = (isAdmin && targetType != PromptType.Eigenfilter) ? null : userId,
                Type = targetType,
                Name = chosenName,
                DisplayOrder = 0,
                IsAiGenerated = true,
                AiBatchId = batchId,
                CreatedAt = DateTime.UtcNow
            };
            try
            {
                _uow.FilterCategory.Add(cat);
                _uow.Save(); // need cat.Id

                for (int idx = 0; idx < items.Count; idx++)
                {
                    var itm = items[idx];
                    _uow.FilterItem.Add(new FilterItem
                    {
                        FilterCategoryId = cat.Id,
                        Title = itm.Title ?? string.Empty,
                        Info = itm.Info ?? string.Empty,
                        Instruction = itm.Instruction ?? string.Empty,
                        SortOrder = idx
                    });
                }

                _uow.Save();
            }
            catch (Exception ex)
            {
                return BadRequest($"Filter konnten nicht gespeichert werden: {ex.Message}");
            }

            // Build sanitized payload (one category, ≤ 8 items, normalized sort orders)
            var sanitized = new global::Dto.PromptFilterPayloadDto
            {
                Data = new List<PromptFilterCategoryDto>
                {
                    new PromptFilterCategoryDto
                    {
                        Id = catDto.Id,
                        Name = chosenName,
                        DisplayOrder = 0,
                        Items = items.Select((it, i) => new PromptFilterItemDto
                        {
                            Id = it.Id,
                            Title = it.Title,
                            Info = it.Info,
                            Instruction = it.Instruction,
                            SortOrder = i
                        }).ToList()
                    }
                },
                Hash = payload.Hash
            };

            // Analytics: log successful filter generation with basic metadata
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
                        "filter_generate",
                        null,
                        new { targetType = targetType.ToString(), modelId = req.ModelId, itemCount = items.Count },
                        ct);
                }
            }
            catch { /* analytics must never break main flow */ }

            return Ok(sanitized); // client may show a summary
        }
    }

}
