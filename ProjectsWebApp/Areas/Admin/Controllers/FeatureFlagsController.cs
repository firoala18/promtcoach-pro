using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using ProjectsWebApp.Utility;

namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public sealed class FeatureFlagsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        public FeatureFlagsController(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // load latest persisted value; fall back to current static flag
            var rec = _db.PromptFeatureSettings
                         .OrderByDescending(x => x.UpdatedAt)
                         .FirstOrDefault();
            var vm = new PromptFeatureSetting
            {
                EnableFilterGeneration = rec?.EnableFilterGeneration ?? FeatureFlags.EnableFilterGeneration,
                EnableSmartSelection   = rec?.EnableSmartSelection   ?? FeatureFlags.EnableSmartSelection
            };
            // Prompt Technique generation currently only in-memory (no DB column)
            ViewBag.EnablePromptTechnique = FeatureFlags.EnablePromptTechnique;
            // Analytics tracking toggle is purely in-memory (no DB persistence)
            ViewBag.EnableAnalytics = FeatureFlags.EnableAnalytics;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Set(bool enableFilterGeneration, bool enableSmartSelection, bool enablePromptTechnique, bool enableAnalytics)
        {
            // upsert record
            var rec = _db.PromptFeatureSettings
                         .OrderByDescending(x => x.UpdatedAt)
                         .FirstOrDefault();
            if (rec == null)
            {
                rec = new PromptFeatureSetting();
                _db.PromptFeatureSettings.Add(rec);
            }
            rec.EnableFilterGeneration = enableFilterGeneration;
            rec.EnableSmartSelection   = enableSmartSelection;
            rec.UpdatedAt = DateTime.UtcNow;
            rec.UpdatedByUserId = _userManager.GetUserId(User);
            _db.SaveChanges();

            // update static flag for current process
            FeatureFlags.EnableFilterGeneration = enableFilterGeneration;
            FeatureFlags.EnableSmartSelection   = enableSmartSelection;
            FeatureFlags.EnablePromptTechnique  = enablePromptTechnique;
            FeatureFlags.EnableAnalytics        = enableAnalytics;

            TempData["success"] = "Feature-Flags gespeichert.";
            return RedirectToAction(nameof(Index));
        }
    }
}
