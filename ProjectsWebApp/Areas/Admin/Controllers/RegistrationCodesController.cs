using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RegistrationCodesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userMgr;
        public RegistrationCodesController(ApplicationDbContext db, UserManager<IdentityUser> userMgr)
        { _db = db; _userMgr = userMgr; }

        private async Task<HashSet<string>> OwnedGroupsAsync()
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var uid = _userMgr.GetUserId(User);
                if (User.IsInRole("Dozent") && !User.IsInRole("SuperAdmin"))
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

        public async Task<IActionResult> Index()
        {
            IQueryable<RegistrationCode> q = _db.RegistrationCodes;
            if (User.IsInRole("Dozent") && !User.IsInRole("SuperAdmin"))
            {
                var owned = await OwnedGroupsAsync();
                if (owned.Count == 0) return View(new List<RegistrationCode>());
                q = q.Where(c => c.Group != null && owned.Contains(c.Group));
            }
            var codes = await q.OrderBy(c => c.Id).ToListAsync();
            var ownerMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var owns = await _db.DozentGroupOwnerships.ToListAsync();
                var ids = owns.Select(o => o.DozentUserId).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var idToEmail = _userMgr.Users.Where(u => ids.Contains(u.Id)).ToDictionary(u => u.Id, u => u.Email ?? u.Id, StringComparer.OrdinalIgnoreCase);
                foreach (var o in owns)
                {
                    var g = (o.Group ?? string.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(g)) continue;
                    if (!ownerMap.TryGetValue(g, out var list)) { list = new List<string>(); ownerMap[g] = list; }
                    if (idToEmail.TryGetValue(o.DozentUserId, out var mail))
                    {
                        if (!list.Any(x => string.Equals(x, mail, StringComparison.OrdinalIgnoreCase))) list.Add(mail);
                    }
                }
            }
            catch { }
            ViewBag.OwnerEmailsByGroup = ownerMap;
            return View(codes);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            if (User.IsInRole("Dozent") && !User.IsInRole("SuperAdmin"))
            {
                var owned = await OwnedGroupsAsync();
                ViewBag.OwnedGroups = owned.ToList();
            }

            if (id == null)
                return View(new RegistrationCode());

            var code = await _db.RegistrationCodes.FindAsync(id);
            if (code == null) return NotFound();
            if (User.IsInRole("Dozent") && !User.IsInRole("SuperAdmin"))
            {
                var owned = await OwnedGroupsAsync();
                var groupNorm = (code.Group ?? string.Empty).Trim();
                if (owned.Count == 0 || string.IsNullOrWhiteSpace(groupNorm) || !owned.Contains(groupNorm)) return Forbid();
            }
            return View(code);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(RegistrationCode model)
        {
            if (!ModelState.IsValid)
            {
                if (User.IsInRole("Dozent") && !User.IsInRole("SuperAdmin"))
                {
                    var owned = await OwnedGroupsAsync();
                    ViewBag.OwnedGroups = owned.ToList();
                }
                return View(model);
            }

            if (model.Id == 0)
            {
                if (User.IsInRole("Dozent") && !User.IsInRole("SuperAdmin"))
                {
                    var groupNorm = (model.Group ?? string.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(groupNorm))
                    {
                        ModelState.AddModelError("Group", "Als Dozent müssen Sie eine Gruppe angeben.");
                        var owned = await OwnedGroupsAsync();
                        ViewBag.OwnedGroups = owned.ToList();
                        return View(model);
                    }

                    // Claim ownership if group is unowned; forbid if owned by someone else
                    var existing = await _db.DozentGroupOwnerships.FirstOrDefaultAsync(o => o.Group == groupNorm);
                    var uid = _userMgr.GetUserId(User);
                    if (existing == null)
                    {
                        _db.DozentGroupOwnerships.Add(new DozentGroupOwnership
                        {
                            DozentUserId = uid,
                            Group = groupNorm,
                            CreatedAt = DateTime.UtcNow
                        });
                        await _db.SaveChangesAsync();
                    }
                    else if (!string.Equals(existing.DozentUserId, uid, StringComparison.Ordinal))
                    {
                        ModelState.AddModelError("Group", "Diese Gruppe gehört bereits einem anderen Dozenten.");
                        var owned = await OwnedGroupsAsync();
                        ViewBag.OwnedGroups = owned.ToList();
                        return View(model);
                    }

                    // Ensure the creating Dozent is also a member of this group (latest membership)
                    try
                    {
                        var latest = await _db.UserGroupMemberships
                            .Where(m => m.UserId == uid)
                            .OrderByDescending(m => m.CreatedAt)
                            .Select(m => m.Group)
                            .FirstOrDefaultAsync();
                        var latestNorm = (latest ?? string.Empty).Trim();
                        if (!string.Equals(latestNorm, groupNorm, StringComparison.OrdinalIgnoreCase))
                        {
                            _db.UserGroupMemberships.Add(new UserGroupMembership { UserId = uid, Group = groupNorm, CreatedAt = DateTime.UtcNow });
                            await _db.SaveChangesAsync();
                        }
                    }
                    catch { }
                    model.Group = groupNorm;

                    // Ensure a Groups row exists immediately when creating an invitation code
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(groupNorm))
                        {
                            var grp = await _db.Groups.FirstOrDefaultAsync(g => g.Name == groupNorm);
                            if (grp == null)
                            {
                                _db.Groups.Add(new Group { Name = groupNorm });
                                await _db.SaveChangesAsync();
                            }
                        }
                    }
                    catch { }
                }
                else if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
                {
                    // Admin/Coach creating a code: claim ownership if unowned so it's visible who owns the group
                    var groupNorm = (model.Group ?? string.Empty).Trim();
                    if (!string.IsNullOrWhiteSpace(groupNorm))
                    {
                        var uid = _userMgr.GetUserId(User);
                        var alreadyOwner = await _db.DozentGroupOwnerships.AnyAsync(o => o.Group == groupNorm && o.DozentUserId == uid);
                        if (!alreadyOwner)
                        {
                            _db.DozentGroupOwnerships.Add(new DozentGroupOwnership
                            {
                                DozentUserId = uid,
                                Group = groupNorm,
                                CreatedAt = DateTime.UtcNow
                            });
                            await _db.SaveChangesAsync();
                        }

                        // Ensure a Groups row exists for Admin-created codes as well
                        try
                        {
                            var grp = await _db.Groups.FirstOrDefaultAsync(g => g.Name == groupNorm);
                            if (grp == null)
                            {
                                _db.Groups.Add(new Group { Name = groupNorm });
                                await _db.SaveChangesAsync();
                            }
                        }
                        catch { }
                    }
                }
                _db.RegistrationCodes.Add(model);
                TempData["success"] = "Code angelegt.";
            }
            else
            {
                var ent = await _db.RegistrationCodes.FindAsync(model.Id);
                if (ent == null) return NotFound();
                if (User.IsInRole("Dozent") && !User.IsInRole("SuperAdmin"))
                {
                    var owned = await OwnedGroupsAsync();
                    var currentGroup = ent.Group?.Trim();
                    if (owned.Count == 0 || string.IsNullOrWhiteSpace(currentGroup) || !owned.Contains(currentGroup!))
                    {
                        ModelState.AddModelError("Group", "Diese Gruppe gehört nicht Ihnen.");
                        // keep entity values for display, but show posted basics
                        if (User.IsInRole("Dozent") && !User.IsInRole("SuperAdmin"))
                        {
                            ViewBag.OwnedGroups = owned.ToList();
                        }
                        // ensure the model carries the entity group back to the view
                        model.Group = ent.Group;
                        return View(model);
                    }
                    // Dozent cannot change group on edit; enforce original group
                    model.Group = ent.Group;
                }
                // capture old/new names (normalized)
                var oldGroupNorm = (ent.Group ?? string.Empty).Trim();
                var newGroupNorm = (model.Group ?? string.Empty).Trim();

                ent.Code = model.Code;
                ent.IsActive = model.IsActive;
                ent.Note = model.Note;
                ent.Group = model.Group;

                // If Admin/SuperAdmin changed the group name, update Groups table accordingly
                if ((User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
                    && !string.IsNullOrWhiteSpace(oldGroupNorm)
                    && !string.IsNullOrWhiteSpace(newGroupNorm)
                    && !string.Equals(oldGroupNorm, newGroupNorm, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var grpOld = await _db.Groups.FirstOrDefaultAsync(g => g.Name == oldGroupNorm);
                        if (grpOld != null)
                        {
                            var grpNew = await _db.Groups.FirstOrDefaultAsync(g => g.Name == newGroupNorm);
                            if (grpNew == null)
                            {
                                // simple rename
                                grpOld.Name = newGroupNorm;
                                await _db.SaveChangesAsync();

                                // keep string-based tables in sync to avoid duplicate names in dropdowns
                                var legacyAssoc = await _db.PromptTemplateGroups
                                                           .Where(pg => pg.Group != null && pg.Group.Trim() == oldGroupNorm)
                                                           .ToListAsync();
                                foreach (var a in legacyAssoc) a.Group = newGroupNorm;
                                var asstLegacy = await _db.AssistantGroups
                                                           .Where(ag => ag.Group != null && ag.Group.Trim() == oldGroupNorm)
                                                           .ToListAsync();
                                foreach (var a in asstLegacy) a.Group = newGroupNorm;
                                var owns = await _db.DozentGroupOwnerships.Where(o => o.Group == oldGroupNorm).ToListAsync();
                                foreach (var o in owns) o.Group = newGroupNorm;
                                var mems = await _db.UserGroupMemberships.Where(m => m.Group == oldGroupNorm).ToListAsync();
                                foreach (var m in mems) m.Group = newGroupNorm;
                                await _db.SaveChangesAsync();
                            }
                            else
                            {
                                // merge: move associations to grpNew and drop grpOld
                                var assoc = await _db.PromptTemplateGroups
                                                     .Where(pg => pg.GroupId == grpOld.Id)
                                                     .ToListAsync();
                                foreach (var a in assoc)
                                {
                                    a.GroupId = grpNew.Id;
                                }
                                // update legacy names for consistency
                                var legacyAssoc = await _db.PromptTemplateGroups
                                                           .Where(pg => pg.Group != null && pg.Group.Trim() == oldGroupNorm)
                                                           .ToListAsync();
                                foreach (var a in legacyAssoc)
                                {
                                    a.Group = newGroupNorm;
                                }
                                // assistants: reassign by id and update legacy
                                var asstById = await _db.AssistantGroups.Where(ag => ag.GroupId == grpOld.Id).ToListAsync();
                                foreach (var a in asstById) a.GroupId = grpNew.Id;
                                var asstLegacy = await _db.AssistantGroups.Where(ag => ag.Group != null && ag.Group.Trim() == oldGroupNorm).ToListAsync();
                                foreach (var a in asstLegacy) a.Group = newGroupNorm;
                                // optional: update other string-based tables to avoid confusion in dropdowns
                                var owns = await _db.DozentGroupOwnerships.Where(o => o.Group == oldGroupNorm).ToListAsync();
                                foreach (var o in owns) o.Group = newGroupNorm;
                                var mems = await _db.UserGroupMemberships.Where(m => m.Group == oldGroupNorm).ToListAsync();
                                foreach (var m in mems) m.Group = newGroupNorm;

                                await _db.SaveChangesAsync();

                                try
                                {
                                    _db.Groups.Remove(grpOld);
                                    await _db.SaveChangesAsync();
                                }
                                catch { /* ignore if constrained */ }
                            }
                        }
                        else
                        {
                            // No old group row present → nothing to rename. Keep data consistent in string tables.
                            var legacyAssoc = await _db.PromptTemplateGroups
                                                       .Where(pg => pg.Group != null && pg.Group.Trim() == oldGroupNorm)
                                                       .ToListAsync();
                            foreach (var a in legacyAssoc) a.Group = newGroupNorm;
                            var asstLegacy = await _db.AssistantGroups
                                                       .Where(ag => ag.Group != null && ag.Group.Trim() == oldGroupNorm)
                                                       .ToListAsync();
                            foreach (var a in asstLegacy) a.Group = newGroupNorm;
                            var owns = await _db.DozentGroupOwnerships.Where(o => o.Group == oldGroupNorm).ToListAsync();
                            foreach (var o in owns) o.Group = newGroupNorm;
                            var mems = await _db.UserGroupMemberships.Where(m => m.Group == oldGroupNorm).ToListAsync();
                            foreach (var m in mems) m.Group = newGroupNorm;
                            await _db.SaveChangesAsync();

                            // Also ensure a Groups row exists for the new name so future publishes won't create a new Group implicitly
                            var ensure = await _db.Groups.FirstOrDefaultAsync(g => g.Name == newGroupNorm);
                            if (ensure == null)
                            {
                                _db.Groups.Add(new Group { Name = newGroupNorm });
                                await _db.SaveChangesAsync();
                            }
                        }
                    }
                    catch { /* best effort rename */ }
                }
                TempData["success"] = "Code aktualisiert.";
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ent = await _db.RegistrationCodes.FindAsync(id);
            if (ent != null)
            {
                if (User.IsInRole("Dozent") && !User.IsInRole("SuperAdmin"))
                {
                    var owned = await OwnedGroupsAsync();
                    if (owned.Count == 0 || ent.Group == null || !owned.Contains(ent.Group)) return Forbid();
                }
                _db.RegistrationCodes.Remove(ent);
                await _db.SaveChangesAsync();
                TempData["success"] = "Code gelöscht.";
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Aktive/​Inaktive umschalten
        /// </summary>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            var ent = await _db.RegistrationCodes.FindAsync(id);
            if (ent != null)
            {
                if (User.IsInRole("Dozent") && !User.IsInRole("SuperAdmin"))
                {
                    var owned = await OwnedGroupsAsync();
                    if (owned.Count == 0 || ent.Group == null || !owned.Contains(ent.Group)) return Forbid();
                }
                ent.IsActive = !ent.IsActive;
                await _db.SaveChangesAsync();
                TempData["success"] = ent.IsActive
                    ? "Code aktiviert."
                    : "Code deaktiviert.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
