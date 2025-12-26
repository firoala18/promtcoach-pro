using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.DataAccsess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectsWebApp.ViewComponents
{
    public class AdminUsersMegaMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userMgr;

        public AdminUsersMegaMenuViewComponent(ApplicationDbContext db, UserManager<IdentityUser> userMgr)
        {
            _db = db;
            _userMgr = userMgr;
        }

        public sealed class UserEntry
        {
            public string Id { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }

        public sealed class GroupBucket
        {
            public string GroupName { get; set; } = string.Empty;
            public List<UserEntry> Users { get; set; } = new();
        }

        public sealed class AdminUsersMegaMenuModel
        {
            public string ReturnUrl { get; set; } = "/";
            public List<GroupBucket> Groups { get; set; } = new();
            public int TotalUsers { get; set; }
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new AdminUsersMegaMenuModel();

            try
            {
                var path = HttpContext?.Request?.Path.Value ?? "/";
                var query = HttpContext?.Request?.QueryString.Value ?? string.Empty;
                model.ReturnUrl = string.IsNullOrWhiteSpace(query) ? path : path + query;
            }
            catch
            {
                model.ReturnUrl = "/";
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

            var adminIds = new HashSet<string>((await _userMgr.GetUsersInRoleAsync("Admin")).Select(u => u.Id));
            var superIds = new HashSet<string>((await _userMgr.GetUsersInRoleAsync("SuperAdmin")).Select(u => u.Id));
            var dozentIds = new HashSet<string>((await _userMgr.GetUsersInRoleAsync("Dozent")).Select(u => u.Id));

            var allUsers = await _userMgr.Users.ToListAsync();
            var grouped = new Dictionary<string, List<UserEntry>>(StringComparer.OrdinalIgnoreCase);

            foreach (var u in allUsers)
            {
                if (adminIds.Contains(u.Id) || superIds.Contains(u.Id) || dozentIds.Contains(u.Id))
                    continue;

                groupLookup.TryGetValue(u.Id, out var grpRaw);
                var norm = grpRaw?.Trim();
                var key = string.IsNullOrWhiteSpace(norm) ? "Ohne Gruppe" : norm!;

                if (!grouped.TryGetValue(key, out var list))
                {
                    list = new List<UserEntry>();
                    grouped[key] = list;
                }

                list.Add(new UserEntry
                {
                    Id = u.Id,
                    Email = u.Email ?? u.UserName ?? u.Id
                });
            }

            foreach (var kv in grouped.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
            {
                model.Groups.Add(new GroupBucket
                {
                    GroupName = kv.Key,
                    Users = kv.Value.OrderBy(x => x.Email, StringComparer.OrdinalIgnoreCase).ToList()
                });
            }

            model.TotalUsers = model.Groups.Sum(g => g.Users.Count);
            return View(model);
        }
    }
}
