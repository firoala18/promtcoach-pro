using Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.RegularExpressions;
using ProjectsWebApp.DataAccsess.Services.Interfaces;

namespace ProjectsWebApp.Areas.User.Controllers
{
    [Authorize]
    [Area("User")]
    public class MyPromptsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userMgr;
        private readonly IWebHostEnvironment _env;
        private readonly IUserActivityLogger _activityLogger;

        public MyPromptsController(ApplicationDbContext db,
                                   UserManager<IdentityUser> userMgr,
                                   IWebHostEnvironment env,
                                   IUserActivityLogger activityLogger)
        {
            _db = db;
            _userMgr = userMgr;
            _env = env;
            _activityLogger = activityLogger;
        }

        // GET: /User/MyPrompts
        public async Task<IActionResult> Index()
        {
            var uid = _userMgr.GetUserId(User);
            var list = await _db.SavedPrompts
                                .Where(p => p.UserId == uid)
                                .OrderByDescending(p => p.CreatedAt)
                                .ToListAsync();
            return View(list);
        }

        //  POST  /User/MyPrompts/UploadImage
        [HttpPost]
        public async Task<IActionResult> UploadImage(int id, IFormFile imageFile)
        {
            var uid = _userMgr.GetUserId(User);
            var prompt = await _db.SavedPrompts.FirstOrDefaultAsync(p => p.Id == id && p.UserId == uid);
            if (prompt == null) return NotFound();

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var physPath = Path.Combine(_env.WebRootPath, "uploads", $"user_{uid}", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(physPath)!);
            using var fs = System.IO.File.Create(physPath);
            await imageFile.CopyToAsync(fs);

            /*  !!!  hier mit  ~/  speichern  */
            prompt.GeneratedImagePath = $"~/uploads/user_{uid}/{fileName}";
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }


        // POST: /User/MyPrompts/DeleteImage/5
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var uid = _userMgr.GetUserId(User);
            var prompt = await _db.SavedPrompts.FirstOrDefaultAsync(p => p.Id == id && p.UserId == uid);
            if (prompt == null) return NotFound();
            if (!string.IsNullOrEmpty(prompt.GeneratedImagePath))
            {
                var phys = Path.Combine(_env.WebRootPath, prompt.GeneratedImagePath.TrimStart('/'));
                if (System.IO.File.Exists(phys)) System.IO.File.Delete(phys);
                prompt.GeneratedImagePath = null;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // NEW  A simple probe:  /User/MyPrompts/Exists?akronym=XYZ
        [HttpGet]
        public async Task<IActionResult> Exists(string akronym)
        {
            var uid = _userMgr.GetUserId(User);
            var exists = await _db.SavedPrompts
                                  .AnyAsync(p => p.UserId == uid &&
                                                 p.Akronym == akronym.Trim());
            return Json(new { exists });
        }


        // GET: /User/MyPrompts/Details/5
        // In the Details action
        public async Task<IActionResult> Details(int id)
        {
            var uid = _userMgr.GetUserId(User);
            var prompt = await _db.SavedPrompts
                                  .Include(s => s.PromptVariations)
                                  .SingleOrDefaultAsync(s => s.Id == id);
            if (prompt == null) return NotFound();

            ViewData["CurrentUserId"] = uid;
            ViewData["PromptUserId"] = prompt.UserId;

            return View(prompt);
        }



        // POST: /User/MyPrompts/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var uid = _userMgr.GetUserId(User);
            // finde die Variation inkl. Prompt, damit wir den User-Check machen können
            var variation = await _db.SavedPromptVariations
                                     .Include(v => v.SavedPrompt)
                                     .FirstOrDefaultAsync(v
                                         => v.Id == id
                                         && v.SavedPrompt.UserId == uid);
            if (variation == null)
                return NotFound();

            _db.SavedPromptVariations.Remove(variation);
            await _db.SaveChangesAsync();
            return Ok();
        }

        private static string MergeWithTechniqueTags(string? filterJson, string? promptHtml)
        {
            var items = new List<Dictionary<string, string>>();
            var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 1) existing items (FilterJson) – deduplicated by value
            if (!string.IsNullOrWhiteSpace(filterJson))
            {
                try
                {
                    using var doc = JsonDocument.Parse(filterJson);
                    if (doc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var el in doc.RootElement.EnumerateArray())
                        {
                            var val = el.TryGetProperty("value", out var v) ? (v.GetString() ?? string.Empty) : string.Empty;
                            var ins = el.TryGetProperty("instruction", out var i) ? (i.GetString() ?? val) : val;
                            if (string.IsNullOrWhiteSpace(val)) continue;

                            var key = val.Trim();
                            if (!existing.Add(key)) continue; // skip duplicates

                            items.Add(new Dictionary<string, string>
                            {
                                { "value", key },
                                { "instruction", ins.Trim() }
                            });
                        }
                    }
                }
                catch { }
            }

            // 2) extract techniques like (Role Prompting): from promptHtml
            var text = Regex.Replace(promptHtml ?? string.Empty, "<.*?>", string.Empty);
            var matches = Regex.Matches(text, "\\(([^)]+)\\)\\s*:");
            foreach (Match m in matches.Cast<Match>())
            {
                var name = (m.Groups[1].Value ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(name)) continue;
                if (existing.Add(name))
                    items.Add(new Dictionary<string, string> { { "value", name }, { "instruction", name } });
            }

            return JsonSerializer.Serialize(items);
        }

        // POST: /User/MyPrompts/Delete/5           (same route the JS already calls)
        [HttpPost]
        public async Task<IActionResult> DeletePrompt(int id)
        {
            var uid = _userMgr.GetUserId(User);

            // 1) load the prompt incl. its variations (for cascade or manual delete)
            var prompt = await _db.SavedPrompts
                                  .Include(p => p.PromptVariations)
                                  .FirstOrDefaultAsync(p => p.Id == id && p.UserId == uid);

            if (prompt == null) return NotFound();

            // 2) optional: remove the generated image from disk
            if (!string.IsNullOrWhiteSpace(prompt.GeneratedImagePath))
            {
                var phys = Path.Combine(_env.WebRootPath, prompt.GeneratedImagePath.TrimStart('/'));
                if (System.IO.File.Exists(phys)) System.IO.File.Delete(phys);
            }

            // 3) delete prompt → EF takes care of child variations if
            //    ON DELETE CASCADE is configured; otherwise remove them manually
            _db.SavedPrompts.Remove(prompt);
            await _db.SaveChangesAsync();

            return Ok();          // 200 => JS will remove the card + toast “Gelöscht!”
        }


        [HttpPost]
        public async Task<IActionResult> AddVariation([FromBody] AddSavedPromptVariationDto dto)
        {
            var uid = _userMgr.GetUserId(User);
            var prompt = await _db.SavedPrompts
                                  .FirstOrDefaultAsync(p => p.UserId == uid && p.Akronym == dto.Akronym);
            if (prompt == null)
                return NotFound(new { success = false, message = "Prompt nicht gefunden." });

            var variation = new SavedPromptVariation
            {
                SavedPromptId = prompt.Id,
                VariationJson = JsonSerializer.Serialize(dto.Data)
            };
            _db.SavedPromptVariations.Add(variation);
            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "Variation gespeichert." });
        }

        // Areas/User/Controllers/MyPromptsController.cs
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUsedModels(int id, string usedModels)
        {
            var uid = _userMgr.GetUserId(User);
            var prompt = await _db.SavedPrompts
                                  .FirstOrDefaultAsync(p => p.Id == id && p.UserId == uid);
            if (prompt == null) return NotFound();

            prompt.UsedModels = usedModels?.Trim();
            await _db.SaveChangesAsync();

            TempData["success"] = "Verwendete Modelle gespeichert.";
            return RedirectToAction(nameof(Details), new { id });
        }


        // POST: /User/MyPrompts/Save
        // UPDATED  “Save” now overwrites on duplicate instead of inserting twice
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SavedPrompt dto)
        {
            var uid = _userMgr.GetUserId(User);

            // 1) does a prompt with that Akronym already exist for this user?
            var entity = await _db.SavedPrompts
                                  .FirstOrDefaultAsync(p => p.UserId == uid &&
                                                            p.Akronym == dto.Akronym.Trim());

            if (entity == null)
            {
                // create new
                entity = new SavedPrompt
                {
                    UserId = uid,
                    Akronym = dto.Akronym.Trim(),
                    CreatedAt = DateTime.UtcNow
                };
                _db.SavedPrompts.Add(entity);
            }

            // 2) copy *all* mutable fields (both for new & overwrite)
            entity.Title = dto.Title;
            entity.Beschreibung = dto.Beschreibung;
            entity.Schluesselbegriffe = dto.Schluesselbegriffe;
            entity.Thema = dto.Thema;
            entity.Ziele = dto.Ziele;
            entity.PromptHtml = dto.PromptHtml;
            entity.PromptType = dto.PromptType;
            // Merge existing FilterJson with techniques parsed from the generated PromptHtml
            entity.FilterJson = MergeWithTechniqueTags(dto.FilterJson, dto.PromptHtml);
            entity.UsedModels = dto.UsedModels;
            entity.GeneratedImagePath = dto.GeneratedImagePath;
            entity.Temperatur = dto.Temperatur;
            entity.MaxZeichen = dto.MaxZeichen;
            entity.MetaHash = dto.MetaHash;

            await _db.SaveChangesAsync();

            // Analytics: prompt saved to personal collection (Sammlung)
            try
            {
                if (ProjectsWebApp.Utility.FeatureFlags.EnableAnalytics)
                {
                    string? groupName = null;
                    try
                    {
                        groupName = await _db.UserGroupMemberships
                            .Where(m => m.UserId == uid)
                            .OrderByDescending(m => m.CreatedAt)
                            .Select(m => m.Group)
                            .FirstOrDefaultAsync();
                    }
                    catch { }

                    await _activityLogger.LogAsync(
                        uid ?? string.Empty,
                        string.IsNullOrWhiteSpace(groupName) ? null : groupName!.Trim(),
                        "prompt_save_collection",
                        null,
                        new { dto.Akronym, dto.Title, dto.PromptType },
                        CancellationToken.None);
                }
            }
            catch { /* analytics must never break main flow */ }

            return Ok();
        }

        // SEE another user's collection (read‑only)
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> AdminList(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid)) return BadRequest();

            var targetUser = await _userMgr.FindByIdAsync(uid);
            if (targetUser == null) return NotFound();

            var list = await _db.SavedPrompts
                                .Where(p => p.UserId == uid)
                                .OrderByDescending(p => p.CreatedAt)
                                .ToListAsync();

            ViewBag.ReadOnly = true;          // flag for the view
            ViewBag.TargetUser = targetUser;

            return View("Index", list);         // reuse existing view
        }

        // Optional: details view for a single prompt
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> AdminDetails(int id)
        {
            var prompt = await _db.SavedPrompts
                                  .Include(p => p.PromptVariations)
                                  .SingleOrDefaultAsync(p => p.Id == id);
            if (prompt == null) return NotFound();

            ViewBag.ReadOnly = true;
            ViewBag.TargetUser = await _userMgr.FindByIdAsync(prompt.UserId);

            return View("Details", prompt);
        }


    }
}
