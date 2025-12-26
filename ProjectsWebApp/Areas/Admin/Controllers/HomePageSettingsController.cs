using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;

namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomePageSettingsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomePageSettingsController(ApplicationDbContext db) => _db = db;

        // GET: Admin/HomePageSettings
        public async Task<IActionResult> Index()
        {
            return View(await _db.HomePageSettings.ToListAsync());
        }

        // GET: Admin/HomePageSettings/Details/1
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var settings = await _db.HomePageSettings.FindAsync(id);
            if (settings == null) return NotFound();
            return View(settings);
        }

        // GET: Admin/HomePageSettings/Create
        public IActionResult Create() => View();

        // GET: Admin/HomePageSettings/Upsert
        public async Task<IActionResult> Upsert()
        {
            // we have exactly one settings row with Id=1
            var settings = await _db.HomePageSettings
                                   .Include(h => h.Modes)
                                   .Include(h => h.Features)
                                   .FirstOrDefaultAsync(h => h.Id == 1);

            if (settings == null)
            {
                settings = new HomePageSettings { Id = 1 };
                _db.HomePageSettings.Add(settings);
                await _db.SaveChangesAsync();
            }

            // ensure child collections are ordered
            settings.Modes = settings.Modes.OrderBy(m => m.Id).ToList();
            settings.Features = settings.Features.OrderBy(f => f.Id).ToList();

            return View(settings);
        }

        // POST: Admin/HomePageSettings/Upsert
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(HomePageSettings model)
        {
            if (!ModelState.IsValid) return View(model);

            // always modify the one record
            _db.HomePageSettings.Update(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Einstellungen gespeichert!";
            return RedirectToAction(nameof(Upsert));
        }

        // POST: Admin/HomePageSettings/DeleteMode/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMode(int id)
        {
            var mode = await _db.ModeCards.FindAsync(id);
            if (mode != null)
            {
                _db.ModeCards.Remove(mode);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Upsert));
        }

        // POST: Admin/HomePageSettings/DeleteFeature/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFeature(int id)
        {
            var feat = await _db.FeatureCards.FindAsync(id);
            if (feat != null)
            {
                _db.FeatureCards.Remove(feat);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Upsert));
        }
    }
}
