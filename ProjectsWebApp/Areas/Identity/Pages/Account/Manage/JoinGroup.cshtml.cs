using System.ComponentModel.DataAnnotations;
using System.Linq;
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
    public class JoinGroupModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public JoinGroupModel(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        // Groups the current user already belongs to (for display only)
        public List<string> MemberGroups { get; set; } = new();

        public class InputModel
        {
            [Required]
            [Display(Name = "Einladungscode")]
            public string InviteCode { get; set; } = string.Empty;
        }

        public async Task OnGetAsync()
        {
            await LoadMembershipsAsync();
        }

        private async Task LoadMembershipsAsync()
        {
            MemberGroups.Clear();
            try
            {
                var uid = _userManager.GetUserId(User);
                if (string.IsNullOrWhiteSpace(uid)) return;

                var groups = await _db.UserGroupMemberships
                    .Where(m => m.UserId == uid && m.Group != null && m.Group != "")
                    .Select(m => m.Group!.Trim())
                    .Where(g => g != "")
                    .Distinct()
                    .OrderBy(g => g)
                    .ToListAsync();

                MemberGroups.AddRange(groups);
            }
            catch { }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadMembershipsAsync();
                return Page();
            }

            try
            {
                var code = (Input?.InviteCode ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(code))
                {
                    TempData["error"] = "Bitte einen Einladungscode eingeben.";
                    await LoadMembershipsAsync();
                    return Page();
                }

                var codeEntry = await _db.RegistrationCodes
                    .FirstOrDefaultAsync(c => c.Code == code && c.IsActive);
                if (codeEntry == null)
                {
                    TempData["error"] = "Ungültiger Einladungscode.";
                    await LoadMembershipsAsync();
                    return Page();
                }

                var groupName = codeEntry.Group?.Trim();
                if (string.IsNullOrWhiteSpace(groupName))
                {
                    TempData["error"] = "Dieser Einladungscode ist keiner Gruppe zugeordnet.";
                    await LoadMembershipsAsync();
                    return Page();
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
                        UserId = uid,
                        Group = groupName,
                        CreatedAt = createdAt
                    });
                    await _db.SaveChangesAsync();
                }

                TempData["success"] = $"Gruppe '{groupName}' wurde deinem Konto hinzugefügt.";
                return RedirectToPage("./Index");
            }
            catch
            {
                TempData["error"] = "Unerwarteter Fehler beim Beitritt. Bitte erneut versuchen.";
                await LoadMembershipsAsync();
                return Page();
            }
        }
    }
}
