using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using ProjectsWebApp.Areas.Admin.Models;
using ProjectsWebApp.DataAccsess.Services.Interfaces;

namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserActivityAnalyticsService _analytics;

        public GroupsController(ApplicationDbContext db, UserManager<IdentityUser> userManager, IUserActivityAnalyticsService analytics)
        {
            _db = db;
            _userManager = userManager;
            _analytics = analytics;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 12)
        {
            var query = _db.Groups.AsQueryable();

            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            var isDozent = User.IsInRole("Dozent") && !User.IsInRole("SuperAdmin");

            if (isDozent && !isAdmin)
            {
                var owned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                try
                {
                    var ownedList = await _db.DozentGroupOwnerships
                        .Where(o => o.DozentUserId == userId)
                        .Select(o => o.Group)
                        .Where(g => g != null && g != "")
                        .Select(g => g!.Trim())
                        .ToListAsync();
                    owned = new HashSet<string>(ownedList, StringComparer.OrdinalIgnoreCase);
                }
                catch { }

                var memberGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                try
                {
                    var memberList = await _db.UserGroupMemberships
                        .Where(m => m.UserId == userId && m.Group != null && m.Group != "")
                        .Select(m => m.Group!.Trim())
                        .ToListAsync();
                    memberGroups = new HashSet<string>(memberList, StringComparer.OrdinalIgnoreCase);
                }
                catch { }

                var allowedNames = owned.Union(memberGroups).ToList();
                ViewBag.OwnedGroupNames = owned;
                if (allowedNames.Count == 0)
                {
                    return View(new List<Group>());
                }

                query = query.Where(g => allowedNames.Contains(g.Name));
            }
            else if (!isAdmin && !User.IsInRole("Dozent"))
            {
                // Normale Benutzer: nur Gruppen zeigen, in denen sie Mitglied sind
                try
                {
                    var memberNames = await _db.UserGroupMemberships
                        .Where(m => m.UserId == userId && m.Group != null && m.Group != "")
                        .Select(m => m.Group!.Trim())
                        .ToListAsync();
                    var memberSet = new HashSet<string>(memberNames, StringComparer.OrdinalIgnoreCase);

                    if (memberSet.Count == 0)
                    {
                        return View(new List<Group>());
                    }

                    query = query.Where(g => memberSet.Contains(g.Name));
                }
                catch
                {
                    return View(new List<Group>());
                }
            }

            if (pageSize <= 0) pageSize = 12;

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (page < 1) page = 1;
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var groups = await query
                .OrderBy(g => g.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;

            return View(groups);
        }

        [HttpGet]
        public async Task<IActionResult> JoinGroup()
        {
            var vm = new JoinGroupAdminViewModel();
            await LoadMemberGroupsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinGroup(JoinGroupAdminViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await LoadMemberGroupsAsync(vm);
                return View(vm);
            }

            try
            {
                var code = (vm.InviteCode ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(code))
                {
                    TempData["error"] = "Bitte geben Sie einen Einladungscode ein.";
                    await LoadMemberGroupsAsync(vm);
                    return View(vm);
                }

                var codeEntry = await _db.RegistrationCodes
                    .FirstOrDefaultAsync(c => c.Code == code && c.IsActive);
                if (codeEntry == null)
                {
                    TempData["error"] = "Der Einladungscode ist ungültig.";
                    await LoadMemberGroupsAsync(vm);
                    return View(vm);
                }

                var groupName = codeEntry.Group?.Trim();
                if (string.IsNullOrWhiteSpace(groupName))
                {
                    TempData["error"] = "Dieser Einladungscode ist keiner Gruppe zugeordnet.";
                    await LoadMemberGroupsAsync(vm);
                    return View(vm);
                }

                // Ensure Groups table row exists
                try
                {
                    var grp = await _db.Groups.FirstOrDefaultAsync(g => g.Name == groupName);
                    if (grp == null)
                    {
                        _db.Groups.Add(new Group { Name = groupName });
                        await _db.SaveChangesAsync();
                    }
                }
                catch { }

                // Add membership if not yet present
                var uid = _userManager.GetUserId(User);
                bool exists = false;
                try
                {
                    exists = await _db.UserGroupMemberships
                        .AnyAsync(m => m.UserId == uid && m.Group != null && m.Group.Trim() == groupName);
                }
                catch { exists = false; }

                if (!exists)
                {
                    // Do not switch the user's 'current' group implicitly: keep latest membership unchanged
                    DateTime createdAt = DateTime.UtcNow;
                    try
                    {
                        var latest = await _db.UserGroupMemberships
                            .Where(m => m.UserId == uid)
                            .OrderByDescending(m => m.CreatedAt)
                            .FirstOrDefaultAsync();
                        if (latest != null)
                        {
                            // set slightly older than current latest so this new one won't become the latest
                            createdAt = latest.CreatedAt.AddTicks(-1);
                        }
                    }
                    catch { createdAt = DateTime.UtcNow; }

                    _db.UserGroupMemberships.Add(new UserGroupMembership
                    {
                        UserId = uid!,
                        Group = groupName,
                        CreatedAt = createdAt
                    });
                    await _db.SaveChangesAsync();
                }

                TempData["success"] = $"Die Gruppe '{groupName}' wurde Ihrem Konto hinzugefügt.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["error"] = "Unerwarteter Fehler beim Beitritt. Bitte versuchen Sie es erneut.";
                await LoadMemberGroupsAsync(vm);
                return View(vm);
            }
        }

        // GET: /Admin/Groups/Create
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create()
        {
            var vm = new GroupEditViewModel();
            await PopulateOwnerOptionsAsync(vm, preselectCurrentUser: true);
            return View(vm);
        }

        // POST: /Admin/Groups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create(GroupEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateOwnerOptionsAsync(vm, preselectCurrentUser: false);
                return View(vm);
            }

            var nameTrimmed = (vm.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(nameTrimmed))
            {
                ModelState.AddModelError(nameof(vm.Name), "Name der Gruppe ist erforderlich.");
                await PopulateOwnerOptionsAsync(vm, preselectCurrentUser: false);
                return View(vm);
            }

            bool exists = await _db.Groups.AnyAsync(g => g.Name == nameTrimmed);
            if (exists)
            {
                ModelState.AddModelError(nameof(vm.Name), "Es existiert bereits eine Gruppe mit diesem Namen.");
                await PopulateOwnerOptionsAsync(vm, preselectCurrentUser: false);
                return View(vm);
            }

            var group = new Group
            {
                Name = nameTrimmed,
                Description = vm.Description?.Trim(),
                CreatedAt = DateTime.UtcNow
            };
            _db.Groups.Add(group);
            await _db.SaveChangesAsync();

            // Owners
            var ownerIds = (vm.SelectedOwnerIds ?? new List<string>())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var now = DateTime.UtcNow;
            foreach (var ownerId in ownerIds)
            {
                _db.DozentGroupOwnerships.Add(new DozentGroupOwnership
                {
                    DozentUserId = ownerId,
                    Group = nameTrimmed,
                    CreatedAt = now
                });

                // ensure membership as well
                bool alreadyMember = await _db.UserGroupMemberships
                    .AnyAsync(m => m.UserId == ownerId && m.Group == nameTrimmed);
                if (!alreadyMember)
                {
                    _db.UserGroupMemberships.Add(new UserGroupMembership
                    {
                        UserId = ownerId,
                        Group = nameTrimmed,
                        CreatedAt = now
                    });
                }
            }

            // Auto-create a registration code for this group
            var code = new RegistrationCode
            {
                Code = GenerateInviteCode(),
                IsActive = true,
                Group = nameTrimmed,
                Note = "Automatisch beim Anlegen der Gruppe erzeugt"
            };
            _db.RegistrationCodes.Add(code);

            await _db.SaveChangesAsync();

            TempData["success"] = "Gruppe wurde angelegt.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Groups/Edit/{id}
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        public async Task<IActionResult> Edit(int id)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group == null) return NotFound();

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            var isDozentOnly = User.IsInRole("Dozent") && !isAdmin;

            if (isDozentOnly)
            {
                var uid = _userManager.GetUserId(User);
                bool isOwner = await _db.DozentGroupOwnerships
                    .AnyAsync(o => o.Group == group.Name && o.DozentUserId == uid);
                if (!isOwner) return Forbid();
            }

            var vm = new GroupEditViewModel
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description
            };

            var groupName = group.Name;
            if (!string.IsNullOrWhiteSpace(groupName))
            {
                await LoadExistingOwnersAsync(vm, groupName);
            }

            if (isAdmin)
            {
                await PopulateOwnerOptionsAsync(vm, preselectCurrentUser: false);
            }
            return View(vm);
        }

        // POST: /Admin/Groups/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        public async Task<IActionResult> Edit(int id, GroupEditViewModel vm)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group == null) return NotFound();

            // Name is treated as immutable in this phase
            vm.Name = group.Name;

            var groupName = group.Name;
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            var isDozentOnly = User.IsInRole("Dozent") && !isAdmin;

            if (isDozentOnly)
            {
                var uid = _userManager.GetUserId(User);
                bool isOwner = await _db.DozentGroupOwnerships
                    .AnyAsync(o => o.Group == groupName && o.DozentUserId == uid);
                if (!isOwner) return Forbid();
            }

            if (!ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(groupName))
                {
                    await LoadExistingOwnersAsync(vm, groupName);
                }
                if (isAdmin)
                {
                    await PopulateOwnerOptionsAsync(vm, preselectCurrentUser: false);
                }
                return View(vm);
            }

            group.Description = vm.Description?.Trim();

            if (isAdmin)
            {
                // Admins/SuperAdmins can fully manage the owner list via checkboxes
                var currentOwners = await _db.DozentGroupOwnerships
                    .Where(o => o.Group == groupName)
                    .ToListAsync();
                _db.DozentGroupOwnerships.RemoveRange(currentOwners);

                var ownerIds = (vm.SelectedOwnerIds ?? new List<string>())
                    .Where(id2 => !string.IsNullOrWhiteSpace(id2))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var now = DateTime.UtcNow;
                foreach (var ownerId in ownerIds)
                {
                    _db.DozentGroupOwnerships.Add(new DozentGroupOwnership
                    {
                        DozentUserId = ownerId,
                        Group = groupName,
                        CreatedAt = now
                    });

                    bool alreadyMember = await _db.UserGroupMemberships
                        .AnyAsync(m => m.UserId == ownerId && m.Group == groupName);
                    if (!alreadyMember)
                    {
                        _db.UserGroupMemberships.Add(new UserGroupMembership
                        {
                            UserId = ownerId,
                            Group = groupName,
                            CreatedAt = now
                        });
                    }
                }
            }
            else if (isDozentOnly)
            {
                // Dozenten dürfen aus Datenschutzgründen keine globale Liste sehen,
                // aber zusätzliche Besitzer:innen per E-Mail hinzufügen
                if (!string.IsNullOrWhiteSpace(vm.AddOwnerEmail))
                {
                    var email = vm.AddOwnerEmail.Trim();
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        ModelState.AddModelError(nameof(vm.AddOwnerEmail), "Kein Benutzer mit dieser E-Mail gefunden.");
                    }
                    else
                    {
                        var targetIsAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                        var targetIsSuper = await _userManager.IsInRoleAsync(user, "SuperAdmin");
                        var targetIsDozent = await _userManager.IsInRoleAsync(user, "Dozent");
                        var targetIsCoach = await _userManager.IsInRoleAsync(user, "Coach");
                        var targetIsApiManager = await _userManager.IsInRoleAsync(user, "ApiManager");

                        if (!(targetIsAdmin || targetIsSuper || targetIsDozent || targetIsCoach || targetIsApiManager))
                        {
                            ModelState.AddModelError(nameof(vm.AddOwnerEmail), "Nur Admins, Coaches, Dozenten oder API‑Manager können Besitzer:innen sein.");
                        }
                        else
                        {
                            bool alreadyOwner = await _db.DozentGroupOwnerships
                                .AnyAsync(o => o.Group == groupName && o.DozentUserId == user.Id);
                            if (alreadyOwner)
                            {
                                ModelState.AddModelError(nameof(vm.AddOwnerEmail), "Diese Person ist bereits Besitzer:in der Gruppe.");
                            }
                            else
                            {
                                var now = DateTime.UtcNow;
                                _db.DozentGroupOwnerships.Add(new DozentGroupOwnership
                                {
                                    DozentUserId = user.Id,
                                    Group = groupName,
                                    CreatedAt = now
                                });

                                bool alreadyMember = await _db.UserGroupMemberships
                                    .AnyAsync(m => m.UserId == user.Id && m.Group == groupName);
                                if (!alreadyMember)
                                {
                                    _db.UserGroupMemberships.Add(new UserGroupMembership
                                    {
                                        UserId = user.Id,
                                        Group = groupName,
                                        CreatedAt = now
                                    });
                                }
                            }
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        await LoadExistingOwnersAsync(vm, groupName);
                        return View(vm);
                    }
                }
            }

            await _db.SaveChangesAsync();

            TempData["success"] = "Gruppe wurde aktualisiert.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Groups/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group == null) return NotFound();

            var groupName = (group.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupName)) return NotFound();

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            var isDozentOnly = User.IsInRole("Dozent") && !isAdmin;

            if (isDozentOnly)
            {
                // Reuse visibility rules from Index: only groups the dozent owns or is member of
                var userId = _userManager.GetUserId(User);
                var ownedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var memberNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                try
                {
                    var ownedList = await _db.DozentGroupOwnerships
                        .Where(o => o.DozentUserId == userId)
                        .Select(o => o.Group)
                        .ToListAsync();
                    foreach (var g in ownedList.Where(s => !string.IsNullOrWhiteSpace(s)))
                        ownedNames.Add(g!.Trim());

                    var memberList = await _db.UserGroupMemberships
                        .Where(m => m.UserId == userId && m.Group != null && m.Group != "")
                        .Select(m => m.Group!)
                        .ToListAsync();
                    foreach (var g in memberList.Where(s => !string.IsNullOrWhiteSpace(s)))
                        memberNames.Add(g.Trim());
                }
                catch { }

                if (!ownedNames.Contains(groupName) && !memberNames.Contains(groupName))
                    return Forbid();
            }

            // Normale Benutzer: nur Gruppen einsehen, in denen sie Mitglied sind
            if (!isAdmin && !User.IsInRole("Dozent"))
            {
                var uid = _userManager.GetUserId(User);
                bool isMember = await _db.UserGroupMemberships
                    .AnyAsync(m => m.UserId == uid && m.Group != null && m.Group.Trim() == groupName);
                if (!isMember) return Forbid();
            }

            var vm = new GroupDetailsViewModel
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                CreatedAt = group.CreatedAt
            };

            // Owners
            try
            {
                var owners = await _db.DozentGroupOwnerships
                    .Where(o => o.Group == groupName)
                    .ToListAsync();
                var ownerIds = owners.Select(o => o.DozentUserId).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                if (ownerIds.Count > 0)
                {
                    var users = _userManager.Users
                        .Where(u => ownerIds.Contains(u.Id))
                        .ToList();

                    bool hasApiManagerOwner = false;
                    foreach (var u in users)
                    {
                        vm.Owners.Add(u.Email ?? u.UserName ?? u.Id);

                        if (!hasApiManagerOwner && await _userManager.IsInRoleAsync(u, "ApiManager"))
                        {
                            hasApiManagerOwner = true;
                        }
                    }

                    vm.HasApiManagerOwner = hasApiManagerOwner;
                }
            }
            catch { }

            // Deletion permission: Admin/SuperAdmin can always delete; Dozent only if owner
            try
            {
                bool canDelete = false;

                if (isAdmin)
                {
                    canDelete = true;
                }
                else if (User.IsInRole("Dozent"))
                {
                    var uid = _userManager.GetUserId(User);
                    if (!string.IsNullOrWhiteSpace(uid))
                    {
                        try
                        {
                            canDelete = await _db.DozentGroupOwnerships
                                .AnyAsync(o => o.Group == groupName && o.DozentUserId == uid);
                        }
                        catch { canDelete = false; }
                    }
                }

                vm.CanDeleteGroup = canDelete;
            }
            catch { vm.CanDeleteGroup = false; }

            // Members and role counts
            try
            {
                var memberships = await _db.UserGroupMemberships
                    .Where(m => m.Group != null && m.Group.Trim() == groupName)
                    .ToListAsync();
                var memberIds = memberships
                    .Select(m => m.UserId)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (memberIds.Count > 0)
                {
                    // Load all memberships for these users (across all groups) and
                    // compute distinct group counts in memory to avoid unsupported
                    // LINQ translations in EF.
                    var allMembershipsForMembers = await _db.UserGroupMemberships
                        .Where(m => memberIds.Contains(m.UserId) && m.Group != null && m.Group != "")
                        .ToListAsync();

                    var groupCountsByUserId = allMembershipsForMembers
                        .GroupBy(m => m.UserId, StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(x => (x.Group ?? string.Empty).Trim())
                                  .Where(s => !string.IsNullOrWhiteSpace(s))
                                  .Distinct(StringComparer.OrdinalIgnoreCase)
                                  .Count(),
                            StringComparer.OrdinalIgnoreCase);

                    // Load per-user activity timestamps
                    var activities = new List<UserActivity>();
                    try
                    {
                        activities = await _db.UserActivities
                            .Where(a => memberIds.Contains(a.UserId))
                            .ToListAsync();
                    }
                    catch { }

                    var activityByUserId = activities
                        .GroupBy(a => a.UserId, StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.LastLoginAt ?? x.CreatedAt).First(), StringComparer.OrdinalIgnoreCase);

                    // Load optional AplicationUser rows for names & role aliases
                    var appUserById = new Dictionary<string, AplicationUser>(StringComparer.OrdinalIgnoreCase);
                    try
                    {
                        var appRows = await _db.AplicationUser
                            .Where(a => memberIds.Contains(a.Id))
                            .ToListAsync();
                        foreach (var a in appRows)
                        {
                            appUserById[a.Id] = a;
                        }
                    }
                    catch { }

                    vm.RoleAliasesByUserId.Clear();
                    foreach (var kv in appUserById)
                    {
                        if (!string.IsNullOrWhiteSpace(kv.Value.RoleAlias))
                        {
                            vm.RoleAliasesByUserId[kv.Key] = kv.Value.RoleAlias.Trim();
                        }
                    }

                    var users = _userManager.Users
                        .Where(u => memberIds.Contains(u.Id))
                        .ToList();
                    foreach (var u in users)
                    {
                        var roles = await _userManager.GetRolesAsync(u);
                        var rolesStr = string.Join(", ", roles);

                        activityByUserId.TryGetValue(u.Id, out var ua);
                        appUserById.TryGetValue(u.Id, out var appUser);
                        groupCountsByUserId.TryGetValue(u.Id, out var groupCount);

                        vm.Members.Add(new GroupMemberRow
                        {
                            UserId = u.Id,
                            Email = u.Email ?? u.UserName ?? u.Id,
                            Name = appUser?.Name,
                            Roles = rolesStr,
                            GroupCount = groupCount,
                            CreatedAt = ua?.CreatedAt,
                            LastLoginAt = ua?.LastLoginAt
                        });
                    }

                    vm.MemberCount = vm.Members.Count;
                    vm.AdminCount = vm.Members.Count(m => m.Roles.Contains("Admin"));
                    vm.DozentCount = vm.Members.Count(m => m.Roles.Contains("Dozent"));
                    vm.RegularUserCount = vm.MemberCount - vm.AdminCount - vm.DozentCount;

                    // Sort members so that Admin/SuperAdmin come first, then ApiManager/Dozent,
                    // then regular users, each block ordered by email.
                    vm.Members = vm.Members
                        .OrderBy(m => GetRoleRankForMember(m.Roles))
                        .ThenBy(m => m.Email, StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }
            }
            catch { }

            // Prompt, assistant and invite code counts
            try
            {
                vm.PromptCount = await _db.PromptTemplateGroups
                    .CountAsync(pg => pg.Group != null && pg.Group.Trim() == groupName);
            }
            catch { vm.PromptCount = 0; }

            try
            {
                vm.AssistantCount = await _db.AssistantGroups
                    .CountAsync(ag => ag.Group != null && ag.Group.Trim() == groupName);
            }
            catch { vm.AssistantCount = 0; }

            try
            {
                var codes = await _db.RegistrationCodes
                    .Where(rc => rc.Group != null && rc.Group.Trim() == groupName)
                    .OrderBy(rc => rc.Id)
                    .ToListAsync();

                vm.InviteCodeCount = codes.Count;
                vm.InviteCodes = codes
                    .Select(rc => new GroupInviteCodeRow
                    {
                        Id = rc.Id,
                        Code = rc.Code,
                        IsActive = rc.IsActive,
                        Note = rc.Note,
                        IsDozentCode = rc.IsDozentCode,
                        DozentBecomesOwner = rc.DozentBecomesOwner
                    })
                    .ToList();
            }
            catch
            {
                vm.InviteCodeCount = 0;
                vm.InviteCodes = new List<GroupInviteCodeRow>();
            }

            // Load core prompt settings for this group (admin dashboard)
            try
            {
                var latestPrompt = await _db.GroupPromptAiSettings
                    .Where(g => g.Group == groupName)
                    .OrderByDescending(g => g.UpdatedAt)
                    .FirstOrDefaultAsync();

                vm.Prompts = new GroupPromptsAdminViewModel
                {
                    GroupId = group.Id,
                    GroupName = groupName,
                    UseGlobal = latestPrompt?.UseGlobal ?? true,
                    SystemPreamble = latestPrompt?.SystemPreamble,
                    UserInstructionText = latestPrompt?.UserInstructionText,
                    KiAssistantSystemPrompt = latestPrompt?.KiAssistantSystemPrompt,
                    FilterSystemPreamble = latestPrompt?.FilterSystemPreamble,
                    FilterFirstLine = latestPrompt?.FilterFirstLine,
                    SmartSelectionSystemPreamble = latestPrompt?.SmartSelectionSystemPreamble,
                    SmartSelectionUserPrompt = latestPrompt?.SmartSelectionUserPrompt
                };
            }
            catch
            {
                vm.Prompts = new GroupPromptsAdminViewModel
                {
                    GroupId = group.Id,
                    GroupName = groupName,
                    UseGlobal = true
                };
            }

            // Load effective per-type guidances (locals overriding globals)
            try
            {
                vm.Prompts.TypeGuidances.Clear();

                var globals = await _db.PromptTypeGuidances
                    .GroupBy(x => x.Type)
                    .Select(g => g.OrderByDescending(x => x.UpdatedAt).First())
                    .ToListAsync();
                var globalDict = globals.ToDictionary(x => x.Type, x => x.GuidanceText ?? string.Empty);

                var locals = await _db.GroupPromptTypeGuidances
                    .Where(x => x.Group == groupName)
                    .GroupBy(x => x.Type)
                    .Select(g => g.OrderByDescending(x => x.UpdatedAt).First())
                    .ToListAsync();
                var localDict = locals.ToDictionary(x => x.Type, x => x.GuidanceText ?? string.Empty);

                vm.Prompts.TypeGuidances = new Dictionary<string, string>();
                foreach (PromptType t in Enum.GetValues(typeof(PromptType)))
                {
                    string text;
                    if (localDict.TryGetValue(t, out var ltxt) && !string.IsNullOrWhiteSpace(ltxt))
                    {
                        text = ltxt;
                    }
                    else if (globalDict.TryGetValue(t, out var gtxt) && !string.IsNullOrWhiteSpace(gtxt))
                    {
                        text = gtxt;
                    }
                    else
                    {
                        text = string.Empty;
                    }

                    vm.Prompts.TypeGuidances[t.ToString()] = text;
                }
            }
            catch
            {
                vm.Prompts.TypeGuidances = new Dictionary<string, string>();
            }

            // Load core API key settings for this group (admin dashboard)
            try
            {
                var latestKeys = await _db.GroupApiKeySettings
                    .Where(a => a.Group == groupName)
                    .OrderByDescending(a => a.UpdatedAt)
                    .FirstOrDefaultAsync();

                vm.ApiKeys = new GroupApiKeysAdminViewModel
                {
                    GroupId = group.Id,
                    GroupName = groupName,
                    ActiveProvider = latestKeys?.ActiveProvider,
                    KisskiApiKey = latestKeys?.KisskiApiKey,
                    KisskiModel = latestKeys?.KisskiModel,
                    OpenAIKey = latestKeys?.OpenAIKey,
                    OpenAIModel = latestKeys?.OpenAIModel,
                    GeminiApiKey = latestKeys?.GeminiApiKey,
                    GeminiModel = latestKeys?.GeminiModel,
                    ClaudeApiKey = latestKeys?.ClaudeApiKey,
                    ClaudeModel = latestKeys?.ClaudeModel
                };
            }
            catch
            {
                vm.ApiKeys = new GroupApiKeysAdminViewModel
                {
                    GroupId = group.Id,
                    GroupName = groupName
                };
            }

            // Analytics summary for the last 30 days (UTC)
            vm.AnalyticsEnabled = ProjectsWebApp.Utility.FeatureFlags.EnableAnalytics;
            try
            {
                var toUtc = DateTime.UtcNow;
                var fromUtc = toUtc.AddDays(-30);

                vm.AnalyticsFromUtc = fromUtc.Date;
                vm.AnalyticsToUtc = toUtc.Date;

                await _analytics.RebuildUserDailyStatsAsync(fromUtc, toUtc);
                var stats = await _analytics.GetGroupDailyStatsAsync(groupName, fromUtc, toUtc);

                if (stats != null && stats.Count > 0)
                {
                    vm.TotalEventsLast30Days = stats.Sum(s => s.TotalEvents);
                    vm.TotalLoginLast30Days = stats.Sum(s => s.LoginCount);
                    vm.TotalPromptGenerateLast30Days = stats.Sum(s => s.PromptGenerateCount);
                    vm.TotalFilterGenerateLast30Days = stats.Sum(s => s.FilterGenerateCount);
                    vm.TotalSmartSelectionLast30Days = stats.Sum(s => s.SmartSelectionCount);
                    vm.TotalAssistantActionsLast30Days = stats.Sum(s => s.AssistantChatCount + s.AssistantChatStreamCount);
                    vm.TotalPromptSaveCollectionLast30Days = stats.Sum(s => s.PromptSaveCollectionCount);
                    vm.TotalPromptPublishLibraryLast30Days = stats.Sum(s => s.PromptPublishLibraryCount);
                    vm.ActiveUsersLast30Days = stats.Select(s => s.UserId).Distinct(StringComparer.OrdinalIgnoreCase).Count();

                    var totalsByUser = stats
                        .GroupBy(s => s.UserId, StringComparer.OrdinalIgnoreCase)
                        .Select(g => new { UserId = g.Key, TotalEvents = g.Sum(x => x.TotalEvents) })
                        .OrderByDescending(x => x.TotalEvents)
                        .Take(5)
                        .ToList();

                    // Only keep users that still exist as group members (deleted accounts are not in vm.Members)
                    var memberIdSet = new HashSet<string>(
                        vm.Members.Select(m => m.UserId),
                        StringComparer.OrdinalIgnoreCase);

                    totalsByUser = totalsByUser
                        .Where(t => memberIdSet.Contains(t.UserId))
                        .ToList();

                    vm.TopUsersLast30Days.Clear();
                    foreach (var row in totalsByUser)
                    {
                        var member = vm.Members.FirstOrDefault(m => string.Equals(m.UserId, row.UserId, StringComparison.OrdinalIgnoreCase));
                        vm.TopUsersLast30Days.Add(new GroupUserAnalyticsRow
                        {
                            UserId = row.UserId,
                            Email = member?.Email ?? row.UserId,
                            TotalEvents = row.TotalEvents
                        });
                    }
                }
            }
            catch { }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        public async Task<IActionResult> RemoveMembership(int id, string uid)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group == null) return NotFound();

            if (string.IsNullOrWhiteSpace(uid)) return BadRequest();

            var groupName = (group.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupName)) return NotFound();

            List<UserGroupMembership> membershipsInGroup;
            try
            {
                membershipsInGroup = await _db.UserGroupMemberships
                    .Where(m => m.UserId == uid && m.Group != null && m.Group.Trim() == groupName)
                    .ToListAsync();
            }
            catch
            {
                membershipsInGroup = new List<UserGroupMembership>();
            }

            if (membershipsInGroup == null || membershipsInGroup.Count == 0)
            {
                TempData["error"] = "Mitgliedschaft konnte nicht gefunden werden.";
                return RedirectToAction(nameof(Details), new { id });
            }

            int totalGroups = 0;
            try
            {
                var allGroups = await _db.UserGroupMemberships
                    .Where(m => m.UserId == uid && m.Group != null && m.Group != "")
                    .Select(m => m.Group!)
                    .ToListAsync();

                totalGroups = allGroups
                    .Select(g => (g ?? string.Empty).Trim())
                    .Where(g => !string.IsNullOrWhiteSpace(g))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Count();
            }
            catch
            {
                totalGroups = 0;
            }

            if (totalGroups <= 1)
            {
                TempData["error"] = "Benutzer kann nicht aus seiner einzigen Gruppe entfernt werden.";
                return RedirectToAction(nameof(Details), new { id });
            }

            _db.UserGroupMemberships.RemoveRange(membershipsInGroup);
            await _db.SaveChangesAsync();

            TempData["success"] = "Benutzer wurde aus dieser Gruppe entfernt.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        public async Task<IActionResult> UserAnalytics(int id, string uid)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group == null) return NotFound();

            var groupName = (group.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupName)) return NotFound();

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            var isDozentOnly = User.IsInRole("Dozent") && !isAdmin;

            if (isDozentOnly)
            {
                var currentUserId = _userManager.GetUserId(User);
                var ownedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var memberNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                try
                {
                    var ownedList = await _db.DozentGroupOwnerships
                        .Where(o => o.DozentUserId == currentUserId)
                        .Select(o => o.Group)
                        .ToListAsync();
                    foreach (var g in ownedList.Where(s => !string.IsNullOrWhiteSpace(s)))
                        ownedNames.Add(g!.Trim());

                    var memberList = await _db.UserGroupMemberships
                        .Where(m => m.UserId == currentUserId && m.Group != null && m.Group != "")
                        .Select(m => m.Group!)
                        .ToListAsync();
                    foreach (var g in memberList.Where(s => !string.IsNullOrWhiteSpace(s)))
                        memberNames.Add(g.Trim());
                }
                catch { }

                if (!ownedNames.Contains(groupName) && !memberNames.Contains(groupName))
                    return Forbid();
            }

            if (string.IsNullOrWhiteSpace(uid)) return BadRequest();

            var isMember = await _db.UserGroupMemberships
                .AnyAsync(m => m.UserId == uid && m.Group != null && m.Group.Trim() == groupName);
            if (!isMember) return NotFound();

            var user = await _userManager.FindByIdAsync(uid);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var rolesStr = string.Join(", ", roles);

            var toUtc = DateTime.UtcNow;
            var fromUtc = toUtc.AddDays(-30);

            try
            {
                await _analytics.RebuildUserDailyStatsAsync(fromUtc, toUtc);
            }
            catch { }

            var stats = await _analytics.GetUserDailyStatsForGroupAsync(uid, groupName, fromUtc, toUtc);

            var vm = new UserAnalyticsViewModel
            {
                GroupId = group.Id,
                GroupName = groupName,
                UserId = uid,
                Email = user.Email ?? user.UserName ?? uid,
                Roles = rolesStr,
                FromUtc = fromUtc.Date,
                ToUtc = toUtc.Date,
                DailyStats = stats.OrderBy(s => s.DateUtc).ToList(),
                AnalyticsEnabled = ProjectsWebApp.Utility.FeatureFlags.EnableAnalytics
            };

            vm.TotalEvents = vm.DailyStats.Sum(s => s.TotalEvents);
            vm.LoginCount = vm.DailyStats.Sum(s => s.LoginCount);
            vm.PromptGenerateCount = vm.DailyStats.Sum(s => s.PromptGenerateCount);
            vm.FilterGenerateCount = vm.DailyStats.Sum(s => s.FilterGenerateCount);
            vm.SmartSelectionCount = vm.DailyStats.Sum(s => s.SmartSelectionCount);
            vm.AssistantChatCount = vm.DailyStats.Sum(s => s.AssistantChatCount + s.AssistantChatStreamCount);
            vm.PromptSaveCollectionCount = vm.DailyStats.Sum(s => s.PromptSaveCollectionCount);
            vm.PromptPublishLibraryCount = vm.DailyStats.Sum(s => s.PromptPublishLibraryCount);
            vm.TotalDurationSeconds = vm.DailyStats.Sum(s => s.TotalDurationSeconds);

            return View(vm);
        }

        private static int GetRoleRankForMember(string roles)
        {
            if (string.IsNullOrWhiteSpace(roles)) return 3;

            if (roles.IndexOf("SuperAdmin", StringComparison.OrdinalIgnoreCase) >= 0)
                return 0;
            if (roles.IndexOf("Admin", StringComparison.OrdinalIgnoreCase) >= 0)
                return 1;
            if (roles.IndexOf("ApiManager", StringComparison.OrdinalIgnoreCase) >= 0 ||
                roles.IndexOf("Dozent", StringComparison.OrdinalIgnoreCase) >= 0)
                return 2;

            return 3;
        }

        private async Task LoadMemberGroupsAsync(JoinGroupAdminViewModel vm)
        {
            vm.MemberGroups.Clear();
            try
            {
                var uid = _userManager.GetUserId(User);
                if (string.IsNullOrWhiteSpace(uid)) return;

                // Get group names user belongs to
                var groupNames = await _db.UserGroupMemberships
                    .Where(m => m.UserId == uid && m.Group != null && m.Group != "")
                    .Select(m => m.Group!.Trim())
                    .Where(g => g != "")
                    .Distinct()
                    .ToListAsync();

                // Get Group entities with IDs
                var groups = await _db.Groups
                    .Where(g => groupNames.Contains(g.Name))
                    .OrderBy(g => g.Name)
                    .Select(g => new MemberGroupItem { Id = g.Id, Name = g.Name })
                    .ToListAsync();

                vm.MemberGroups.AddRange(groups);
            }
            catch { }
        }

        private async Task PopulateOwnerOptionsAsync(GroupEditViewModel vm, bool preselectCurrentUser)
        {
            var currentUserId = _userManager.GetUserId(User);
            var users = _userManager.Users
                .OrderBy(u => u.Email)
                .ToList();

            var selected = new HashSet<string>(vm.SelectedOwnerIds ?? new List<string>(), StringComparer.OrdinalIgnoreCase);

            vm.OwnerOptions.Clear();

            foreach (var u in users)
            {
                // Only allow potential owners that are at least Dozent or Admin/SuperAdmin
                bool isOwnerCandidate = await _userManager.IsInRoleAsync(u, "Dozent")
                                         || await _userManager.IsInRoleAsync(u, "Admin")
                                         || await _userManager.IsInRoleAsync(u, "SuperAdmin")
                                         || await _userManager.IsInRoleAsync(u, "Coach")
                                         || await _userManager.IsInRoleAsync(u, "ApiManager");
                if (!isOwnerCandidate) continue;

                bool isSelected = selected.Contains(u.Id);
                if (!isSelected && preselectCurrentUser && !string.IsNullOrEmpty(currentUserId) && u.Id == currentUserId)
                {
                    isSelected = true;
                    vm.SelectedOwnerIds.Add(u.Id);
                }

                vm.OwnerOptions.Add(new GroupOwnerOption
                {
                    UserId = u.Id,
                    DisplayName = u.Email ?? u.Id,
                    IsSelected = isSelected
                });
            }
        }

        private async Task LoadExistingOwnersAsync(GroupEditViewModel vm, string groupName)
        {
            vm.CurrentOwners.Clear();

            try
            {
                var owners = await _db.DozentGroupOwnerships
                    .Where(o => o.Group == groupName)
                    .ToListAsync();

                var ownerIds = owners
                    .Select(o => o.DozentUserId)
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                vm.SelectedOwnerIds = ownerIds.ToList();

                if (ownerIds.Count > 0)
                {
                    var users = _userManager.Users
                        .Where(u => ownerIds.Contains(u.Id))
                        .ToList();

                    foreach (var u in users)
                    {
                        vm.CurrentOwners.Add(new GroupOwnerOption
                        {
                            UserId = u.Id,
                            DisplayName = u.Email ?? u.UserName ?? u.Id,
                            IsSelected = true
                        });
                    }
                }
            }
            catch
            {
                // ignore, keep empty
            }
        }

        private static string GenerateInviteCode()
        {
            // simple random code; we can refine later if needed
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        public async Task<IActionResult> EditInviteCode(int id, int codeId, string code, string? note)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group == null) return NotFound();

            var groupName = (group.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupName))
            {
                TempData["error"] = "Ungültiger Gruppenname.";
                return RedirectToAction(nameof(Details), new { id, tab = "invites" });
            }

            var isSuper = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
            var isDozent = User.IsInRole("Dozent");
            if (!isSuper)
            {
                if (!isDozent) return Forbid();
                var uid = _userManager.GetUserId(User);
                bool owner;
                try
                {
                    owner = await _db.DozentGroupOwnerships.AnyAsync(o => o.Group == groupName && o.DozentUserId == uid);
                }
                catch { owner = false; }
                if (!owner) return Forbid();
            }

            try
            {
                var entity = await _db.RegistrationCodes
                    .FirstOrDefaultAsync(rc => rc.Id == codeId && rc.Group != null && rc.Group.Trim() == groupName);
                if (entity == null)
                {
                    TempData["error"] = "Einladungscode wurde nicht gefunden.";
                    return RedirectToAction(nameof(Details), new { id, tab = "invites" });
                }

                // Determine IsActive directly from the posted form value for the checkbox
                var rawValue = Request.Form["isActive"].FirstOrDefault();
                bool isActiveFromForm;
                if (string.IsNullOrEmpty(rawValue))
                {
                    isActiveFromForm = false;
                }
                else
                {
                    var norm = rawValue.Trim().ToLowerInvariant();
                    isActiveFromForm = norm == "true" || norm == "on" || norm == "1" || norm == "yes";
                }

                entity.Code = (code ?? string.Empty).Trim();
                entity.IsActive = isActiveFromForm;
                entity.Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();

                await _db.SaveChangesAsync();
                TempData["success"] = "Einladungscode wurde aktualisiert.";
            }
            catch
            {
                TempData["error"] = "Aktualisieren des Einladungscodes ist fehlgeschlagen.";
            }

            return RedirectToAction(nameof(Details), new { id, tab = "invites" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        public async Task<IActionResult> CreateInviteCode(int id, bool isDozent = false, bool dozentOwner = false)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group == null) return NotFound();

            var groupName = (group.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupName))
            {
                TempData["error"] = "Ungültiger Gruppenname.";
                return RedirectToAction(nameof(Details), new { id, tab = "invites" });
            }

            // Permissions: Admin/SuperAdmin or Dozent owner of this group
            var isSuper = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
            var isDozentUser = User.IsInRole("Dozent");
            if (!isSuper)
            {
                if (!isDozentUser) return Forbid();
                var uid = _userManager.GetUserId(User);
                bool owner;
                try
                {
                    owner = await _db.DozentGroupOwnerships.AnyAsync(o => o.Group == groupName && o.DozentUserId == uid);
                }
                catch { owner = false; }
                if (!owner) return Forbid();
            }

            try
            {
                var code = new RegistrationCode
                {
                    Code = GenerateInviteCode(),
                    IsActive = true,
                    Group = groupName,
                    IsDozentCode = isDozent,
                    DozentBecomesOwner = isDozent && dozentOwner
                };
                _db.RegistrationCodes.Add(code);
                await _db.SaveChangesAsync();

                if (isDozent)
                {
                    TempData["success"] = dozentOwner
                        ? "Neuer Dozenten‑Einladungscode (mit Besitzer‑Rolle) wurde erzeugt."
                        : "Neuer Dozenten‑Einladungscode wurde erzeugt.";
                }
                else
                {
                    TempData["success"] = "Neuer Einladungscode wurde erzeugt.";
                }
            }
            catch
            {
                TempData["error"] = "Erzeugen des Einladungscodes ist fehlgeschlagen.";
            }

            return RedirectToAction(nameof(Details), new { id, tab = "invites" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        public async Task<IActionResult> ToggleInviteCode(int id, int codeId)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group == null) return NotFound();

            var groupName = (group.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupName))
            {
                TempData["error"] = "Ungltiger Gruppenname.";
                return RedirectToAction(nameof(Details), new { id, tab = "invites" });
            }

            var isSuper = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
            var isDozent = User.IsInRole("Dozent");
            if (!isSuper)
            {
                if (!isDozent) return Forbid();
                var uid = _userManager.GetUserId(User);
                bool owner;
                try
                {
                    owner = await _db.DozentGroupOwnerships.AnyAsync(o => o.Group == groupName && o.DozentUserId == uid);
                }
                catch { owner = false; }
                if (!owner) return Forbid();
            }

            try
            {
                var entity = await _db.RegistrationCodes
                    .FirstOrDefaultAsync(rc => rc.Id == codeId && rc.Group != null && rc.Group.Trim() == groupName);
                if (entity == null)
                {
                    TempData["error"] = "Einladungscode wurde nicht gefunden.";
                    return RedirectToAction(nameof(Details), new { id, tab = "invites" });
                }

                entity.IsActive = !entity.IsActive;
                await _db.SaveChangesAsync();
                TempData["success"] = entity.IsActive ? "Code aktiviert." : "Code deaktiviert.";
            }
            catch
            {
                TempData["error"] = "Aktualisieren des Einladungscodes ist fehlgeschlagen.";
            }

            return RedirectToAction(nameof(Details), new { id, tab = "invites" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        public async Task<IActionResult> DeleteInviteCode(int id, int codeId)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group == null) return NotFound();

            var groupName = (group.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupName))
            {
                TempData["error"] = "Ungltiger Gruppenname.";
                return RedirectToAction(nameof(Details), new { id, tab = "invites" });
            }

            var isSuper = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
            var isDozent = User.IsInRole("Dozent");
            if (!isSuper)
            {
                if (!isDozent) return Forbid();
                var uid = _userManager.GetUserId(User);
                bool owner;
                try
                {
                    owner = await _db.DozentGroupOwnerships.AnyAsync(o => o.Group == groupName && o.DozentUserId == uid);
                }
                catch { owner = false; }
                if (!owner) return Forbid();
            }

            try
            {
                var entity = await _db.RegistrationCodes
                    .FirstOrDefaultAsync(rc => rc.Id == codeId && rc.Group != null && rc.Group.Trim() == groupName);
                if (entity != null)
                {
                    _db.RegistrationCodes.Remove(entity);
                    await _db.SaveChangesAsync();
                    TempData["success"] = "Einladungscode wurde gelscht.";
                }
            }
            catch
            {
                TempData["error"] = "Lschen des Einladungscodes ist fehlgeschlagen.";
            }

            return RedirectToAction(nameof(Details), new { id, tab = "invites" });
        }

        // POST: /Admin/Groups/SavePrompts/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        public async Task<IActionResult> SavePrompts(int id, GroupPromptsAdminViewModel model)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group == null) return NotFound();

            var groupName = (group.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupName))
            {
                TempData["error"] = "Ungültiger Gruppenname.";
                return RedirectToAction(nameof(Details), new { id, tab = "invites" });
            }

            // Permissions: Admin/SuperAdmin or Dozent owner of this group
            var isSuper = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
            var isDozent = User.IsInRole("Dozent");
            if (!isSuper)
            {
                if (!isDozent) return Forbid();
                var uid = _userManager.GetUserId(User);
                bool owner;
                try
                {
                    owner = await _db.DozentGroupOwnerships.AnyAsync(o => o.Group == groupName && o.DozentUserId == uid);
                }
                catch { owner = false; }
                if (!owner) return Forbid();
            }

            try
            {
                var latest = await _db.GroupPromptAiSettings
                    .Where(g => g.Group == groupName)
                    .OrderByDescending(g => g.UpdatedAt)
                    .FirstOrDefaultAsync();

                var rec = new GroupPromptAiSetting
                {
                    Group = groupName,
                    UseGlobal = model.UseGlobal,
                    SystemPreamble = string.IsNullOrWhiteSpace(model.SystemPreamble) ? latest?.SystemPreamble : model.SystemPreamble.Trim(),
                    KiAssistantSystemPrompt = string.IsNullOrWhiteSpace(model.KiAssistantSystemPrompt) ? latest?.KiAssistantSystemPrompt : model.KiAssistantSystemPrompt.Trim(),
                    UserInstructionText = string.IsNullOrWhiteSpace(model.UserInstructionText) ? latest?.UserInstructionText : model.UserInstructionText.Trim(),
                    FilterSystemPreamble = latest?.FilterSystemPreamble,
                    FilterFirstLine = latest?.FilterFirstLine,
                    SmartSelectionSystemPreamble = latest?.SmartSelectionSystemPreamble,
                    SmartSelectionUserPrompt = latest?.SmartSelectionUserPrompt,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedByUserId = _userManager.GetUserId(User)
                };

                _db.GroupPromptAiSettings.Add(rec);
                await _db.SaveChangesAsync();
                TempData["success"] = "System-Prompts für diese Gruppe wurden gespeichert.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Speichern der Gruppen‑Prompts fehlgeschlagen: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> UploadGroupImage(int id, IFormFile? imageFile)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group == null) return NotFound();

            if (imageFile == null || imageFile.Length == 0)
            {
                TempData["error"] = "Bitte wählen Sie ein Bild aus.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(imageFile.ContentType) ||
                !imageFile.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                TempData["error"] = "Nur Bilddateien sind erlaubt.";
                return RedirectToAction(nameof(Index));
            }

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            var isDozentOnly = User.IsInRole("Dozent") && !isAdmin;

            if (isDozentOnly)
            {
                var uid = _userManager.GetUserId(User);
                bool isOwner = false;
                try
                {
                    isOwner = await _db.DozentGroupOwnerships
                        .AnyAsync(o => o.Group == group.Name && o.DozentUserId == uid);
                }
                catch
                {
                    isOwner = false;
                }

                if (!isOwner) return Forbid();
            }

            try
            {
                string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string uploadsFolder = Path.Combine(wwwRootPath, "images", "groups");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                if (!string.IsNullOrWhiteSpace(group.ImageUrl) &&
                    group.ImageUrl.StartsWith("/images/groups", StringComparison.OrdinalIgnoreCase))
                {
                    var oldImagePath = Path.Combine(
                        wwwRootPath,
                        group.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                string extension = Path.GetExtension(imageFile.FileName);
                if (string.IsNullOrWhiteSpace(extension))
                {
                    extension = ".jpg";
                }

                string fileName = $"group-{group.Id}-{Guid.NewGuid():N}{extension}";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                group.ImageUrl = "/images/groups/" + fileName;
                await _db.SaveChangesAsync();

                TempData["success"] = "Gruppenbild wurde aktualisiert.";
            }
            catch
            {
                TempData["error"] = "Das Gruppenbild konnte nicht gespeichert werden.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        public async Task<IActionResult> Delete(int id)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group == null) return NotFound();

            var groupName = (group.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupName))
            {
                TempData["error"] = "Ungültiger Gruppenname.";
                return RedirectToAction(nameof(Index));
            }

            var isSuper = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
            var isDozent = User.IsInRole("Dozent");

            if (!isSuper)
            {
                if (!isDozent) return Forbid();
                var uid = _userManager.GetUserId(User);
                bool owner = false;
                try
                {
                    owner = await _db.DozentGroupOwnerships.AnyAsync(o => o.Group == groupName && o.DozentUserId == uid);
                }
                catch
                {
                    owner = false;
                }

                if (!owner) return Forbid();
            }

            List<string> memberUserIds;
            try
            {
                memberUserIds = await _db.UserGroupMemberships
                    .Where(m => m.Group != null && m.Group.Trim() == groupName)
                    .Select(m => m.UserId)
                    .Distinct()
                    .ToListAsync();
            }
            catch
            {
                memberUserIds = new List<string>();
            }

            var usersToDelete = new List<IdentityUser>();

            foreach (var userId in memberUserIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) continue;

                bool isAdminRole = await _userManager.IsInRoleAsync(user, "Admin");
                bool isSuperAdminRole = await _userManager.IsInRoleAsync(user, "SuperAdmin");
                bool isDozentRole = await _userManager.IsInRoleAsync(user, "Dozent");
                bool isApiManagerRole = await _userManager.IsInRoleAsync(user, "ApiManager");

                if (isAdminRole || isSuperAdminRole || isDozentRole || isApiManagerRole)
                {
                    continue;
                }

                HashSet<string> groupsForUser;
                try
                {
                    var groupsRaw = await _db.UserGroupMemberships
                        .Where(m => m.UserId == userId && m.Group != null && m.Group != "")
                        .Select(m => m.Group!)
                        .ToListAsync();

                    groupsForUser = new HashSet<string>(
                        groupsRaw
                            .Where(g => !string.IsNullOrWhiteSpace(g))
                            .Select(g => g.Trim()),
                        StringComparer.OrdinalIgnoreCase);
                }
                catch
                {
                    continue;
                }

                if (groupsForUser.Count == 0)
                {
                    continue;
                }

                bool onlyThisGroup = groupsForUser.All(g => string.Equals(g, groupName, StringComparison.OrdinalIgnoreCase));
                if (onlyThisGroup)
                {
                    usersToDelete.Add(user);
                }
            }

            try
            {
                var groupMemberships = _db.UserGroupMemberships
                    .Where(m => m.Group != null && m.Group.Trim() == groupName);
                _db.UserGroupMemberships.RemoveRange(groupMemberships);

                var owners = _db.DozentGroupOwnerships
                    .Where(o => o.Group == groupName);
                _db.DozentGroupOwnerships.RemoveRange(owners);

                var featureSettings = _db.GroupFeatureSettings
                    .Where(f => f.Group == groupName);
                _db.GroupFeatureSettings.RemoveRange(featureSettings);

                var promptSettings = _db.GroupPromptAiSettings
                    .Where(p => p.Group == groupName);
                _db.GroupPromptAiSettings.RemoveRange(promptSettings);

                var apiSettings = _db.GroupApiKeySettings
                    .Where(a => a.Group == groupName);
                _db.GroupApiKeySettings.RemoveRange(apiSettings);

                var typeGuidances = _db.GroupPromptTypeGuidances
                    .Where(t => t.Group == groupName);
                _db.GroupPromptTypeGuidances.RemoveRange(typeGuidances);

                var promptGroups = _db.PromptTemplateGroups
                    .Where(pg => pg.Group != null && pg.Group.Trim() == groupName);
                _db.PromptTemplateGroups.RemoveRange(promptGroups);

                var assistantGroups = _db.AssistantGroups
                    .Where(ag => ag.Group != null && ag.Group.Trim() == groupName);
                _db.AssistantGroups.RemoveRange(assistantGroups);

                var registrationCodes = _db.RegistrationCodes
                    .Where(rc => rc.Group != null && rc.Group.Trim() == groupName);
                _db.RegistrationCodes.RemoveRange(registrationCodes);

                var dailyStats = _db.UserDailyActivityStats
                    .Where(s => s.Group != null && s.Group.Trim() == groupName);
                _db.UserDailyActivityStats.RemoveRange(dailyStats);

                foreach (var user in usersToDelete)
                {
                    var uid = user.Id;

                    try
                    {
                        var mems = _db.UserGroupMemberships.Where(m => m.UserId == uid);
                        _db.UserGroupMemberships.RemoveRange(mems);

                        var acts = _db.UserActivities.Where(a => a.UserId == uid);
                        _db.UserActivities.RemoveRange(acts);

                        var owns = _db.DozentGroupOwnerships.Where(o => o.DozentUserId == uid);
                        _db.DozentGroupOwnerships.RemoveRange(owns);

                        var appUser = await _db.AplicationUser.FirstOrDefaultAsync(a => a.Id == uid);
                        if (appUser != null)
                        {
                            _db.AplicationUser.Remove(appUser);
                        }
                    }
                    catch
                    {
                    }
                }

                _db.Groups.Remove(group);

                await _db.SaveChangesAsync();

                foreach (var user in usersToDelete)
                {
                    try
                    {
                        await _userManager.DeleteAsync(user);
                    }
                    catch
                    {
                    }
                }

                TempData["success"] = $"Gruppe '{groupName}' wurde gelöscht. {usersToDelete.Count} Benutzerkonten wurden entfernt.";
            }
            catch
            {
                TempData["error"] = "Löschen der Gruppe ist fehlgeschlagen.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Groups/SaveApiKeys/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        public async Task<IActionResult> SaveApiKeys(int id, GroupApiKeysAdminViewModel model)
        {
            var group = await _db.Groups.FindAsync(id);
            if (group == null) return NotFound();

            var groupName = (group.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupName))
            {
                TempData["error"] = "Ungültiger Gruppenname.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var isSuper = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
            var isDozent = User.IsInRole("Dozent");

            if (!isSuper)
            {
                if (!isDozent) return Forbid();
                var uid = _userManager.GetUserId(User);
                bool owner = false;
                try
                {
                    owner = await _db.DozentGroupOwnerships.AnyAsync(o => o.Group == groupName && o.DozentUserId == uid);
                }
                catch { owner = false; }
                if (!owner) return Forbid();
            }

            try
            {
                var prev = await _db.GroupApiKeySettings
                    .Where(g => g.Group == groupName)
                    .OrderByDescending(g => g.UpdatedAt)
                    .FirstOrDefaultAsync();

                var rec = new GroupApiKeySetting
                {
                    Group = groupName,
                    ActiveProvider = string.IsNullOrWhiteSpace(model.ActiveProvider)
                        ? prev?.ActiveProvider
                        : model.ActiveProvider.Trim().ToLowerInvariant(),

                    KisskiApiKey = string.IsNullOrWhiteSpace(model.KisskiApiKey) ? prev?.KisskiApiKey : model.KisskiApiKey.Trim(),
                    KisskiModel = string.IsNullOrWhiteSpace(model.KisskiModel) ? prev?.KisskiModel : model.KisskiModel.Trim(),

                    OpenAIKey = string.IsNullOrWhiteSpace(model.OpenAIKey) ? prev?.OpenAIKey : model.OpenAIKey.Trim(),
                    OpenAIModel = string.IsNullOrWhiteSpace(model.OpenAIModel) ? prev?.OpenAIModel : model.OpenAIModel.Trim(),

                    GeminiApiKey = string.IsNullOrWhiteSpace(model.GeminiApiKey) ? prev?.GeminiApiKey : model.GeminiApiKey.Trim(),
                    GeminiModel = string.IsNullOrWhiteSpace(model.GeminiModel) ? prev?.GeminiModel : model.GeminiModel.Trim(),

                    ClaudeApiKey = string.IsNullOrWhiteSpace(model.ClaudeApiKey) ? prev?.ClaudeApiKey : model.ClaudeApiKey.Trim(),
                    ClaudeModel = string.IsNullOrWhiteSpace(model.ClaudeModel) ? prev?.ClaudeModel : model.ClaudeModel.Trim(),

                    UpdatedAt = DateTime.UtcNow,
                    UpdatedByUserId = _userManager.GetUserId(User)
                };

                _db.GroupApiKeySettings.Add(rec);
                await _db.SaveChangesAsync();
                TempData["success"] = "API‑Schlüssel für diese Gruppe wurden gespeichert.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Speichern der Gruppen‑API‑Schlüssel fehlgeschlagen: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
