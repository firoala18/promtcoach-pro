using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;

namespace ProjectsWebApp.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class GroupPromptsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userMgr;

        public GroupPromptsModel(ApplicationDbContext db, UserManager<IdentityUser> userMgr)
        {
            _db = db;
            _userMgr = userMgr;
        }

        [BindProperty(SupportsGet = true)]
        public string GroupName { get; set; } = string.Empty;

        [BindProperty] public bool UseGlobal { get; set; }
        [BindProperty] public string SystemPreamble { get; set; }
        [BindProperty] public string KiAssistantSystemPrompt { get; set; }
        [BindProperty] public string UserInstructionText { get; set; }
        [BindProperty] public string FilterSystemPreamble { get; set; }
        [BindProperty] public string FilterFirstLine { get; set; }
        [BindProperty] public string SmartSelectionSystemPreamble { get; set; }
        [BindProperty] public string SmartSelectionUserPrompt { get; set; }

        public bool DisableByOwner { get; private set; }
        public DateTime? LastUpdatedAt { get; private set; }
        public Dictionary<PromptType, string> TypeGuidances { get; private set; } = new();

        public async Task<IActionResult> OnGetAsync(string group)
        {
            GroupName = (group ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(GroupName))
            {
                // Default to current user's latest group if none provided
                try
                {
                    var uid = _userMgr.GetUserId(User);
                    var g = await _db.UserGroupMemberships
                                     .Where(m => m.UserId == uid)
                                     .OrderByDescending(m => m.CreatedAt)
                                     .Select(m => m.Group)
                                     .FirstOrDefaultAsync();
                    GroupName = string.IsNullOrWhiteSpace(g) ? "Ohne Gruppe" : g.Trim();
                }
                catch { GroupName = "Ohne Gruppe"; }
            }

            await LoadStateAsync();
            await LoadTypeGuidancesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Do not auto-disable by owner; allow Admin/Coach/Dozent owner to choose UseGlobal vs custom

            var isSuper = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
            var isDozent = User.IsInRole("Dozent");
            if (!isSuper)
            {
                if (!isDozent) return Forbid();
                var uid = _userMgr.GetUserId(User);
                bool owner;
                try { owner = await _db.DozentGroupOwnerships.AnyAsync(o => o.Group == GroupName && o.DozentUserId == uid); }
                catch { owner = false; }
                if (!owner) return Forbid();
            }

            try
            {
                var rec = new GroupPromptAiSetting
                {
                    Group = GroupName,
                    UseGlobal = UseGlobal,
                    SystemPreamble = string.IsNullOrWhiteSpace(SystemPreamble) ? null : SystemPreamble.Trim(),
                    KiAssistantSystemPrompt = string.IsNullOrWhiteSpace(KiAssistantSystemPrompt) ? null : KiAssistantSystemPrompt.Trim(),
                    UserInstructionText = string.IsNullOrWhiteSpace(UserInstructionText) ? null : UserInstructionText.Trim(),
                    FilterSystemPreamble = string.IsNullOrWhiteSpace(FilterSystemPreamble) ? null : FilterSystemPreamble.Trim(),
                    FilterFirstLine = string.IsNullOrWhiteSpace(FilterFirstLine) ? null : FilterFirstLine.Trim(),
                    SmartSelectionSystemPreamble = string.IsNullOrWhiteSpace(SmartSelectionSystemPreamble) ? null : SmartSelectionSystemPreamble.Trim(),
                    SmartSelectionUserPrompt = string.IsNullOrWhiteSpace(SmartSelectionUserPrompt) ? null : SmartSelectionUserPrompt.Trim(),
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedByUserId = _userMgr.GetUserId(User)
                };
                _db.GroupPromptAiSettings.Add(rec);
                await _db.SaveChangesAsync();
                TempData["success"] = "Prompts gespeichert.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Speichern fehlgeschlagen: {ex.Message}";
            }

            return RedirectToPage(new { group = GroupName });
        }

        public async Task<IActionResult> OnPostSaveTechniqueAsync()
        {
            var auth = await EnsureCanEditAsync();
            if (auth is ForbidResult || auth is UnauthorizedResult) return auth;

            try
            {
                var latest = await _db.GroupPromptAiSettings
                                       .Where(g => g.Group == GroupName)
                                       .OrderByDescending(g => g.UpdatedAt)
                                       .FirstOrDefaultAsync();
                var rec = new GroupPromptAiSetting
                {
                    Group = GroupName,
                    UseGlobal = UseGlobal,
                    SystemPreamble = string.IsNullOrWhiteSpace(SystemPreamble) ? latest?.SystemPreamble : SystemPreamble.Trim(),
                    KiAssistantSystemPrompt = string.IsNullOrWhiteSpace(KiAssistantSystemPrompt) ? latest?.KiAssistantSystemPrompt : KiAssistantSystemPrompt.Trim(),
                    UserInstructionText = string.IsNullOrWhiteSpace(UserInstructionText) ? latest?.UserInstructionText : UserInstructionText.Trim(),
                    FilterSystemPreamble = latest?.FilterSystemPreamble,
                    FilterFirstLine = latest?.FilterFirstLine,
                    SmartSelectionSystemPreamble = latest?.SmartSelectionSystemPreamble,
                    SmartSelectionUserPrompt = latest?.SmartSelectionUserPrompt,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedByUserId = _userMgr.GetUserId(User)
                };
                _db.GroupPromptAiSettings.Add(rec);
                await _db.SaveChangesAsync();
                TempData["success"] = "Prompt‑Generieren Abschnitt gespeichert.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Speichern fehlgeschlagen: {ex.Message}";
            }
            return RedirectToPage(new { group = GroupName });
        }

        public async Task<IActionResult> OnPostSaveFilterAsync()
        {
            var auth = await EnsureCanEditAsync();
            if (auth is ForbidResult || auth is UnauthorizedResult) return auth;

            try
            {
                var latest = await _db.GroupPromptAiSettings
                                       .Where(g => g.Group == GroupName)
                                       .OrderByDescending(g => g.UpdatedAt)
                                       .FirstOrDefaultAsync();
                var rec = new GroupPromptAiSetting
                {
                    Group = GroupName,
                    UseGlobal = UseGlobal,
                    SystemPreamble = latest?.SystemPreamble,
                    KiAssistantSystemPrompt = latest?.KiAssistantSystemPrompt,
                    UserInstructionText = latest?.UserInstructionText,
                    FilterSystemPreamble = string.IsNullOrWhiteSpace(FilterSystemPreamble) ? latest?.FilterSystemPreamble : FilterSystemPreamble.Trim(),
                    FilterFirstLine = string.IsNullOrWhiteSpace(FilterFirstLine) ? latest?.FilterFirstLine : FilterFirstLine.Trim(),
                    SmartSelectionSystemPreamble = latest?.SmartSelectionSystemPreamble,
                    SmartSelectionUserPrompt = latest?.SmartSelectionUserPrompt,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedByUserId = _userMgr.GetUserId(User)
                };
                _db.GroupPromptAiSettings.Add(rec);
                await _db.SaveChangesAsync();
                TempData["success"] = "Filter‑Abschnitt gespeichert.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Speichern fehlgeschlagen: {ex.Message}";
            }
            return RedirectToPage(new { group = GroupName });
        }

        public async Task<IActionResult> OnPostSaveSmartAsync()
        {
            var auth = await EnsureCanEditAsync();
            if (auth is ForbidResult || auth is UnauthorizedResult) return auth;

            try
            {
                var latest = await _db.GroupPromptAiSettings
                                       .Where(g => g.Group == GroupName)
                                       .OrderByDescending(g => g.UpdatedAt)
                                       .FirstOrDefaultAsync();
                var rec = new GroupPromptAiSetting
                {
                    Group = GroupName,
                    UseGlobal = UseGlobal,
                    SystemPreamble = latest?.SystemPreamble,
                    KiAssistantSystemPrompt = latest?.KiAssistantSystemPrompt,
                    UserInstructionText = latest?.UserInstructionText,
                    FilterSystemPreamble = latest?.FilterSystemPreamble,
                    FilterFirstLine = latest?.FilterFirstLine,
                    SmartSelectionSystemPreamble = string.IsNullOrWhiteSpace(SmartSelectionSystemPreamble) ? latest?.SmartSelectionSystemPreamble : SmartSelectionSystemPreamble.Trim(),
                    SmartSelectionUserPrompt = string.IsNullOrWhiteSpace(SmartSelectionUserPrompt) ? latest?.SmartSelectionUserPrompt : SmartSelectionUserPrompt.Trim(),
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedByUserId = _userMgr.GetUserId(User)
                };
                _db.GroupPromptAiSettings.Add(rec);
                await _db.SaveChangesAsync();
                TempData["success"] = "Smart‑Selection Abschnitt gespeichert.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Speichern fehlgeschlagen: {ex.Message}";
            }
            return RedirectToPage(new { group = GroupName });
        }

        public async Task<IActionResult> OnPostToggleModeAsync()
        {
            var auth = await EnsureCanEditAsync();
            if (auth is ForbidResult || auth is UnauthorizedResult) return auth;

            try
            {
                var latest = await _db.GroupPromptAiSettings
                                       .Where(g => g.Group == GroupName)
                                       .OrderByDescending(g => g.UpdatedAt)
                                       .FirstOrDefaultAsync();
                var rec = new GroupPromptAiSetting
                {
                    Group = GroupName,
                    UseGlobal = UseGlobal,
                    SystemPreamble = latest?.SystemPreamble,
                    KiAssistantSystemPrompt = latest?.KiAssistantSystemPrompt,
                    UserInstructionText = latest?.UserInstructionText,
                    FilterSystemPreamble = latest?.FilterSystemPreamble,
                    FilterFirstLine = latest?.FilterFirstLine,
                    SmartSelectionSystemPreamble = latest?.SmartSelectionSystemPreamble,
                    SmartSelectionUserPrompt = latest?.SmartSelectionUserPrompt,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedByUserId = _userMgr.GetUserId(User)
                };
                _db.GroupPromptAiSettings.Add(rec);
                await _db.SaveChangesAsync();
                TempData["success"] = UseGlobal ? "Globaler Modus aktiviert." : "Gruppen‑Prompts aktiviert.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Speichern fehlgeschlagen: {ex.Message}";
            }
            return RedirectToPage(new { group = GroupName });
        }

        private async Task<IActionResult> EnsureCanEditAsync()
        {
            var isSuper = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
            var isDozent = User.IsInRole("Dozent");
            if (!isSuper)
            {
                if (!isDozent) return Forbid();
                var uid = _userMgr.GetUserId(User);
                bool owner;
                try { owner = await _db.DozentGroupOwnerships.AnyAsync(o => o.Group == GroupName && o.DozentUserId == uid); }
                catch { owner = false; }
                if (!owner) return Forbid();
            }
            return Page();
        }

        private async Task LoadDisableAsync()
        {
            // Keep for future per-policy disable; for now do not disable automatically
            DisableByOwner = false;
        }

        private async Task LoadStateAsync()
        {
            await LoadDisableAsync();

            // Pre-fill with latest group prompts if any
            try
            {
                var gp = await _db.GroupPromptAiSettings
                                   .Where(g => g.Group == GroupName)
                                   .OrderByDescending(g => g.UpdatedAt)
                                   .FirstOrDefaultAsync();
                if (gp != null)
                {
                    UseGlobal = gp.UseGlobal ?? true;
                    SystemPreamble = gp.SystemPreamble;
                    KiAssistantSystemPrompt = gp.KiAssistantSystemPrompt;
                    UserInstructionText = gp.UserInstructionText;
                    FilterSystemPreamble = gp.FilterSystemPreamble;
                    FilterFirstLine = gp.FilterFirstLine;
                    SmartSelectionSystemPreamble = gp.SmartSelectionSystemPreamble;
                    SmartSelectionUserPrompt = gp.SmartSelectionUserPrompt;
                    LastUpdatedAt = gp.UpdatedAt;
                    return;
                }
            }
            catch { /* tolerate missing table until migration */ }

            // Otherwise leave empty (falls back to global in runtime resolution)
            UseGlobal = true;

        }

        private async Task LoadTypeGuidancesAsync()
        {
            try
            {
                // Global defaults per PromptType
                var globals = await _db.PromptTypeGuidances
                                       .GroupBy(x => x.Type)
                                       .Select(g => g.OrderByDescending(x => x.UpdatedAt).First())
                                       .ToListAsync();
                var globalDict = globals.ToDictionary(x => x.Type, x => x.GuidanceText ?? string.Empty);

                // Group-specific overrides
                var grp = (GroupName ?? string.Empty).Trim();
                var locals = await _db.GroupPromptTypeGuidances
                                      .Where(x => x.Group == grp)
                                      .GroupBy(x => x.Type)
                                      .Select(g => g.OrderByDescending(x => x.UpdatedAt).First())
                                      .ToListAsync();
                var localDict = locals.ToDictionary(x => x.Type, x => x.GuidanceText ?? string.Empty);

                TypeGuidances = new Dictionary<PromptType, string>();
                foreach (PromptType t in Enum.GetValues(typeof(PromptType)))
                {
                    if (localDict.TryGetValue(t, out var ltxt) && !string.IsNullOrWhiteSpace(ltxt))
                    {
                        TypeGuidances[t] = ltxt;
                    }
                    else if (globalDict.TryGetValue(t, out var gtxt) && !string.IsNullOrWhiteSpace(gtxt))
                    {
                        TypeGuidances[t] = gtxt;
                    }
                    else
                    {
                        TypeGuidances[t] = string.Empty;
                    }
                }
            }
            catch
            {
                TypeGuidances = new Dictionary<PromptType, string>();
            }
        }

        public async Task<IActionResult> OnPostSaveFilterTypesAsync(string groupName, List<string> types, List<string> texts)
        {
            var auth = await EnsureCanEditAsync();
            if (auth is ForbidResult || auth is UnauthorizedResult) return auth;

            GroupName = (groupName ?? GroupName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(GroupName))
            {
                TempData["error"] = "Ungültiger Gruppenname.";
                return RedirectToPage();
            }

            if (types == null || texts == null || types.Count != texts.Count)
            {
                TempData["error"] = "Ungültige Eingabe für Prompt‑Typen.";
                return RedirectToPage(new { group = GroupName });
            }

            try
            {
                var existing = await _db.GroupPromptTypeGuidances
                                        .Where(x => x.Group == GroupName)
                                        .ToListAsync();

                for (int i = 0; i < types.Count; i++)
                {
                    if (!Enum.TryParse<PromptType>(types[i], out var t)) continue;
                    var text = (texts[i] ?? string.Empty).Trim();

                    var row = existing.FirstOrDefault(x => x.Type == t);
                    if (row == null)
                    {
                        _db.GroupPromptTypeGuidances.Add(new GroupPromptTypeGuidance
                        {
                            Group = GroupName,
                            Type = t,
                            GuidanceText = text,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        row.GuidanceText = text;
                        row.UpdatedAt = DateTime.UtcNow;
                    }
                }

                await _db.SaveChangesAsync();
                TempData["success"] = "Typ‑Guidance für diese Gruppe gespeichert.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Speichern fehlgeschlagen: {ex.Message}";
            }

            return RedirectToPage(new { group = GroupName });
        }
    }
}
