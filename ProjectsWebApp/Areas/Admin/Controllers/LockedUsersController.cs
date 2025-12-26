using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.Areas.Admin.Models;
using ProjectsWebApp.DataAccsess.Data;

namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class LockedUsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public LockedUsersController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var states = _db.UserSecurityStates
                .Where(s => s.IsPermanentlyLocked)
                .ToList();

            var userIds = states.Select(s => s.UserId).ToList();
            var users = _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToList();
            var userById = users.ToDictionary(u => u.Id, u => u);

            var model = new List<PermanentlyLockedUserViewModel>();

            foreach (var s in states)
            {
                userById.TryGetValue(s.UserId, out var u);
                model.Add(new PermanentlyLockedUserViewModel
                {
                    UserId = s.UserId,
                    Email = u?.Email,
                    UserName = u?.UserName,
                    LockoutWindowCount = s.LockoutWindowCount,
                    FirstLockoutAtUtc = s.FirstLockoutAtUtc,
                    LastLockoutAtUtc = s.LastLockoutAtUtc
                });
            }

            model = model
                .OrderByDescending(m => m.LastLockoutAtUtc ?? m.FirstLockoutAtUtc ?? DateTime.MinValue)
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToAction(nameof(Index));
            }

            var state = _db.UserSecurityStates.FirstOrDefault(s => s.UserId == userId);
            var user = await _userManager.FindByIdAsync(userId);

            if (state != null)
            {
                state.IsPermanentlyLocked = false;
                state.LockoutWindowCount = 0;
                state.FirstLockoutAtUtc = null;
                state.LastLockoutAtUtc = null;
            }

            if (user != null)
            {
                user.LockoutEnd = null;
                await _userManager.UpdateAsync(user);
            }

            await _db.SaveChangesAsync();
            TempData["success"] = "Konto wurde wieder freigeschaltet.";

            return RedirectToAction(nameof(Index));
        }
    }
}
