using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;

namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class ContactMessageSettingsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ContactMessageSettingsController(ApplicationDbContext db) => _db = db;

        // GET: /Admin/ContactMessageSettings/Edit
        public async Task<IActionResult> Edit()
        {
            var setting = await _db.ContactMessageSettings.FindAsync(1);
            if (setting == null) return NotFound();
            return View(setting);
        }

        // POST: /Admin/ContactMessageSettings/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContactMessageSetting model)
        {
            if (!ModelState.IsValid) return View(model);

            // 1) Datensatz mit richtiger Id laden
            var setting = await _db.ContactMessageSettings.FindAsync(model.Id);
            if (setting is null) return NotFound();

            // 2) Nur die Felder überschreiben, die tatsächlich editierbar sind
            setting.Message = model.Message;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Kontakt‑Nachricht wurde gespeichert.";
            return RedirectToAction(nameof(Edit));
        }

    }
}
