using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using ProjectsWebApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectsWebApp.Areas.Identity.Pages.Account.Manage
{

    [Authorize(Roles = "SuperAdmin,Dozent,Admin")]
    public class ManageAdminsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userMgr;
        private readonly SignInManager<IdentityUser> _signInMgr;
        private readonly ApplicationDbContext _db;

        public ManageAdminsModel(UserManager<IdentityUser> userMgr,
                                 SignInManager<IdentityUser> signInMgr,
                                 ApplicationDbContext db)
        {
            _userMgr = userMgr;
            _signInMgr = signInMgr;
            _db = db;
        }

        public UserManager<IdentityUser> UserManager => _userMgr;

        [BindProperty] public string NewAdminEmailRegister { get; set; }
        [BindProperty] public string NewAdminName { get; set; }
        [BindProperty] public string NewAdminPassword { get; set; }
        [BindProperty] public string RemoveAdminEmail { get; set; }
        [BindProperty] public string UpdateStatusEmail { get; set; }
        [BindProperty] public string UpdateStatusRole { get; set; }
        [BindProperty] public string UpdateStatusGroup { get; set; }

        [BindProperty] public string RemoveUserEmail { get; set; }

        public List<IdentityUser> AdminUsers { get; } = new();
        public List<IdentityUser> RegularUsers { get; } = new();
        public List<(string Group, List<IdentityUser> Users)> RegularUsersByGroup { get; } = new();
        public Dictionary<string, GroupFeatureSetting> GroupFeatureFlags { get; } = new(StringComparer.OrdinalIgnoreCase);
        public bool DefaultFilterEnabled { get; private set; }
        public bool DefaultSmartEnabled { get; private set; }
        public Dictionary<string, UserActivity> UserActivitiesByUserId { get; } = new();
        public Dictionary<string, List<IdentityUser>> OwnersByGroup { get; } = new(StringComparer.OrdinalIgnoreCase);
        public List<string> AllGroups { get; } = new();

        private async Task<int> TotalAdminsAsync() =>
            (await _userMgr.GetUsersInRoleAsync("Admin")).Count +
            (await _userMgr.GetUsersInRoleAsync("SuperAdmin")).Count;

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
            catch { /* table may not exist yet */ }
            return set;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Load default/global feature flags (persisted) as fallback
            try
            {
                var rec = await _db.PromptFeatureSettings
                                   .OrderByDescending(x => x.UpdatedAt)
                                   .FirstOrDefaultAsync();
                DefaultFilterEnabled = rec?.EnableFilterGeneration ?? Utility.FeatureFlags.EnableFilterGeneration;
                DefaultSmartEnabled = rec?.EnableSmartSelection ?? Utility.FeatureFlags.EnableSmartSelection;
            }
            catch
            {
                DefaultFilterEnabled = Utility.FeatureFlags.EnableFilterGeneration;
                DefaultSmartEnabled = Utility.FeatureFlags.EnableSmartSelection;
            }

            // Load group feature flags (latest per group) — normalize keys
            GroupFeatureFlags.Clear();
            try
            {
                var gfs = await _db.GroupFeatureSettings.ToListAsync();
                foreach (var g in gfs
                         .GroupBy(x => (x.Group ?? string.Empty).Trim(), StringComparer.OrdinalIgnoreCase)
                         .Select(grp => grp.OrderByDescending(x => x.UpdatedAt).First()))
                {
                    var key = string.IsNullOrWhiteSpace(g.Group) ? "Ohne Gruppe" : g.Group.Trim();
                    GroupFeatureFlags[key] = g;
                }
            }
            catch { /* ignore if table not present yet */ }

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

            // Load user activity timestamps
            try
            {
                var acts = await _db.UserActivities.ToListAsync();
                foreach (var a in acts)
                    UserActivitiesByUserId[a.UserId] = a;
            }
            catch { /* ignore if table not present yet */ }

            // Build a lookup of latest group per user (robust if migrations not applied yet)
            Dictionary<string, string?> groupLookup;
            bool membershipAvailable = true;
            try
            {
                groupLookup = await _db.UserGroupMemberships
                    .GroupBy(m => m.UserId)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        Group = g.OrderByDescending(x => x.CreatedAt)
                                 .Select(x => x.Group)
                                 .FirstOrDefault()
                    })
                    .ToDictionaryAsync(x => x.UserId, x => x.Group);
            }
            catch
            {
                groupLookup = new Dictionary<string, string?>();
                membershipAvailable = false;
            }

            // Collect all known groups for selection (excluding "Ohne Gruppe")
            AllGroups.Clear();
            var groupSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // From latest memberships
            foreach (var g in groupLookup.Values)
            {
                var norm = g?.Trim();
                if (!string.IsNullOrWhiteSpace(norm))
                    groupSet.Add(norm);
            }

            // From feature flags
            foreach (var key in GroupFeatureFlags.Keys)
            {
                var norm = key?.Trim();
                if (!string.IsNullOrWhiteSpace(norm) &&
                    !string.Equals(norm, "Ohne Gruppe", StringComparison.OrdinalIgnoreCase))
                {
                    groupSet.Add(norm);
                }
            }

            // From group owners
            foreach (var key in OwnersByGroup.Keys)
            {
                var norm = key?.Trim();
                if (!string.IsNullOrWhiteSpace(norm))
                    groupSet.Add(norm);
            }

            // From registration codes
            try
            {
                var codeGroups = await _db.RegistrationCodes
                    .Where(c => c.Group != null && c.Group != "")
                    .Select(c => c.Group!)
                    .ToListAsync();
                foreach (var cg in codeGroups)
                {
                    var norm = cg?.Trim();
                    if (!string.IsNullOrWhiteSpace(norm))
                        groupSet.Add(norm);
                }
            }
            catch { }

            AllGroups.AddRange(groupSet.OrderBy(x => x, StringComparer.OrdinalIgnoreCase));

            // Partition users into Admins and Regulars, and bucket regulars by group
            var grouped = new Dictionary<string, List<IdentityUser>>(StringComparer.OrdinalIgnoreCase);

            var isSuper = User.IsInRole("SuperAdmin");
            var isDozent = User.IsInRole("Dozent");
            var dozentOwnedGroups = isDozent && !isSuper ? await GetOwnedGroupsAsync() : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string? dozentMemberGroup = null;
            if (isDozent && !isSuper && membershipAvailable)
            {
                try
                {
                    var myId = _userMgr.GetUserId(User);
                    groupLookup.TryGetValue(myId, out var g);
                    dozentMemberGroup = g?.Trim();
                    if (string.IsNullOrWhiteSpace(dozentMemberGroup)) dozentMemberGroup = null;
                }
                catch { /* ignore */ }
            }

            // IMPORTANT: materialize before awaiting inside loop to avoid concurrent DataReader issues on PostgreSQL
            var allUsers = await _userMgr.Users.ToListAsync();
            foreach (var u in allUsers)
            {
                var isAdminRole = await _userMgr.IsInRoleAsync(u, "Admin");
                var isSuperAdminRole = await _userMgr.IsInRoleAsync(u, "SuperAdmin");
                var isDozentRole = await _userMgr.IsInRoleAsync(u, "Dozent");
                var isApiManagerRole = await _userMgr.IsInRoleAsync(u, "ApiManager");

                // Include Admin, SuperAdmin, Dozent, and ApiManager in AdminUsers
                if (isAdminRole || isSuperAdminRole || isDozentRole || isApiManagerRole)
                {
                    AdminUsers.Add(u);
                    continue;
                }

                RegularUsers.Add(u);
                groupLookup.TryGetValue(u.Id, out var grpRaw);
                var norm = grpRaw?.Trim();
                var key = string.IsNullOrWhiteSpace(norm) ? "Ohne Gruppe" : norm!;
                if (isDozent && !isSuper && membershipAvailable)
                {
                    bool allowed = false;
                    if (dozentOwnedGroups.Count > 0)
                        allowed = dozentOwnedGroups.Contains(key);
                    if (!allowed && dozentMemberGroup != null)
                        allowed = string.Equals(key, dozentMemberGroup, StringComparison.OrdinalIgnoreCase);
                    if (!allowed)
                        continue; // hide groups not owned or not the dozent's own membership group
                }
                if (!grouped.TryGetValue(key, out var list))
                {
                    list = new List<IdentityUser>();
                    grouped[key] = list;
                }
                list.Add(u);
            }

            // Sort groups and users for stable display
            foreach (var kv in grouped.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
            {
                var users = kv.Value.OrderBy(x => x.Email, StringComparer.OrdinalIgnoreCase).ToList();
                RegularUsersByGroup.Add((kv.Key, users));
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSetGroupFeaturesAsync(string groupName, bool enableFilterGeneration, bool enableSmartSelection)
        {
            var groupNorm = (groupName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupNorm))
            {
                groupNorm = "Ohne Gruppe";
            }

            if (string.IsNullOrWhiteSpace(groupNorm))
            {
                TempData["error"] = "Ungültiger Gruppenname.";
                return RedirectToPage();
            }

            try
            {
                // Dozent: only their groups
                if (User.IsInRole("Dozent") && !User.IsInRole("SuperAdmin"))
                {
                    var owned = await GetOwnedGroupsAsync();
                    if (owned.Count == 0 || !owned.Contains(groupNorm)) return Forbid();
                }
                var rec = await _db.GroupFeatureSettings
                                   .FirstOrDefaultAsync(x => x.Group != null && x.Group.Trim() == groupNorm);
                if (rec == null)
                {
                    rec = new GroupFeatureSetting { Group = groupNorm };
                    _db.GroupFeatureSettings.Add(rec);
                }
                rec.EnableFilterGeneration = enableFilterGeneration;
                rec.EnableSmartSelection = enableSmartSelection;
                rec.UpdatedAt = DateTime.UtcNow;
                rec.UpdatedByUserId = _userMgr.GetUserId(User);
                await _db.SaveChangesAsync();
                TempData["success"] = $"Feature-Flags für Gruppe '{groupNorm}' gespeichert.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Speichern der Gruppen-Flags fehlgeschlagen: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRegisterAdminAsync()
        {
            if (!User.IsInRole("SuperAdmin")) return Forbid();
            if (await _userMgr.FindByEmailAsync(NewAdminEmailRegister) != null)
            {
                TempData["error"] = "Benutzer existiert bereits.";
                return RedirectToPage();
            }

            var user = new AplicationUser
            {
                UserName = NewAdminEmailRegister,
                Email = NewAdminEmailRegister,
                Name = NewAdminName,
                EmailConfirmed = true
            };
            var res = await _userMgr.CreateAsync(user, NewAdminPassword);
            if (!res.Succeeded)
            {
                TempData["error"] = string.Join(", ", res.Errors.Select(e => e.Description));
                return RedirectToPage();
            }

            await _userMgr.AddToRoleAsync(user, "Admin");
            TempData["success"] = "Admin erfolgreich angelegt.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveAdminAsync()
        {
            if (!User.IsInRole("SuperAdmin")) return Forbid();
            var user = await _userMgr.FindByEmailAsync(RemoveAdminEmail);
            if (user == null)
            {
                TempData["error"] = "Benutzer nicht gefunden.";
                return RedirectToPage();
            }

            if (RemoveAdminEmail.Equals(User.Identity!.Name,
                StringComparison.OrdinalIgnoreCase))
            {
                TempData["error"] = "Sie können sich nicht selbst löschen.";
                return RedirectToPage();
            }

            if (await TotalAdminsAsync() <= 1)
            {
                TempData["error"] = "Mindestens ein Admin muss bleiben.";
                return RedirectToPage();
            }

            var del = await _userMgr.DeleteAsync(user);
            TempData[del.Succeeded ? "success" : "error"] =
                del.Succeeded ? "Administrator gelöscht."
                              : "Fehler: " + string.Join(", ", del.Errors.Select(e => e.Description));
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync()
        {
            if (!User.IsInRole("SuperAdmin")) return Forbid();
            if (string.IsNullOrWhiteSpace(UpdateStatusEmail) ||
                string.IsNullOrWhiteSpace(UpdateStatusRole))
            {
                TempData["error"] = "Ungültige Eingabe.";
                return RedirectToPage();
            }

            var user = await _userMgr.FindByEmailAsync(UpdateStatusEmail);
            if (user == null)
            {
                TempData["error"] = "Benutzer nicht gefunden.";
                return RedirectToPage();
            }

            // Only block when the very last admin would be demoted to a regular user.
            if (await TotalAdminsAsync() == 1 &&
                (await _userMgr.IsInRoleAsync(user, "Admin") || await _userMgr.IsInRoleAsync(user, "SuperAdmin")) &&
                string.Equals(UpdateStatusRole, "User", StringComparison.OrdinalIgnoreCase))
            {
                TempData["error"] = "Mindestens ein Admin muss bleiben.";
                return RedirectToPage();
            }

            if (UpdateStatusRole == "SuperAdmin")
            {
                if (!await _userMgr.IsInRoleAsync(user, "Admin"))
                    await _userMgr.AddToRoleAsync(user, "Admin");
                if (!await _userMgr.IsInRoleAsync(user, "SuperAdmin"))
                    await _userMgr.AddToRoleAsync(user, "SuperAdmin");
                if (await _userMgr.IsInRoleAsync(user, "Dozent"))
                    await _userMgr.RemoveFromRoleAsync(user, "Dozent");
            }
            else if (UpdateStatusRole == "Admin")
            {
                if (await _userMgr.IsInRoleAsync(user, "SuperAdmin"))
                    await _userMgr.RemoveFromRoleAsync(user, "SuperAdmin");
                if (!await _userMgr.IsInRoleAsync(user, "Admin"))
                    await _userMgr.AddToRoleAsync(user, "Admin");
                // keep Admin only; remove Dozent if present
                if (await _userMgr.IsInRoleAsync(user, "Dozent"))
                    await _userMgr.RemoveFromRoleAsync(user, "Dozent");
            }
            else if (UpdateStatusRole == "Dozent")
            {
                // Dozent should NOT be Admin. Remove SuperAdmin/Admin (guard last Admin), then ensure Dozent
                if (await _userMgr.IsInRoleAsync(user, "SuperAdmin"))
                    await _userMgr.RemoveFromRoleAsync(user, "SuperAdmin");

                if (await _userMgr.IsInRoleAsync(user, "Admin"))
                {
                    if (await TotalAdminsAsync() <= 1)
                    {
                        TempData["error"] = "Mindestens ein Admin muss bleiben.";
                        return RedirectToPage();
                    }
                    await _userMgr.RemoveFromRoleAsync(user, "Admin");
                }

                // Option A: ensure pure Dozent (no Coach privileges)
                if (await _userMgr.IsInRoleAsync(user, "Coach"))
                    await _userMgr.RemoveFromRoleAsync(user, "Coach");

                if (!await _userMgr.IsInRoleAsync(user, "Dozent"))
                    await _userMgr.AddToRoleAsync(user, "Dozent");
            }
            else if (UpdateStatusRole == "ApiManager")
            {
                // Make user exclusively an ApiManager (no Admin/SuperAdmin/Dozent)
                if (await _userMgr.IsInRoleAsync(user, "SuperAdmin"))
                    await _userMgr.RemoveFromRoleAsync(user, "SuperAdmin");

                if (await _userMgr.IsInRoleAsync(user, "Admin"))
                {
                    if (await TotalAdminsAsync() <= 1)
                    {
                        TempData["error"] = "Mindestens ein Admin muss bleiben.";
                        return RedirectToPage();
                    }
                    await _userMgr.RemoveFromRoleAsync(user, "Admin");
                }

                if (await _userMgr.IsInRoleAsync(user, "Dozent"))
                    await _userMgr.RemoveFromRoleAsync(user, "Dozent");

                if (!await _userMgr.IsInRoleAsync(user, "ApiManager"))
                    await _userMgr.AddToRoleAsync(user, "ApiManager");
            }
            else // User
            {
                // Entferne alle Admin-Rollen
                if (await _userMgr.IsInRoleAsync(user, "SuperAdmin"))
                    await _userMgr.RemoveFromRoleAsync(user, "SuperAdmin");
                if (await _userMgr.IsInRoleAsync(user, "Admin"))
                    await _userMgr.RemoveFromRoleAsync(user, "Admin");
                if (await _userMgr.IsInRoleAsync(user, "Dozent"))
                    await _userMgr.RemoveFromRoleAsync(user, "Dozent");
                if (await _userMgr.IsInRoleAsync(user, "ApiManager"))
                    await _userMgr.RemoveFromRoleAsync(user, "ApiManager");
            }

            await _userMgr.UpdateSecurityStampAsync(user);
            if (UpdateStatusEmail.Equals(User.Identity!.Name,
                StringComparison.OrdinalIgnoreCase))
            {
                await _signInMgr.RefreshSignInAsync(user);
            }

            TempData["success"] = "Rolle wurde aktualisiert.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateStatusWithGroupAsync()
        {
            if (!User.IsInRole("SuperAdmin")) return Forbid();

            if (string.IsNullOrWhiteSpace(UpdateStatusEmail) ||
                string.IsNullOrWhiteSpace(UpdateStatusRole))
            {
                TempData["error"] = "Bitte Rolle auswählen.";
                return RedirectToPage();
            }

            // Parse composite role value (e.g., "Dozent|Forscher" -> role "Dozent", display "Forscher")
            var roleParts = UpdateStatusRole.Split('|');
            var actualRole = roleParts[0];
            var displayName = roleParts.Length > 1 ? roleParts[1] : "";

            // Check if this is a User role (Student/Benutzer) - group is required only for these
            var isUserRole = actualRole.Equals("User", StringComparison.OrdinalIgnoreCase);

            var groupNorm = (UpdateStatusGroup ?? string.Empty).Trim();
            var hasValidGroup = !string.IsNullOrWhiteSpace(groupNorm) &&
                                !string.Equals(groupNorm, "Ohne Gruppe", StringComparison.OrdinalIgnoreCase);

            // For User roles, group is required
            if (isUserRole && !hasValidGroup)
            {
                TempData["error"] = "Bitte eine Gruppe für Student*in/Benutzer*in auswählen.";
                return RedirectToPage();
            }

            var user = await _userMgr.FindByEmailAsync(UpdateStatusEmail);
            if (user == null)
            {
                TempData["error"] = "Benutzer nicht gefunden.";
                return RedirectToPage();
            }

            // Store display name as claim (for alias distinction like Forscher/Dozent/GruppenManager)
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                var existingClaims = await _userMgr.GetClaimsAsync(user);
                var existingDisplayClaim = existingClaims.FirstOrDefault(c => c.Type == "RoleDisplayName");
                if (existingDisplayClaim != null)
                {
                    await _userMgr.RemoveClaimAsync(user, existingDisplayClaim);
                }
                await _userMgr.AddClaimAsync(user, new System.Security.Claims.Claim("RoleDisplayName", displayName));
            }

            // Only update group membership if a valid group was selected
            if (hasValidGroup)
            {
                try
                {
                    var existing = await _db.UserGroupMemberships
                        .Where(m => m.UserId == user.Id)
                        .OrderByDescending(m => m.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (existing == null)
                    {
                        _db.UserGroupMemberships.Add(new UserGroupMembership
                        {
                            UserId = user.Id,
                            Group = groupNorm,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        existing.Group = groupNorm;
                        existing.CreatedAt = DateTime.UtcNow;
                        _db.UserGroupMemberships.Update(existing);
                    }
                    await _db.SaveChangesAsync();
                }
                catch { }
            }

            // Update the role value to actual role for the existing handler
            UpdateStatusRole = actualRole;

            // Delegate role change logic to existing handler
            return await OnPostUpdateStatusAsync();
        }

        public async Task<IActionResult> OnPostRemoveUserAsync()
        {
            var isSuper = User.IsInRole("SuperAdmin");
            var isDozent = User.IsInRole("Dozent");
            if (!isSuper && !isDozent) return Forbid();
            if (string.IsNullOrWhiteSpace(RemoveUserEmail))
            {
                TempData["error"] = "Ungültige Eingabe.";
                return RedirectToPage();
            }

            var user = await _userMgr.FindByEmailAsync(RemoveUserEmail);
            if (user == null)
            {
                TempData["error"] = "Benutzer nicht gefunden.";
                return RedirectToPage();
            }

            // Dozent: may delete only regular users within owned groups
            if (!isSuper)
            {
                if (await _userMgr.IsInRoleAsync(user, "Admin") ||
                    await _userMgr.IsInRoleAsync(user, "SuperAdmin") ||
                    await _userMgr.IsInRoleAsync(user, "Dozent"))
                {
                    return Forbid();
                }
                // Resolve user's group
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
                if (owned.Count == 0 || !owned.Contains(key))
                    return Forbid();
            }

            var del = await _userMgr.DeleteAsync(user);
            if (del.Succeeded)
                TempData["success"] = $"Benutzer {RemoveUserEmail} gelöscht.";
            else
                TempData["error"] = "Fehler: " + string.Join(", ", del.Errors.Select(e => e.Description));

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostDeleteAllUsersAsync()
        {
            if (!User.IsInRole("SuperAdmin")) return Forbid();
            // lösche alle Nicht-Admin-User
            var toDelete = (await _userMgr.GetUsersInRoleAsync("Admin"))
                           .Concat(await _userMgr.GetUsersInRoleAsync("SuperAdmin"));
            var all = _userMgr.Users.ToList();
            var normals = all.Except(toDelete, new IdentityUserComparer()).ToList();

            foreach (var u in normals)
                await _userMgr.DeleteAsync(u);

            TempData["success"] = "Alle regulären Benutzer wurden gelöscht.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteGroupUsersAsync(string groupName)
        {
            if (!User.IsInRole("SuperAdmin")) return Forbid();
            if (groupName == null)
            {
                TempData["error"] = "Ungültiger Gruppenname.";
                return RedirectToPage();
            }

            // Build group lookup (latest membership per user). If table not present, act as empty
            Dictionary<string, string?> groupLookup;
            try
            {
                groupLookup = await _db.UserGroupMemberships
                    .GroupBy(m => m.UserId)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        Group = g.OrderByDescending(x => x.CreatedAt)
                                 .Select(x => x.Group)
                                 .FirstOrDefault()
                    })
                    .ToDictionaryAsync(x => x.UserId, x => x.Group);
            }
            catch
            {
                groupLookup = new Dictionary<string, string?>();
            }

            // Determine target users for this group
            bool deleteNoGroup = string.Equals(groupName, "Ohne Gruppe", StringComparison.OrdinalIgnoreCase);
            var adminSet = new HashSet<string>((await _userMgr.GetUsersInRoleAsync("Admin")).Select(u => u.Id));
            var superSet = new HashSet<string>((await _userMgr.GetUsersInRoleAsync("SuperAdmin")).Select(u => u.Id));
            var dozentSet = new HashSet<string>((await _userMgr.GetUsersInRoleAsync("Dozent")).Select(u => u.Id));

            var allUsers = _userMgr.Users.ToList();

            var targets = allUsers.Where(u =>
            {
                if (adminSet.Contains(u.Id) || superSet.Contains(u.Id) || dozentSet.Contains(u.Id)) return false; // never delete admins
                if (!groupLookup.TryGetValue(u.Id, out var grp))
                    return deleteNoGroup; // user without membership belongs to "Ohne Gruppe"
                var hasGroup = !string.IsNullOrWhiteSpace(grp);
                if (deleteNoGroup) return !hasGroup;
                return string.Equals(grp, groupName, StringComparison.OrdinalIgnoreCase);
            }).ToList();

            if (targets.Count == 0)
            {
                TempData["success"] = $"Keine Benutzer in Gruppe '{groupName}' zu löschen.";
                return RedirectToPage();
            }

            // Remove memberships for those users to keep DB tidy
            try
            {
                var ids = targets.Select(t => t.Id).ToList();
                var memberships = _db.UserGroupMemberships.Where(m => ids.Contains(m.UserId));
                _db.UserGroupMemberships.RemoveRange(memberships);
                await _db.SaveChangesAsync();
            }
            catch { /* ignore if table not present */ }

            // Delete the users
            int deleted = 0;
            foreach (var u in targets)
            {
                var res = await _userMgr.DeleteAsync(u);
                if (res.Succeeded) deleted++;
            }

            TempData["success"] = $"{deleted} Benutzer aus Gruppe '{groupName}' gelöscht.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddOwnerAsync(string groupName, string ownerEmail)
        {
            if (!(User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))) return Forbid();
            var groupNorm = (groupName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupNorm)) { TempData["error"] = "Ungültiger Gruppenname."; return RedirectToPage(); }
            if (string.IsNullOrWhiteSpace(ownerEmail)) { TempData["error"] = "E-Mail erforderlich."; return RedirectToPage(); }

            var user = await _userMgr.FindByEmailAsync(ownerEmail);
            if (user == null) { TempData["error"] = "Dozent nicht gefunden."; return RedirectToPage(); }
            if (!await _userMgr.IsInRoleAsync(user, "Dozent")) { TempData["error"] = "Nur Dozenten können Besitzer sein."; return RedirectToPage(); }

            var exists = await _db.DozentGroupOwnerships
                .AnyAsync(o => o.Group == groupNorm && o.DozentUserId == user.Id);
            if (!exists)
            {
                _db.DozentGroupOwnerships.Add(new DozentGroupOwnership
                {
                    Group = groupNorm,
                    DozentUserId = user.Id,
                    CreatedAt = DateTime.UtcNow
                });
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
            if (!(User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))) return Forbid();
            var groupNorm = (groupName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupNorm) || string.IsNullOrWhiteSpace(ownerId))
            { TempData["error"] = "Ungültige Eingabe."; return RedirectToPage(); }

            var rec = await _db.DozentGroupOwnerships
                .FirstOrDefaultAsync(o => o.Group == groupNorm && o.DozentUserId == ownerId);
            if (rec == null)
            {
                TempData["error"] = "Besitzerzuordnung nicht gefunden.";
                return RedirectToPage();
            }
            _db.DozentGroupOwnerships.Remove(rec);
            await _db.SaveChangesAsync();
            TempData["success"] = "Besitzer entfernt.";
            return RedirectToPage();
        }

        // Hilfsklasse zum Vergleichen
        private class IdentityUserComparer : IEqualityComparer<IdentityUser>
        {
            public bool Equals(IdentityUser x, IdentityUser y)
                => x?.Id == y?.Id;
            public int GetHashCode(IdentityUser obj) => obj.Id.GetHashCode();
        }
    }
}
