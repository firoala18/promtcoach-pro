using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectsWebApp.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Roles = "SuperAdmin,Dozent,Admin")]
    public class ManageUsersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userMgr;
        private readonly SignInManager<IdentityUser> _signInMgr;
        private readonly ApplicationDbContext _db;

        public ManageUsersModel(UserManager<IdentityUser> userMgr,
                                 SignInManager<IdentityUser> signInMgr,
                                 ApplicationDbContext db)
        {
            _userMgr = userMgr;
            _signInMgr = signInMgr;
            _db = db;
        }

        public async Task<IActionResult> OnPostSaveGroupPromptsAsync()
        {
            var groupNorm = (GroupName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupNorm)) { TempData["error"] = "Ungültiger Gruppenname."; return RedirectToPage(); }

            var isSuper = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
            var isDozent = User.IsInRole("Dozent");

            if (!isSuper)
            {
                if (!isDozent) return Forbid();
                var uid = _userMgr.GetUserId(User);
                bool owner = false;
                try { owner = await _db.DozentGroupOwnerships.AnyAsync(o => o.Group == groupNorm && o.DozentUserId == uid); } catch { owner = false; }
                if (!owner) return Forbid();
            }

            try
            {
                var rec = new GroupPromptAiSetting
                {
                    Group = groupNorm,
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
                _db.Set<GroupPromptAiSetting>().Add(rec);
                await _db.SaveChangesAsync();
                TempData["success"] = $"Prompts für Gruppe '{groupNorm}' gespeichert.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Speichern fehlgeschlagen: {ex.Message}";
            }
            if (IsAjaxRequest())
                return new OkResult();

            return RedirectToPage();
        }

        private bool IsAjaxRequest()
        {
            try
            {
                if (Request?.Headers == null) return false;
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") return true;
            }
            catch { }
            return false;
        }

        public UserManager<IdentityUser> UserManager => _userMgr;

        [BindProperty] public string RemoveUserEmail { get; set; }
        [BindProperty] public string UpdateStatusEmail { get; set; }
        [BindProperty] public string UpdateStatusRole { get; set; }

        // Admin password reset form
        [BindProperty] public string ResetPasswordEmail { get; set; }
        [BindProperty] public string NewPassword { get; set; }
        [BindProperty] public string ConfirmPassword { get; set; }

        // Group API keys form
        [BindProperty] public string GroupName { get; set; }
        [BindProperty] public string ActiveProvider { get; set; }
        [BindProperty] public string KisskiApiKey { get; set; }
        [BindProperty] public string KisskiBaseUrl { get; set; }
        [BindProperty] public string KisskiModel { get; set; }
        [BindProperty] public string OpenAIKey { get; set; }
        [BindProperty] public string OpenAIBaseUrl { get; set; }
        [BindProperty] public string OpenAIModel { get; set; }
        [BindProperty] public string GeminiApiKey { get; set; }
        [BindProperty] public string ClaudeApiKey { get; set; }
        [BindProperty] public string GeminiModel { get; set; }
        [BindProperty] public string ClaudeModel { get; set; }

        public List<(string Group, List<IdentityUser> Users)> RegularUsersByGroup { get; } = new();
        public Dictionary<string, List<IdentityUser>> OwnersByGroup { get; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, UserActivity> UserActivitiesByUserId { get; } = new();
        public Dictionary<string, GroupApiKeySetting?> GroupApiKeysByGroup { get; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, GroupPromptAiSetting?> GroupPromptsByGroup { get; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string?> RoleAliasesByUserId { get; } = new(StringComparer.OrdinalIgnoreCase);
        public string CurrentUserId { get; private set; } = string.Empty;

        [BindProperty] public string SystemPreamble { get; set; }
        [BindProperty] public string KiAssistantSystemPrompt { get; set; }
        [BindProperty] public string UserInstructionText { get; set; }
        [BindProperty] public string FilterSystemPreamble { get; set; }
        [BindProperty] public string FilterFirstLine { get; set; }
        [BindProperty] public string SmartSelectionSystemPreamble { get; set; }
        [BindProperty] public string SmartSelectionUserPrompt { get; set; }

        private async Task<HashSet<string>> GetOwnedGroupsAsync()
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var uid = _userMgr.GetUserId(User);
                if (await _userMgr.IsInRoleAsync(await _userMgr.GetUserAsync(User)!, "Dozent"))
                {
                    var owned = await _db.DozentGroupOwnerships
                                         .Where(o => o.DozentUserId == uid)
                                         .Select(o => o.Group)
                                         .ToListAsync();
                    foreach (var g in owned.Where(s => !string.IsNullOrWhiteSpace(s))) set.Add(g!.Trim());
                }
            }
            catch { }
            return set;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            CurrentUserId = _userMgr.GetUserId(User) ?? string.Empty;
            // Owners by group
            OwnersByGroup.Clear();
            try
            {
                var owners = await _db.DozentGroupOwnerships.ToListAsync();
                var byGroup = owners
                    .Where(o => !string.IsNullOrWhiteSpace(o.Group))
                    .GroupBy(o => o.Group!.Trim(), StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.Select(o => o.DozentUserId).ToList(), StringComparer.OrdinalIgnoreCase);
                var allIds = byGroup.Values.SelectMany(x => x).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var idToUser = _userMgr.Users.Where(u => allIds.Contains(u.Id)).ToDictionary(u => u.Id, u => u, StringComparer.OrdinalIgnoreCase);
                foreach (var kv in byGroup)
                {
                    var list = new List<IdentityUser>();
                    foreach (var id in kv.Value)
                    {
                        if (idToUser.TryGetValue(id, out var u)) list.Add(u);
                    }
                    OwnersByGroup[kv.Key] = list;
                }
            }
            catch { }

            // Group API keys (latest per group by UpdatedAt)
            GroupApiKeysByGroup.Clear();
            try
            {
                var recs = await _db.Set<GroupApiKeySetting>().ToListAsync();
                foreach (var g in recs
                    .Where(r => !string.IsNullOrWhiteSpace(r.Group))
                    .GroupBy(r => r.Group!.Trim(), StringComparer.OrdinalIgnoreCase))
                {
                    var latest = g.OrderByDescending(x => x.UpdatedAt).First();
                    GroupApiKeysByGroup[g.Key] = latest;
                }
            }
            catch { }

            GroupPromptsByGroup.Clear();
            try
            {
                var recs = await _db.Set<GroupPromptAiSetting>().ToListAsync();
                foreach (var g in recs
                    .Where(r => !string.IsNullOrWhiteSpace(r.Group))
                    .GroupBy(r => r.Group!.Trim(), StringComparer.OrdinalIgnoreCase))
                {
                    var latest = g.OrderByDescending(x => x.UpdatedAt).First();
                    GroupPromptsByGroup[g.Key] = latest;
                }
            }
            catch { }

            // User activity timestamps
            try
            {
                var acts = await _db.UserActivities.ToListAsync();
                foreach (var a in acts)
                    UserActivitiesByUserId[a.UserId] = a;
            }
            catch { }

            // Role aliases per user (stored in AplicationUser)
            try
            {
                RoleAliasesByUserId.Clear();
                var aliasRows = await _db.AplicationUser.ToListAsync();
                foreach (var a in aliasRows)
                {
                    if (!string.IsNullOrWhiteSpace(a.RoleAlias))
                        RoleAliasesByUserId[a.Id] = a.RoleAlias.Trim();
                }
            }
            catch { }

            // Build ALL groups per user (multi-membership)
            Dictionary<string, HashSet<string>> membershipsByUser;
            bool membershipAvailable = true;
            try
            {
                var allMems = await _db.UserGroupMemberships
                    .Where(m => m.Group != null && m.Group != "")
                    .ToListAsync();
                membershipsByUser = allMems
                    .GroupBy(m => m.UserId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(m => m.Group!.Trim())
                              .Where(s => !string.IsNullOrWhiteSpace(s))
                              .ToHashSet(StringComparer.OrdinalIgnoreCase)
                    );
            }
            catch
            {
                membershipsByUser = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
                membershipAvailable = false;
            }

            var grouped = new Dictionary<string, List<IdentityUser>>(StringComparer.OrdinalIgnoreCase);

            var isSuper = User.IsInRole("SuperAdmin");
            var isDozent = User.IsInRole("Dozent");
            var dozentOwnedGroups = isDozent && !isSuper ? await GetOwnedGroupsAsync() : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> dozentMemberGroups = new(StringComparer.OrdinalIgnoreCase);
            if (isDozent && !isSuper && membershipAvailable)
            {
                try
                {
                    var myId = _userMgr.GetUserId(User);
                    if (!string.IsNullOrEmpty(myId) && membershipsByUser.TryGetValue(myId, out var set))
                    {
                        foreach (var g in set) dozentMemberGroups.Add(g);
                    }
                }
                catch { }
            }

            var allUsers = await _userMgr.Users.ToListAsync();
            foreach (var u in allUsers)
            {
                // Skip Admins & SuperAdmins (this page lists regular users, including Dozenten)
                if (await _userMgr.IsInRoleAsync(u, "Admin") || await _userMgr.IsInRoleAsync(u, "SuperAdmin"))
                    continue;

                // Determine all groups for this user
                List<string> userGroups;
                if (membershipsByUser.TryGetValue(u.Id, out var setForUser) && setForUser.Count > 0)
                {
                    userGroups = setForUser.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToList();
                }
                else
                {
                    userGroups = new List<string> { "Ohne Gruppe" };
                }

                foreach (var key in userGroups)
                {
                    if (isDozent && !isSuper && membershipAvailable)
                    {
                        bool allowed = false;
                        if (dozentOwnedGroups.Count > 0)
                            allowed = dozentOwnedGroups.Contains(key);
                        if (!allowed && dozentMemberGroups.Count > 0)
                            allowed = dozentMemberGroups.Contains(key);
        
                        if (!allowed) continue;
                    }

                    if (!grouped.TryGetValue(key, out var list))
                    {
                        list = new List<IdentityUser>();
                        grouped[key] = list;
                    }
                    list.Add(u);
                }
            }

            // Ensure empty groups are visible as well (from owners and registration codes)
            try
            {
                // Start with any groups already having members
                var allGroups = new HashSet<string>(grouped.Keys, StringComparer.OrdinalIgnoreCase);

                // Add groups that have owners
                foreach (var g in OwnersByGroup.Keys)
                {
                    if (!string.IsNullOrWhiteSpace(g)) allGroups.Add(g.Trim());
                }

                // Add groups referenced by active/archived registration codes
                try
                {
                    var codeGroups = await _db.RegistrationCodes
                        .Where(c => c.Group != null && c.Group != "")
                        .Select(c => c.Group!)
                        .ToListAsync();
                    foreach (var cg in codeGroups)
                    {
                        var norm = cg?.Trim();
                        if (!string.IsNullOrWhiteSpace(norm)) allGroups.Add(norm!);
                    }
                }
                catch { }

                // Filter for Dozent view: only show groups they own or any of their membership groups
                bool isSuper2 = User.IsInRole("SuperAdmin");
                bool isDozent2 = User.IsInRole("Dozent");
                HashSet<string> ownedForDozent = isDozent2 && !isSuper2 ? await GetOwnedGroupsAsync() : new(StringComparer.OrdinalIgnoreCase);
                HashSet<string> memberGroupsForDozent = new(StringComparer.OrdinalIgnoreCase);
                if (isDozent2 && !isSuper2)
                {
                    try
                    {
                        var myId = _userMgr.GetUserId(User);
                        if (!string.IsNullOrEmpty(myId) && membershipsByUser.TryGetValue(myId, out var set))
                        {
                            foreach (var mg in set) if (!string.IsNullOrWhiteSpace(mg)) memberGroupsForDozent.Add(mg.Trim());
                        }
                    }
                    catch { }
                }

                foreach (var g in allGroups)
                {
                    if (string.IsNullOrWhiteSpace(g)) continue;
                    // Respect dozent visibility constraints
                    if (isDozent2 && !isSuper2)
                    {
                        bool allowed = false;
                        if (ownedForDozent.Count > 0 && ownedForDozent.Contains(g)) allowed = true;
                        if (!allowed && memberGroupsForDozent.Count > 0 && memberGroupsForDozent.Contains(g)) allowed = true;
                        if (!allowed) continue;
                    }
                    if (!grouped.ContainsKey(g)) grouped[g] = new List<IdentityUser>();
                }
            }
            catch { }

            foreach (var kv in grouped.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
            {
                var users = kv.Value.OrderBy(x => x.Email, StringComparer.OrdinalIgnoreCase).ToList();
                RegularUsersByGroup.Add((kv.Key, users));
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync()
        {
            // SuperAdmin and Admin (Coach) share full privileges for this handler; Dozent has scoped rules
            var isSuper = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
            var isDozent = User.IsInRole("Dozent");

            if (!isSuper && !isDozent) return Forbid();
            if (string.IsNullOrWhiteSpace(UpdateStatusEmail) || string.IsNullOrWhiteSpace(UpdateStatusRole))
            {
                TempData["error"] = "Ungültige Eingabe.";
                return RedirectToPage();
            }

            var user = await _userMgr.FindByEmailAsync(UpdateStatusEmail);
            if (user == null) { TempData["error"] = "Benutzer nicht gefunden."; return RedirectToPage(); }

            var isSelf = UpdateStatusEmail.Equals(User.Identity?.Name, StringComparison.OrdinalIgnoreCase);

            // Parse role|alias (e.g., "Dozent|GruppenManager" or "User|Student")
            var raw = (UpdateStatusRole ?? string.Empty).Trim();
            string role = raw;
            string? alias = null;
            var split = raw.Split('|', 2);
            if (split.Length == 2)
            {
                role = split[0];
                alias = split[1];
            }

            // Permission constraints for Dozent/Forscher/Gruppen‑Manager
            if (isDozent && !isSuper)
            {
                // Dozent may not grant Admin/SuperAdmin/ApiManager
                if (role == "SuperAdmin" || role == "Admin" || role == "ApiManager")
                {
                    TempData["error"] = "Diese Rollen dürfen nur von Administratoren vergeben werden.";
                    return RedirectToPage();
                }

                if (isSelf)
                {
                    // Self: only switch within Dozent aliases
                    if (role != "Dozent")
                    {
                        TempData["error"] = "Sie können Ihre eigene Rolle nur innerhalb Forscher/Dozent/Gruppen‑Manager ändern.";
                        return RedirectToPage();
                    }
                }
                else
                {
                    // Never allow Dozent to modify Admin/SuperAdmin accounts (defense in depth)
                    if (await _userMgr.IsInRoleAsync(user, "SuperAdmin") || await _userMgr.IsInRoleAsync(user, "Admin"))
                        return Forbid();

                    // For group members: only switch between User and Dozent
                    if (role != "Dozent" && role != "User")
                    {
                        TempData["error"] = "Sie können nur zwischen Forscher/Dozent/Gruppen‑Manager und Student/Benutzer wechseln.";
                        return RedirectToPage();
                    }

                    // Check that target user belongs to a group this Dozent owns or is a member of
                    var owned = await GetOwnedGroupsAsync();
                    var memberGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    try
                    {
                        var myId = _userMgr.GetUserId(User);
                        if (!string.IsNullOrEmpty(myId))
                        {
                            var myGroups = await _db.UserGroupMemberships
                                .Where(m => m.UserId == myId && m.Group != null && m.Group != "")
                                .Select(m => m.Group!)
                                .ToListAsync();
                            foreach (var g in myGroups)
                            {
                                var norm = g?.Trim();
                                if (!string.IsNullOrWhiteSpace(norm))
                                    memberGroups.Add(norm);
                            }
                        }
                    }
                    catch { }

                    var targetGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    try
                    {
                        var tGroups = await _db.UserGroupMemberships
                            .Where(m => m.UserId == user.Id && m.Group != null && m.Group != "")
                            .Select(m => m.Group!)
                            .ToListAsync();
                        if (tGroups.Count == 0)
                        {
                            targetGroups.Add("Ohne Gruppe");
                        }
                        else
                        {
                            foreach (var g in tGroups)
                            {
                                var norm = g?.Trim();
                                if (!string.IsNullOrWhiteSpace(norm))
                                    targetGroups.Add(norm);
                            }
                        }
                    }
                    catch { }

                    bool allowedGroup = false;
                    foreach (var g in targetGroups)
                    {
                        if (owned.Contains(g) || memberGroups.Contains(g))
                        {
                            allowedGroup = true;
                            break;
                        }
                    }

                    if (!allowedGroup)
                        return Forbid();
                }
            }

            // Promote/demote similar to ManageAdmins
            if (role == "SuperAdmin")
            {
                if (!await _userMgr.IsInRoleAsync(user, "Admin"))
                    await _userMgr.AddToRoleAsync(user, "Admin");
                if (!await _userMgr.IsInRoleAsync(user, "SuperAdmin"))
                    await _userMgr.AddToRoleAsync(user, "SuperAdmin");
                if (await _userMgr.IsInRoleAsync(user, "Dozent"))
                    await _userMgr.RemoveFromRoleAsync(user, "Dozent");
            }
            else if (role == "Admin")
            {
                if (await _userMgr.IsInRoleAsync(user, "SuperAdmin"))
                    await _userMgr.RemoveFromRoleAsync(user, "SuperAdmin");
                if (!await _userMgr.IsInRoleAsync(user, "Admin"))
                    await _userMgr.AddToRoleAsync(user, "Admin");
                if (await _userMgr.IsInRoleAsync(user, "Dozent"))
                    await _userMgr.RemoveFromRoleAsync(user, "Dozent");
            }
            else if (role == "Dozent")
            {
                // Dozent: remove Admin/SuperAdmin/ApiManager, then ensure Dozent
                if (await _userMgr.IsInRoleAsync(user, "SuperAdmin"))
                    await _userMgr.RemoveFromRoleAsync(user, "SuperAdmin");
                if (await _userMgr.IsInRoleAsync(user, "Admin"))
                    await _userMgr.RemoveFromRoleAsync(user, "Admin");
                if (await _userMgr.IsInRoleAsync(user, "ApiManager"))
                    await _userMgr.RemoveFromRoleAsync(user, "ApiManager");
                if (!await _userMgr.IsInRoleAsync(user, "Dozent"))
                    await _userMgr.AddToRoleAsync(user, "Dozent");
            }
            else if (role == "ApiManager")
            {
                // ApiManager: make user exclusively ApiManager (no Admin/SuperAdmin/Dozent)
                if (await _userMgr.IsInRoleAsync(user, "SuperAdmin"))
                    await _userMgr.RemoveFromRoleAsync(user, "SuperAdmin");
                if (await _userMgr.IsInRoleAsync(user, "Admin"))
                    await _userMgr.RemoveFromRoleAsync(user, "Admin");
                if (await _userMgr.IsInRoleAsync(user, "Dozent"))
                    await _userMgr.RemoveFromRoleAsync(user, "Dozent");
                if (!await _userMgr.IsInRoleAsync(user, "ApiManager"))
                    await _userMgr.AddToRoleAsync(user, "ApiManager");
            }
            else // User
            {
                if (await _userMgr.IsInRoleAsync(user, "SuperAdmin")) await _userMgr.RemoveFromRoleAsync(user, "SuperAdmin");
                if (await _userMgr.IsInRoleAsync(user, "Admin")) await _userMgr.RemoveFromRoleAsync(user, "Admin");
                if (await _userMgr.IsInRoleAsync(user, "Dozent")) await _userMgr.RemoveFromRoleAsync(user, "Dozent");
                if (await _userMgr.IsInRoleAsync(user, "ApiManager")) await _userMgr.RemoveFromRoleAsync(user, "ApiManager");
            }

            await _userMgr.UpdateSecurityStampAsync(user);
            if (UpdateStatusEmail.Equals(User.Identity!.Name, StringComparison.OrdinalIgnoreCase))
            {
                await _signInMgr.RefreshSignInAsync(user);
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(alias))
                {
                    var appUser = await _db.AplicationUser.FirstOrDefaultAsync(a => a.Id == user.Id);
                    if (appUser == null)
                    {
                        appUser = new AplicationUser
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            Name = user.Email ?? ""
                        };
                        appUser.RoleAlias = alias;
                        _db.AplicationUser.Add(appUser);
                    }
                    else
                    {
                        appUser.RoleAlias = alias;
                        _db.AplicationUser.Update(appUser);
                    }
                    await _db.SaveChangesAsync();
                }
            }
            catch { }

            TempData["success"] = "Rolle wurde aktualisiert.";

            // For AJAX requests (e.g. Admin Groups dashboard) avoid redirecting
            // to this page and just return a 200 OK.
            if (IsAjaxRequest())
                return new OkResult();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveUserAsync()
        {
            var isSuper = User.IsInRole("SuperAdmin");
            var isDozent = User.IsInRole("Dozent");
            if (!isSuper && !isDozent) return Forbid();
            if (string.IsNullOrWhiteSpace(RemoveUserEmail)) { TempData["error"] = "Ungültige Eingabe."; return RedirectToPage(); }

            var user = await _userMgr.FindByEmailAsync(RemoveUserEmail);
            if (user == null) { TempData["error"] = "Benutzer nicht gefunden."; return RedirectToPage(); }

            if (!isSuper)
            {
                if (await _userMgr.IsInRoleAsync(user, "Admin") || await _userMgr.IsInRoleAsync(user, "SuperAdmin") || await _userMgr.IsInRoleAsync(user, "Dozent"))
                    return Forbid();
                string? groupName;
                try
                {
                    groupName = await _db.UserGroupMemberships
                        .Where(m => m.UserId == user.Id)
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => m.Group)
                        .FirstOrDefaultAsync();
                }
                catch { groupName = null; }
                var key = string.IsNullOrWhiteSpace(groupName) ? "Ohne Gruppe" : groupName.Trim();
                var owned = await GetOwnedGroupsAsync();
                if (owned.Count == 0 || !owned.Contains(key)) return Forbid();
            }

            // Cleanup related app records before deleting identity user
            try
            {
                var uid = user.Id;
                var mems = _db.UserGroupMemberships.Where(m => m.UserId == uid);
                _db.UserGroupMemberships.RemoveRange(mems);

                var acts = _db.UserActivities.Where(a => a.UserId == uid);
                _db.UserActivities.RemoveRange(acts);

                var owns = _db.DozentGroupOwnerships.Where(o => o.DozentUserId == uid);
                _db.DozentGroupOwnerships.RemoveRange(owns);

                // Remove possible custom AplicationUser row if present
                try
                {
                    var appUser = await _db.AplicationUser.FirstOrDefaultAsync(a => a.Id == uid);
                    if (appUser != null) _db.AplicationUser.Remove(appUser);
                }
                catch { }

                await _db.SaveChangesAsync();
            }
            catch { }

            var del = await _userMgr.DeleteAsync(user);
            TempData[del.Succeeded ? "success" : "error"] = del.Succeeded ? $"Benutzer {RemoveUserEmail} gelöscht." : "Fehler beim Löschen.";

            if (IsAjaxRequest())
                return new OkResult();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostResetPasswordAsync()
        {
            // Only SuperAdmin or Admin may reset passwords
            if (!(User.IsInRole("SuperAdmin") || User.IsInRole("Admin")))
                return Forbid();

            var email = (ResetPasswordEmail ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                TempData["error"] = "Bitte E-Mail und beide Passwortfelder ausfüllen.";
                return RedirectToPage();
            }

            if (!string.Equals(NewPassword, ConfirmPassword, StringComparison.Ordinal))
            {
                TempData["error"] = "Die Passwörter stimmen nicht überein.";
                return RedirectToPage();
            }

            if (NewPassword.Length < 6)
            {
                TempData["error"] = "Das Passwort muss mindestens 6 Zeichen lang sein.";
                return RedirectToPage();
            }

            var user = await _userMgr.FindByEmailAsync(email);
            if (user == null)
            {
                TempData["error"] = "Benutzer nicht gefunden.";
                return RedirectToPage();
            }

            try
            {
                // Use Identity's reset token flow so that password validators are respected
                var token = await _userMgr.GeneratePasswordResetTokenAsync(user);
                var result = await _userMgr.ResetPasswordAsync(user, token, NewPassword);
                if (!result.Succeeded)
                {
                    var firstError = result.Errors.FirstOrDefault()?.Description ?? "Unbekannter Fehler.";
                    TempData["error"] = $"Passwort konnte nicht gesetzt werden: {firstError}";
                    return RedirectToPage();
                }

                await _userMgr.UpdateSecurityStampAsync(user);
                TempData["success"] = $"Neues Passwort für {email} wurde gesetzt.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Fehler beim Zurücksetzen des Passworts: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddOwnerAsync(string groupName, string ownerEmail)
        {
            var groupNorm = (groupName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupNorm)) { TempData["error"] = "Ungültiger Gruppenname."; return RedirectToPage(); }
            if (string.IsNullOrWhiteSpace(ownerEmail)) { TempData["error"] = "E-Mail erforderlich."; return RedirectToPage(); }

            var isSuper = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
            var isDozent = User.IsInRole("Dozent");

            if (!isSuper)
            {
                if (!isDozent) return Forbid();
                // Only Dozent owners of this group may edit
                var uid = _userMgr.GetUserId(User);
                bool owner = false;
                try
                {
                    owner = await _db.DozentGroupOwnerships.AnyAsync(o => o.Group == groupNorm && o.DozentUserId == uid);
                }
                catch { owner = false; }
                if (!owner) return Forbid();
            }

            var user = await _userMgr.FindByEmailAsync(ownerEmail);
            if (user == null) { TempData["error"] = "Benutzer nicht gefunden."; return RedirectToPage(); }
            var targetIsAdmin = await _userMgr.IsInRoleAsync(user, "Admin");
            var targetIsCoach = await _userMgr.IsInRoleAsync(user, "Coach");
            //var isDozent = await _userMgr.IsInRoleAsync(user, "Dozent");
            var targetIsDozent = await _userMgr.IsInRoleAsync(user, "Dozent");
            var targetIsApiManager = await _userMgr.IsInRoleAsync(user, "ApiManager");
            if (!(targetIsAdmin || targetIsCoach || targetIsDozent || targetIsApiManager)) { TempData["error"] = "Nur Admins, Coaches, Dozenten oder API‑Manager können Besitzer sein."; return RedirectToPage(); }

            var exists = await _db.DozentGroupOwnerships.AnyAsync(o => o.Group == groupNorm && o.DozentUserId == user.Id);
            if (!exists)
            {
                _db.DozentGroupOwnerships.Add(new DozentGroupOwnership { Group = groupNorm, DozentUserId = user.Id, CreatedAt = DateTime.UtcNow });
                await _db.SaveChangesAsync();
                TempData["success"] = $"{ownerEmail} als Besitzer hinzugefügt.";
            }
            else
            {
                TempData["success"] = $"{ownerEmail} ist bereits Besitzer.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveOwnerAsync(string groupName, string ownerId)
        {
            var groupNorm = (groupName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupNorm) || string.IsNullOrWhiteSpace(ownerId)) { TempData["error"] = "Ungültige Eingabe."; return RedirectToPage(); }

            var isSuper = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
            var isDozent = User.IsInRole("Dozent");

            if (!isSuper)
            {
                if (!isDozent) return Forbid();
                var uid = _userMgr.GetUserId(User);
                bool owner = false;
                try
                {
                    owner = await _db.DozentGroupOwnerships.AnyAsync(o => o.Group == groupNorm && o.DozentUserId == uid);
                }
                catch { owner = false; }
                if (!owner) return Forbid();
            }

            var rec = await _db.DozentGroupOwnerships.FirstOrDefaultAsync(o => o.Group == groupNorm && o.DozentUserId == ownerId);
            if (rec == null) { TempData["error"] = "Besitzerzuordnung nicht gefunden."; return RedirectToPage(); }
            _db.DozentGroupOwnerships.Remove(rec);
            await _db.SaveChangesAsync();
            TempData["success"] = "Besitzer entfernt.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteGroupUsersAsync(string groupName)
        {
            if (!User.IsInRole("SuperAdmin")) return Forbid();
            if (groupName == null) { TempData["error"] = "Ungültiger Gruppenname."; return RedirectToPage(); }

            Dictionary<string, string?> groupLookup;
            try
            {
                groupLookup = await _db.UserGroupMemberships
                    .GroupBy(m => m.UserId)
                    .Select(g => new { UserId = g.Key, Group = g.OrderByDescending(x => x.CreatedAt).Select(x => x.Group).FirstOrDefault() })
                    .ToDictionaryAsync(x => x.UserId, x => x.Group);
            }
            catch { groupLookup = new Dictionary<string, string?>(); }

            bool deleteNoGroup = string.Equals(groupName, "Ohne Gruppe", StringComparison.OrdinalIgnoreCase);
            var adminSet = new HashSet<string>((await _userMgr.GetUsersInRoleAsync("Admin")).Select(u => u.Id));
            var superSet = new HashSet<string>((await _userMgr.GetUsersInRoleAsync("SuperAdmin")).Select(u => u.Id));
            var dozentSet = new HashSet<string>((await _userMgr.GetUsersInRoleAsync("Dozent")).Select(u => u.Id));

            var allUsers = _userMgr.Users.ToList();
            var targets = allUsers.Where(u =>
            {
                if (adminSet.Contains(u.Id) || superSet.Contains(u.Id) || dozentSet.Contains(u.Id)) return false;
                if (!groupLookup.TryGetValue(u.Id, out var grp)) return deleteNoGroup;
                var hasGroup = !string.IsNullOrWhiteSpace(grp);
                if (deleteNoGroup) return !hasGroup;
                return string.Equals(grp, groupName, StringComparison.OrdinalIgnoreCase);
            }).ToList();

            // Even if there are no users to delete, proceed to remove the group itself
            if (targets.Count == 0)
            {
                // fall through to group removal below
            }

            try
            {
                var ids = targets.Select(t => t.Id).ToList();
                var memberships = _db.UserGroupMemberships.Where(m => ids.Contains(m.UserId));
                _db.UserGroupMemberships.RemoveRange(memberships);
                await _db.SaveChangesAsync();
            }
            catch { }

            int deleted = 0;
            foreach (var u in targets)
            {
                var res = await _userMgr.DeleteAsync(u);
                if (res.Succeeded) deleted++;
            }

            // If this is a named group (not "Ohne Gruppe"), remove the entire group and related records
            if (!deleteNoGroup)
            {
                var groupNorm = (groupName ?? string.Empty).Trim();
                try
                {
                    var grp = _db.Groups
                                 .AsEnumerable()
                                 .FirstOrDefault(g => !string.IsNullOrWhiteSpace(g.Name)
                                                   && string.Equals(g.Name.Trim(), groupNorm, StringComparison.OrdinalIgnoreCase));

                    if (grp != null)
                    {
                        var assocById = _db.PromptTemplateGroups.Where(pg => pg.GroupId == grp.Id);
                        _db.PromptTemplateGroups.RemoveRange(assocById);
                        var asstById = _db.AssistantGroups.Where(ag => ag.GroupId == grp.Id);
                        _db.AssistantGroups.RemoveRange(asstById);
                    }

                    var assocByLegacy = _db.PromptTemplateGroups.Where(pg => pg.Group != null && pg.Group.Trim() == groupNorm);
                    _db.PromptTemplateGroups.RemoveRange(assocByLegacy);
                    var asstByLegacy = _db.AssistantGroups.Where(ag => ag.Group != null && ag.Group.Trim() == groupNorm);
                    _db.AssistantGroups.RemoveRange(asstByLegacy);

                    var owners = _db.DozentGroupOwnerships.Where(o => o.Group == groupNorm);
                    _db.DozentGroupOwnerships.RemoveRange(owners);

                    var memsAll = _db.UserGroupMemberships.Where(m => m.Group == groupNorm);
                    _db.UserGroupMemberships.RemoveRange(memsAll);

                    var apiKeys = _db.Set<GroupApiKeySetting>().Where(x => x.Group == groupNorm);
                    _db.RemoveRange(apiKeys);

                    var prompts = _db.Set<GroupPromptAiSetting>().Where(x => x.Group == groupNorm);
                    _db.RemoveRange(prompts);

                    try
                    {
                        var codes = _db.RegistrationCodes
                                       .AsEnumerable()
                                       .Where(c => !string.IsNullOrWhiteSpace(c.Group) && string.Equals(c.Group!.Trim(), groupNorm, StringComparison.OrdinalIgnoreCase))
                                       .ToList();
                        if (codes.Count > 0)
                        {
                            _db.RegistrationCodes.RemoveRange(codes);
                        }
                    }
                    catch { }

                    await _db.SaveChangesAsync();

                    if (grp != null)
                    {
                        _db.Groups.Remove(grp);
                        await _db.SaveChangesAsync();
                    }
                }
                catch { }
            }

            TempData["success"] = $"{deleted} Benutzer aus Gruppe '{groupName}' gelöscht und Gruppe entfernt.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSaveGroupApiKeysAsync()
        {
            var groupNorm = (GroupName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupNorm)) { TempData["error"] = "Ungültiger Gruppenname."; return RedirectToPage(); }

            var isSuper = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
            var isDozent = User.IsInRole("Dozent");

            if (!isSuper)
            {
                if (!isDozent) return Forbid();
                // Only Dozent owners of this group may edit
                var uid = _userMgr.GetUserId(User);
                bool owner = false;
                try
                {
                    owner = await _db.DozentGroupOwnerships.AnyAsync(o => o.Group == groupNorm && o.DozentUserId == uid);
                }
                catch { owner = false; }
                if (!owner) return Forbid();
            }

            try
            {
                var prev = await _db.Set<GroupApiKeySetting>()
                                    .Where(g => g.Group == groupNorm)
                                    .OrderByDescending(g => g.UpdatedAt)
                                    .FirstOrDefaultAsync();
                var rec = new GroupApiKeySetting
                {
                    Group = groupNorm,
                    ActiveProvider = string.IsNullOrWhiteSpace(ActiveProvider) ? (prev?.ActiveProvider) : ActiveProvider.Trim().ToLowerInvariant(),
                    KisskiApiKey = string.IsNullOrWhiteSpace(KisskiApiKey) ? prev?.KisskiApiKey : KisskiApiKey.Trim(),
                    KisskiModel = string.IsNullOrWhiteSpace(KisskiModel) ? prev?.KisskiModel : KisskiModel.Trim(),

                    OpenAIKey = string.IsNullOrWhiteSpace(OpenAIKey) ? prev?.OpenAIKey : OpenAIKey.Trim(),
                    OpenAIModel = string.IsNullOrWhiteSpace(OpenAIModel) ? prev?.OpenAIModel : OpenAIModel.Trim(),

                    GeminiApiKey = string.IsNullOrWhiteSpace(GeminiApiKey) ? prev?.GeminiApiKey : GeminiApiKey.Trim(),
                    GeminiModel = string.IsNullOrWhiteSpace(GeminiModel) ? prev?.GeminiModel : GeminiModel.Trim(),

                    ClaudeApiKey = string.IsNullOrWhiteSpace(ClaudeApiKey) ? prev?.ClaudeApiKey : ClaudeApiKey.Trim(),
                    ClaudeModel = string.IsNullOrWhiteSpace(ClaudeModel) ? prev?.ClaudeModel : ClaudeModel.Trim(),

                    UpdatedAt = DateTime.UtcNow,
                    UpdatedByUserId = _userMgr.GetUserId(User)
                };
                _db.Set<GroupApiKeySetting>().Add(rec);
                await _db.SaveChangesAsync();
                TempData["success"] = $"API‑Schlüssel für Gruppe '{groupNorm}' gespeichert.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Speichern fehlgeschlagen: {ex.Message}";
            }
            return RedirectToPage();
        }
    }
}
