// Areas/Admin/Controllers/LandingController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using ProjectsWebApp.Models.Landing;
using ProjectsWebApp.Models.ViewModels;

// Areas/Admin/Controllers/LandingPageController.cs
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class LandingItemController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    // Type mappings for display and route
    private static readonly Dictionary<string, string> RouteTypeDisplayNames = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Text", "Text" },
        { "Bild", "Bild" },
        { "Video", "Video" },
        { "Sound", "Sound" },
        { "Beruf", "KI-Agent" },
        { "Meta", "Meta" },
        { "Eigenfilter", "Szenarien" },
        { "Framework", "Framework" },
        { "Bildung", "Domäne" }
    };

    public LandingItemController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new LandingPageVM
        {
            Hero = await _db.Heroes.FirstOrDefaultAsync(),
            Modes = await _db.Modes.OrderBy(m => m.SortOrder).ToListAsync(),
            Features = await _db.Features.OrderBy(f => f.SortOrder).ToListAsync()
        };

        ViewBag.RouteTypeDisplayNames = RouteTypeDisplayNames;
        return View(vm);
    }

    // ==================== API Endpoints for Modal Editing ====================

    [HttpGet]
    public async Task<IActionResult> GetMode(int id)
    {
        var mode = await _db.Modes.FindAsync(id);
        if (mode == null) return NotFound();
        return Json(new
        {
            mode.Id,
            mode.Title,
            mode.IconClass,
            mode.ImageUrl,
            mode.RouteType,
            mode.SortOrder
        });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateMode([FromForm] int id, [FromForm] string title, [FromForm] string iconClass,
        [FromForm] string routeType, [FromForm] int sortOrder, IFormFile? imageFile)
    {
        var mode = await _db.Modes.FindAsync(id);
        if (mode == null) return Json(new { success = false, message = "Mode nicht gefunden" });

        mode.Title = title;
        mode.IconClass = iconClass;
        mode.RouteType = routeType;
        mode.SortOrder = sortOrder;

        if (imageFile != null && imageFile.Length > 0)
        {
            mode.ImageUrl = await SaveFileAsync(imageFile);
        }

        await _db.SaveChangesAsync();
        return Json(new { success = true, imageUrl = mode.ImageUrl });
    }

    [HttpGet]
    public async Task<IActionResult> GetHero()
    {
        var hero = await _db.Heroes.FirstOrDefaultAsync();
        if (hero == null) return NotFound();
        return Json(new
        {
            hero.Id,
            hero.Title,
            hero.Lead,
            hero.BackgroundUrl
        });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateHero([FromForm] int id, [FromForm] string title, [FromForm] string lead, IFormFile? backgroundFile)
    {
        var hero = await _db.Heroes.FindAsync(id);
        if (hero == null)
        {
            hero = new Hero();
            _db.Heroes.Add(hero);
        }

        hero.Title = title;
        hero.Lead = lead;

        if (backgroundFile != null && backgroundFile.Length > 0)
        {
            hero.BackgroundUrl = await SaveFileAsync(backgroundFile);
        }

        await _db.SaveChangesAsync();
        return Json(new { success = true, backgroundUrl = hero.BackgroundUrl });
    }

    [HttpGet]
    public async Task<IActionResult> GetFeature(int id)
    {
        var feature = await _db.Features.FindAsync(id);
        if (feature == null) return NotFound();
        return Json(new
        {
            feature.Id,
            feature.Title,
            feature.IconClass,
            feature.Description,
            feature.SortOrder
        });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateFeature([FromForm] int id, [FromForm] string title, [FromForm] string iconClass,
        [FromForm] string description, [FromForm] int sortOrder)
    {
        var feature = await _db.Features.FindAsync(id);
        if (feature == null) return Json(new { success = false, message = "Feature nicht gefunden" });

        feature.Title = title;
        feature.IconClass = iconClass;
        feature.Description = description;
        feature.SortOrder = sortOrder;

        await _db.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMode([FromForm] int id)
    {
        var mode = await _db.Modes.FindAsync(id);
        if (mode == null) return Json(new { success = false, message = "Mode nicht gefunden" });

        _db.Modes.Remove(mode);
        await _db.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteFeature([FromForm] int id)
    {
        var feature = await _db.Features.FindAsync(id);
        if (feature == null) return Json(new { success = false, message = "Feature nicht gefunden" });

        _db.Features.Remove(feature);
        await _db.SaveChangesAsync();
        return Json(new { success = true });
    }

    private async Task EnsureDefaultModesAsync()
    {
        // Ensure all nine Prompt-Arten exist as Modes
        var requiredModes = new[]
        {
            new { Title = "Text",      RouteType = "Text",      IconClass = "bi bi-fonts" },
            new { Title = "Bild",      RouteType = "Bild",      IconClass = "bi bi-image" },
            new { Title = "Video",     RouteType = "Video",     IconClass = "bi bi-play-btn" },
            new { Title = "Sound",     RouteType = "Sound",     IconClass = "bi bi-volume-up" },

            // Localized labels for underlying PromptType values
            new { Title = "Domäne",   RouteType = "Bildung",   IconClass = "bi bi-bank" },
            new { Title = "KI-Agent", RouteType = "Beruf",     IconClass = "bi bi-robot" },
            new { Title = "Szenarien",RouteType = "Eigenfilter",IconClass = "bi bi-stars" },
            new { Title = "Framework",RouteType = "Framework", IconClass = "bi bi-diagram-3" },
            new { Title = "Meta",     RouteType = "Meta",      IconClass = "bi bi-layers" }
        };

        var existingRouteTypes = await _db.Modes
            .Select(m => m.RouteType)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToListAsync();

        var existingSet = existingRouteTypes.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var hasAny = await _db.Modes.AnyAsync();
        var currentMaxSortOrder = hasAny
            ? await _db.Modes.MaxAsync(m => m.SortOrder)
            : 0;

        var anyAdded = false;

        foreach (var def in requiredModes)
        {
            if (!existingSet.Contains(def.RouteType))
            {
                currentMaxSortOrder++;
                var mode = new Mode
                {
                    Title = def.Title,
                    IconClass = def.IconClass,
                    ImageUrl = string.Empty,
                    RouteType = def.RouteType,
                    SortOrder = currentMaxSortOrder
                };

                _db.Modes.Add(mode);
                anyAdded = true;
            }
        }

        if (anyAdded)
        {
            await _db.SaveChangesAsync();
        }
    }

    private async Task<string> SaveFileAsync(IFormFile file)
    {
        var uploadsDir = Path.Combine(_env.WebRootPath, "images", "uploads");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/images/uploads/{fileName}";
    }
}