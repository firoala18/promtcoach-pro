using Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.Models;
using ProjectsWebApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectsWebApp.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class FilterCategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        private const string Secret = "uni-wuppertal2025";

        private static readonly JsonSerializerOptions HashSerializationOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        // Helper: map PromptType enum to display label for filenames and UI
        private static string GetTypeLabel(PromptType pt) => pt switch
        {
            PromptType.Eigenfilter => "Szenarien",
            PromptType.Framework => "Framework",
            PromptType.Meta => "Meta",
            _ => pt.ToString()
        };

        public FilterCategoryController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Visibility(string? type = null)
        {
            var promptType = Enum.TryParse<PromptType>(type, true, out var parsed)
                             ? parsed
                             : PromptType.Text;

            var userId = _userManager.GetUserId(User)!;

            var categories = _unitOfWork.FilterCategory
                .GetAll(includeProperties: "FilterItems")
                .Where(c => c.Type == promptType
                            && (c.UserId == null || c.UserId == "system")
                            && !c.IsHidden)
                .OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name)
                .ToList();

            var visRepo = _unitOfWork.GetRepository<UserFilterCategoryVisibility>();
            var hiddenIds = visRepo
                .GetAll()
                .Where(p => p.UserId == userId && p.IsHidden)
                .Select(p => p.FilterCategoryId)
                .ToHashSet();

            ViewBag.Type = promptType;
            ViewBag.HiddenCategoryIds = hiddenIds;
            return View("Visibility", categories);
        }

        // GET: /User/FilterCategory?type=Text
        public IActionResult Index(string? type = null)
        {
            var promptType = Enum.TryParse<PromptType>(type, true, out var parsed)
                             ? parsed
                             : PromptType.Eigenfilter;
            var userId = _userManager.GetUserId(User)!;

            // Eigenfilter, Framework und Meta teilen sich die gruppierte Eigenfilter-Ansicht
            if (promptType == PromptType.Eigenfilter
                || promptType == PromptType.Framework
                || promptType == PromptType.Meta)
            {
                var cats = _unitOfWork.FilterCategory
                    .GetAll(includeProperties: "FilterItems")
                    .Where(c => c.UserId == userId && c.Type == promptType)
                    .OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name)
                    .ToList();

                var vm = new EigenfilterIndexVM
                {
                    Type = promptType,
                    Manual = cats.Where(c => !c.IsAiGenerated),
                    Ai = cats.Where(c => c.IsAiGenerated)
                };

                ViewBag.Type = promptType;
                return View("IndexEigenfilter", vm);
            }

            // Non-Eigenfilter / non-Framework unchanged
            var categories = _unitOfWork.FilterCategory
                .GetAll(includeProperties: "FilterItems")
                .Where(c => c.UserId == userId && c.Type == promptType)
                .OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name)
                .ToList();

            ViewBag.Type = promptType;
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleUserVisibility(int id, string? type = null)
        {
            var userId = _userManager.GetUserId(User)!;

            var category = _unitOfWork.FilterCategory
                .GetFirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();

            if (!string.IsNullOrEmpty(category.UserId) && category.UserId != "system")
                return Forbid();

            var repo = _unitOfWork.GetRepository<UserFilterCategoryVisibility>();
            var pref = repo.GetFirstOrDefault(p => p.UserId == userId && p.FilterCategoryId == id);

            if (pref == null)
            {
                pref = new UserFilterCategoryVisibility
                {
                    UserId = userId,
                    FilterCategoryId = id,
                    IsHidden = true,
                    CreatedAt = DateTime.UtcNow
                };
                repo.Add(pref);
            }
            else
            {
                pref.IsHidden = !pref.IsHidden;
                repo.Update(pref);
            }

            _unitOfWork.Save();

            var t = type ?? category.Type.ToString();
            return RedirectToAction(nameof(Visibility), new { type = t });
        }

        // GET: /User/FilterCategory/Upsert/{id?}
        public IActionResult Upsert(int? id, PromptType? type = null)
        {
            var userId = _userManager.GetUserId(User)!;
            FilterCategory category;

            if (id == null)
            {
                category = new FilterCategory
                {
                    UserId = userId,
                    Type = type ?? PromptType.Eigenfilter,
                    DisplayOrder = 0,
                    FilterItems = new List<FilterItem>()
                };
            }
            else
            {
                category = _unitOfWork.FilterCategory
                    .GetFirstOrDefault(c => c.Id == id && c.UserId == userId,
                                       includeProperties: "FilterItems")
                    ?? throw new InvalidOperationException("Kategorie nicht gefunden oder kein Zugriff");
            }

            ViewBag.Type = category.Type;
            return View(category);
        }

        // POST: /User/FilterCategory/Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(FilterCategory category)
        {
            var userId = _userManager.GetUserId(User)!;

            if (!ModelState.IsValid)
            {
                ViewBag.Type = category.Type;
                return View(category);
            }

            category.UserId = userId;

            if (category.Id == 0)
            {
                _unitOfWork.FilterCategory.Add(category);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index), new { type = category.Type });
            }

            var dbCat = _unitOfWork.FilterCategory
                           .GetFirstOrDefault(c => c.Id == category.Id && c.UserId == userId,
                                              includeProperties: "FilterItems")
                        ?? throw new InvalidOperationException("Kategorie nicht gefunden oder kein Zugriff");

            dbCat.Name = category.Name;
            dbCat.Type = category.Type;
            dbCat.DisplayOrder = category.DisplayOrder;
            dbCat.ItemSortMode = category.ItemSortMode; // persist chosen sort mode

            foreach (var posted in category.FilterItems.OrderBy(i => i.SortOrder))
            {
                if (posted.Id == 0)
                    dbCat.FilterItems.Add(posted);
                else
                {
                    var exist = dbCat.FilterItems.First(f => f.Id == posted.Id);
                    exist.Title = posted.Title;
                    exist.Info = posted.Info;
                    exist.Instruction = posted.Instruction;
                    exist.SortOrder = posted.SortOrder;
                }
            }

            var postedIds = category.FilterItems.Select(i => i.Id).ToHashSet();
            var toDelete = dbCat.FilterItems.Where(i => !postedIds.Contains(i.Id)).ToList();
            foreach (var del in toDelete)
                _unitOfWork.FilterItem.Remove(del);

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index), new { type = category.Type });
        }

        // ========= Export (generic, non-Eigenfilter) =========
        [HttpGet]
        public IActionResult Export(string? type = null)
        {
            var promptType = Enum.TryParse(type, true, out PromptType tmp)
                                ? tmp
                                : PromptType.Eigenfilter;

            var userId = _userManager.GetUserId(User)!;

            var dtoList = _unitOfWork.FilterCategory
                .GetAll(includeProperties: "FilterItems")
                .Where(c => c.UserId == userId && c.Type == promptType)
                .OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name)
                .Select(c => new FilterCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    DisplayOrder = c.DisplayOrder,
                    FilterItems = c.FilterItems
                                     .OrderBy(i => i.SortOrder)
                                     .Select(i => new FilterItemDto
                                     {
                                         Id = i.Id,
                                         Title = i.Title,
                                         Info = i.Info,
                                         Instruction = i.Instruction,
                                         SortOrder = i.SortOrder
                                     })
                                     .ToList()
                })
                .ToList();

            var label = GetTypeLabel(promptType);
            var payloadJson = BuildSignedPayload(dtoList, label + "_MeineFilter.json", out var fileName);
            return File(Encoding.UTF8.GetBytes(payloadJson), "application/json", fileName);
        }

        // ========= NEW: Export only the current Eigenfilter/Framework group (manual|ai) =========
        [HttpGet]
        public IActionResult ExportEigenfilter(string group = "manual", string? type = null)
        {
            var userId = _userManager.GetUserId(User)!;
            bool isAi = string.Equals(group, "ai", StringComparison.OrdinalIgnoreCase);

            var promptType = Enum.TryParse(type, true, out PromptType parsed)
                                ? parsed
                                : PromptType.Eigenfilter;

            var dtoList = _unitOfWork.FilterCategory
                .GetAll(includeProperties: "FilterItems")
                .Where(c => c.UserId == userId
                         && c.Type == promptType
                         && c.IsAiGenerated == isAi)
                .OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name)
                .Select(c => new FilterCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    DisplayOrder = c.DisplayOrder,
                    FilterItems = c.FilterItems
                                     .OrderBy(i => i.SortOrder)
                                     .Select(i => new FilterItemDto
                                     {
                                         Id = i.Id,
                                         Title = i.Title,
                                         Info = i.Info,
                                         Instruction = i.Instruction,
                                         SortOrder = i.SortOrder
                                     })
                                     .ToList()
                })
                .ToList();

            var label = GetTypeLabel(promptType);
            var title = isAi ? label + "_KI" : label + "_Eigene";
            var payloadJson = BuildSignedPayload(dtoList, title + ".json", out var fileName);
            return File(Encoding.UTF8.GetBytes(payloadJson), "application/json", fileName);
        }

        // ========= Import (allows cross-type import) =========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file, string? type = null)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ImportError"] = "Bitte eine JSON-Datei auswählen.";
                return RedirectToAction(nameof(Index), new { type });
            }

            string json;
            using (var sr = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
                json = await sr.ReadToEndAsync();
            if (json.Length > 0 && json[0] == '\uFEFF') json = json[1..];

            FilterCategoriesPayloadDto? payload;
            try
            {
                payload = JsonSerializer.Deserialize<FilterCategoriesPayloadDto>(
                              json,
                              new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                TempData["ImportError"] = "Ungültiges JSON-Format.";
                return RedirectToAction(nameof(Index), new { type });
            }

            var dataJson = JsonSerializer.Serialize(payload.Data, HashSerializationOptions);
            using var sha = SHA256.Create();
            var expectedSig = Convert.ToHexString(
                                  sha.ComputeHash(Encoding.UTF8.GetBytes(Secret + dataJson)))
                              .ToLowerInvariant();

            if (!string.Equals(expectedSig, payload.Hash, StringComparison.Ordinal))
            {
                TempData["ImportError"] = "Integritätsprüfung fehlgeschlagen.";
                return RedirectToAction(nameof(Index), new { type });
            }

            var userId = _userManager.GetUserId(User)!;
            var dtoToCat = new Dictionary<FilterCategoryDto, FilterCategory>();

            // Determine target type: use URL parameter if provided, otherwise use type from JSON
            // This allows users to import categories from one type (e.g., Szenarien) into another (e.g., Framework)
            var targetType = Enum.TryParse<PromptType>(type, true, out var parsedType)
                             ? parsedType
                             : (PromptType?)null;

            foreach (var dto in payload.Data)
            {
                dto.FilterItems ??= new List<FilterItemDto>();

                // Use target type from URL if specified, otherwise use type from JSON
                var effectiveType = targetType ?? dto.Type;

                var cat = _unitOfWork.FilterCategory
                           .GetFirstOrDefault(c => c.Id == dto.Id && c.UserId == userId && c.Type == effectiveType)
                    ?? _unitOfWork.FilterCategory
                           .GetFirstOrDefault(c => c.UserId == userId &&
                                                   c.Name == dto.Name &&
                                                   c.Type == effectiveType)
                    ?? new FilterCategory { UserId = userId };

                cat.Name = dto.Name ?? "(ohne Namen)";
                cat.Type = effectiveType;
                cat.DisplayOrder = dto.DisplayOrder;

                if (cat.Id == 0)
                    _unitOfWork.FilterCategory.Add(cat);

                dtoToCat[dto] = cat;
            }
            _unitOfWork.Save();

            foreach (var (dto, cat) in dtoToCat)
            {
                var existing = _unitOfWork.FilterItem
                                 .GetAll()
                                 .Where(i => i.FilterCategoryId == cat.Id)
                                 .ToList();

                foreach (var itm in dto.FilterItems.OrderBy(i => i.SortOrder))
                {
                    if (itm == null) continue;

                    var item =
                        (itm.Id.HasValue ? existing.FirstOrDefault(x => x.Id == itm.Id) : null)
                        ?? existing.FirstOrDefault(x => x.Title == itm.Title &&
                                                        x.SortOrder == itm.SortOrder)
                        ?? new FilterItem { FilterCategoryId = cat.Id };

                    if (item.Id == 0)
                        _unitOfWork.FilterItem.Add(item);

                    item.Title = itm.Title;
                    item.Info = itm.Info;
                    item.Instruction = itm.Instruction;
                    item.SortOrder = itm.SortOrder;
                }

                var keepIds = dto.FilterItems
                                 .Where(i => i?.Id is int id && id != 0)
                                 .Select(i => i!.Id!.Value)
                                 .ToHashSet();

                foreach (var orphan in existing.Where(i => !keepIds.Contains(i.Id)))
                    _unitOfWork.FilterItem.Remove(orphan);
            }

            _unitOfWork.Save();
            TempData["ImportSuccess"] = "Import erfolgreich durchgeführt.";
            return RedirectToAction(nameof(Index), new { type });
        }

        // ========= Delete only current TYPE (non-Eigenfilter use) =========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteType(string? type = null)
        {
            var promptType = Enum.TryParse<PromptType>(type, true, out var parsed)
                             ? parsed : PromptType.Text;
            var userId = _userManager.GetUserId(User)!;

            var cats = _unitOfWork.FilterCategory
                        .GetAll(includeProperties: "FilterItems")
                        .Where(c => c.UserId == userId && c.Type == promptType)
                        .ToList();

            foreach (var c in cats) _unitOfWork.FilterCategory.Remove(c);
            _unitOfWork.Save();

            var label = GetTypeLabel(promptType);
            TempData["DeleteSuccess"] = $"Alle Kategorien vom Typ \"{label}\" wurden gelöscht.";
            return RedirectToAction(nameof(Index), new { type = promptType });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAll()
        {
            var userId = _userManager.GetUserId(User)!;
            var cats = _unitOfWork.FilterCategory
                          .GetAll(includeProperties: "FilterItems")
                          .Where(c => c.UserId == userId)
                          .ToList();

            foreach (var c in cats) _unitOfWork.FilterCategory.Remove(c);
            _unitOfWork.Save();

            TempData["DeleteSuccess"] = "Alle Kategorien wurden gelöscht.";
            return RedirectToAction(nameof(Index));
        }

        // ========= NEW: Delete only the current Eigenfilter/Framework group (manual|ai) =========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteEigenfilterGroup(string group = "manual", string? type = null)
        {
            var userId = _userManager.GetUserId(User)!;
            bool isAi = string.Equals(group, "ai", StringComparison.OrdinalIgnoreCase);

            var promptType = Enum.TryParse(type, true, out PromptType parsed)
                                ? parsed
                                : PromptType.Eigenfilter;

            var cats = _unitOfWork.FilterCategory
                        .GetAll(includeProperties: "FilterItems")
                        .Where(c => c.UserId == userId
                                 && c.Type == promptType
                                 && c.IsAiGenerated == isAi)
                        .ToList();

            foreach (var c in cats) _unitOfWork.FilterCategory.Remove(c);
            _unitOfWork.Save();

            var label = GetTypeLabel(promptType);
            TempData["DeleteSuccess"] = isAi
                ? $"Alle KI-generierten {label}-Filter wurden gelöscht."
                : $"Alle eigenen {label}-Filter wurden gelöscht.";

            return RedirectToAction(nameof(Index), new { type = promptType });
        }

        // ========= Delete single category (AJAX) =========
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var cat = _unitOfWork.FilterCategory
                      .GetFirstOrDefault(c => c.Id == id && c.UserId == userId);
            if (cat == null)
                return Json(new { success = false });

            _unitOfWork.FilterCategory.Remove(cat);
            _unitOfWork.Save();
            return Json(new { success = true });
        }

        // ===== helper =====
        private string BuildSignedPayload(List<FilterCategoryDto> dtoList, string suggestedFile, out string fileName)
        {
            var dataJson = JsonSerializer.Serialize(dtoList, HashSerializationOptions);
            using var sha = SHA256.Create();
            var sig = Convert.ToHexString(
                          sha.ComputeHash(Encoding.UTF8.GetBytes(Secret + dataJson)))
                      .ToLowerInvariant();

            var payloadJson = JsonSerializer.Serialize(
                new FilterCategoriesPayloadDto { Data = dtoList, Hash = sig },
                new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            fileName = suggestedFile;
            return payloadJson;
        }
    }
}
