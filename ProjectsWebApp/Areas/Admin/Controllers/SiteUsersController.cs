using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.Areas.Admin.Models;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + ",SuperAdmin")]
    public sealed class SiteUsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userMgr;

        public SiteUsersController(ApplicationDbContext db, UserManager<IdentityUser> userMgr)
        {
            _db = db;
            _userMgr = userMgr;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchString, int pageNumber = 1)
        {
            ViewData["Title"] = "Teilnehmende nach Räumen";
            int pageSize = 10; // Or make it configurable
            var model = await LoadGroupsAsync(searchString, pageNumber, pageSize);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> All(string? searchString, int pageNumber = 1)
        {
            ViewData["Title"] = "Alle Teilnehmende";
            int pageSize = 25;
            var model = await LoadUsersAsync(searchString, pageNumber, pageSize);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToReturnUrl(returnUrl);

            var user = await _userMgr.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["error"] = "Benutzer nicht gefunden.";
                return RedirectToReturnUrl(returnUrl);
            }

            if (await IsProtectedUserAsync(user))
            {
                TempData["error"] = "Dieser Benutzer kann nicht gelöscht werden.";
                return RedirectToReturnUrl(returnUrl);
            }

            await CleanupUserDataAsync(user.Id);

            var del = await _userMgr.DeleteAsync(user);
            TempData[del.Succeeded ? "success" : "error"] = del.Succeeded
                ? $"Benutzer {user.Email ?? user.UserName ?? user.Id} gelöscht."
                : "Fehler beim Löschen.";

            return RedirectToReturnUrl(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGroupUsers(string groupName, string? returnUrl = null)
        {
            var groupNorm = (groupName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(groupNorm))
            {
                TempData["error"] = "Ungültiger Gruppenname.";
                return RedirectToReturnUrl(returnUrl);
            }

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

            bool deleteNoGroup = string.Equals(groupNorm, "Ohne Gruppe", StringComparison.OrdinalIgnoreCase);

            var allUsers = await _userMgr.Users.ToListAsync();
            var targets = new List<IdentityUser>();

            foreach (var u in allUsers)
            {
                if (await IsProtectedUserAsync(u))
                    continue;

                if (!groupLookup.TryGetValue(u.Id, out var grp))
                {
                    if (deleteNoGroup) targets.Add(u);
                    continue;
                }

                var hasGroup = !string.IsNullOrWhiteSpace(grp);
                if (deleteNoGroup)
                {
                    if (!hasGroup) targets.Add(u);
                }
                else
                {
                    if (string.Equals((grp ?? string.Empty).Trim(), groupNorm, StringComparison.OrdinalIgnoreCase))
                        targets.Add(u);
                }
            }

            if (targets.Count == 0)
            {
                TempData["success"] = $"Keine Benutzer in Gruppe '{groupNorm}' zu löschen.";
                return RedirectToReturnUrl(returnUrl);
            }

            int deleted = 0;
            foreach (var u in targets)
            {
                await CleanupUserDataAsync(u.Id);
                var res = await _userMgr.DeleteAsync(u);
                if (res.Succeeded) deleted++;
            }

            TempData["success"] = $"{deleted} Benutzer aus Gruppe '{groupNorm}' gelöscht.";
            return RedirectToReturnUrl(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllRegularUsers(string? returnUrl = null)
        {
            var allUsers = await _userMgr.Users.ToListAsync();
            int deleted = 0;

            foreach (var u in allUsers)
            {
                if (await IsProtectedUserAsync(u))
                    continue;

                await CleanupUserDataAsync(u.Id);
                var res = await _userMgr.DeleteAsync(u);
                if (res.Succeeded) deleted++;
            }

            TempData["success"] = $"{deleted} Benutzer wurden gelöscht.";
            return RedirectToReturnUrl(returnUrl);
        }

        private IActionResult RedirectToReturnUrl(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Groups", new { area = "Admin" });
        }

        private async Task<SiteUsersIndexViewModel> LoadGroupsAsync(string? searchString, int pageNumber, int pageSize)
        {
            var model = new SiteUsersIndexViewModel
            {
                SearchString = searchString,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

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

            var adminIds = new HashSet<string>((await _userMgr.GetUsersInRoleAsync("Admin")).Select(u => u.Id));
            var superIds = new HashSet<string>((await _userMgr.GetUsersInRoleAsync("SuperAdmin")).Select(u => u.Id));
            var dozentIds = new HashSet<string>((await _userMgr.GetUsersInRoleAsync("Dozent")).Select(u => u.Id));

            var allUsers = await _userMgr.Users.ToListAsync();
            var grouped = new Dictionary<string, List<SiteUsersUserEntry>>(StringComparer.OrdinalIgnoreCase);

            foreach (var u in allUsers)
            {
                // Only exclude Admin and SuperAdmin
                if (adminIds.Contains(u.Id) || superIds.Contains(u.Id))
                    continue;

                groupLookup.TryGetValue(u.Id, out var grpRaw);
                var norm = grpRaw?.Trim();
                var key = string.IsNullOrWhiteSpace(norm) ? "Ohne Gruppe" : norm!;

                if (!grouped.TryGetValue(key, out var list))
                {
                    list = new List<SiteUsersUserEntry>();
                    grouped[key] = list;
                }

                list.Add(new SiteUsersUserEntry
                {
                    Id = u.Id,
                    Email = u.Email ?? u.UserName ?? u.Id
                });
            }

            // Filter
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var search = searchString.Trim();
                var filteredGroups = new Dictionary<string, List<SiteUsersUserEntry>>(StringComparer.OrdinalIgnoreCase);

                foreach (var kv in grouped)
                {
                    bool groupMatches = kv.Key.Contains(search, StringComparison.OrdinalIgnoreCase);
                    var matchingUsers = kv.Value.Where(u =>
                        (u.Email != null && u.Email.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                        u.Id.Contains(search, StringComparison.OrdinalIgnoreCase)
                    ).ToList();

                    if (groupMatches)
                    {
                        // If group matches, keep all users
                        filteredGroups[kv.Key] = kv.Value;
                    }
                    else if (matchingUsers.Any())
                    {
                        // If group doesn't match but users do, keep matching users
                        filteredGroups[kv.Key] = matchingUsers;
                    }
                }
                grouped = filteredGroups;
            }

            // Calculate totals
            model.TotalUsers = grouped.Sum(g => g.Value.Count);
            model.TotalPages = (int)Math.Ceiling((double)grouped.Count / pageSize);

            // Paginate Groups
            var pagedGroups = grouped.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase)
                                     .Skip((pageNumber - 1) * pageSize)
                                     .Take(pageSize);

            foreach (var kv in pagedGroups)
            {
                model.Groups.Add(new SiteUsersGroupBucket
                {
                    GroupName = kv.Key,
                    Users = kv.Value.OrderBy(x => x.Email, StringComparer.OrdinalIgnoreCase).ToList()
                });
            }

            return model;
        }

        private async Task<SiteUsersAllViewModel> LoadUsersAsync(string? searchString, int pageNumber, int pageSize)
        {
            var model = new SiteUsersAllViewModel
            {
                SearchString = searchString,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            var adminIds = new HashSet<string>((await _userMgr.GetUsersInRoleAsync("Admin")).Select(u => u.Id));
            var superIds = new HashSet<string>((await _userMgr.GetUsersInRoleAsync("SuperAdmin")).Select(u => u.Id));

            var users = (await _userMgr.Users.ToListAsync())
                .Where(u => !adminIds.Contains(u.Id) && !superIds.Contains(u.Id))
                .Select(u => new SiteUsersUserEntry
                {
                    Id = u.Id,
                    Email = u.Email ?? u.UserName ?? u.Id
                })
                .ToList();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var search = searchString.Trim();
                users = users
                    .Where(u =>
                        (!string.IsNullOrWhiteSpace(u.Email) && u.Email.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(u.Id) && u.Id.Contains(search, StringComparison.OrdinalIgnoreCase))
                    )
                    .ToList();
            }

            users = users.OrderBy(u => u.Email, StringComparer.OrdinalIgnoreCase).ToList();

            model.TotalUsers = users.Count;
            model.TotalPages = (int)Math.Ceiling((double)model.TotalUsers / pageSize);
            model.Users = users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return model;
        }

        private async Task<bool> IsProtectedUserAsync(IdentityUser user)
        {
            if (await _userMgr.IsInRoleAsync(user, "SuperAdmin")) return true;
            if (await _userMgr.IsInRoleAsync(user, "Admin")) return true;
            if (await _userMgr.IsInRoleAsync(user, "Dozent")) return true;
            return false;
        }

        private async Task CleanupUserDataAsync(string userId)
        {
            try
            {
                var mems = _db.UserGroupMemberships.Where(m => m.UserId == userId);
                _db.UserGroupMemberships.RemoveRange(mems);

                var acts = _db.UserActivities.Where(a => a.UserId == userId);
                _db.UserActivities.RemoveRange(acts);

                var owns = _db.DozentGroupOwnerships.Where(o => o.DozentUserId == userId);
                _db.DozentGroupOwnerships.RemoveRange(owns);

                try
                {
                    var appUser = await _db.AplicationUser.FirstOrDefaultAsync(a => a.Id == userId);
                    if (appUser != null) _db.AplicationUser.Remove(appUser);
                }
                catch { }

                await _db.SaveChangesAsync();
            }
            catch { }
        }
    }
}
