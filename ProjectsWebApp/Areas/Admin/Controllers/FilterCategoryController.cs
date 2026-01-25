using Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using ProjectsWebApp.Hubs;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class FilterCategoryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private const string Secret = "uni-wuppertal2025";
    private readonly UserManager<IdentityUser> _userMgr;
    private readonly ApplicationDbContext _db;
    private readonly ISemanticIndexService _semIndex;
    private readonly IHubContext<AdminImportHub> _importHub;

    // Add static serialization options
    private static readonly JsonSerializerOptions HashSerializationOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public FilterCategoryController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userMgr, ApplicationDbContext db, ISemanticIndexService semIndex, IHubContext<AdminImportHub> importHub)
    {
        _unitOfWork = unitOfWork;
        _userMgr = userMgr;
        _db = db;
        _semIndex = semIndex;
        _importHub = importHub;
    }

    // GET: Admin/FilterCategory
    public IActionResult Index(string? type = null)
    {
        var promptType = Enum.TryParse<PromptType>(type, true, out var parsed)
                         ? parsed
                         : PromptType.Text;

        var categories = _unitOfWork.FilterCategory
            .GetAll(includeProperties: "FilterItems")
            .Where(f => f.Type == promptType)
            .OrderBy(f => f.DisplayOrder)
            .ThenBy(f => f.Name)
            .ToList();

        ViewBag.Type = promptType;
        return View(categories);
    }

    // GET: Admin/FilterCategory/Upsert/{id?}
    public IActionResult Upsert(int? id, PromptType? type = null)
    {
        if (id == null)
        {
            return View(new FilterCategory
            {
                Type = type ?? PromptType.Text,
                DisplayOrder = 0,
                FilterItems = new List<FilterItem>()
            });
        }

        var category = _unitOfWork.FilterCategory
            .GetFirstOrDefault(c => c.Id == id, includeProperties: "FilterItems");
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ToggleVisibility(int id, string? type = null, string? panel = null)
    {
        var cat = _unitOfWork.FilterCategory.GetFirstOrDefault(c => c.Id == id);
        if (cat == null) return NotFound();
        cat.IsHidden = !cat.IsHidden;
        _unitOfWork.FilterCategory.Update(cat);
        _unitOfWork.Save();
        TempData["success"] = cat.IsHidden ? "Kategorie ausgeblendet." : "Kategorie eingeblendet.";
        return RedirectToAction(nameof(Index), new { type = (type ?? cat.Type.ToString()), panel });
    }

    // POST: Admin/FilterCategory/Upsert
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(FilterCategory category)
    {
        if (!ModelState.IsValid)
            return View(category);

        if (category.Id == 0)
        {
            _unitOfWork.FilterCategory.Add(category);
        }
        else
        {
            var dbCat = _unitOfWork.FilterCategory
                .GetFirstOrDefault(c => c.Id == category.Id, includeProperties: "FilterItems");
            if (dbCat == null) return NotFound();

            dbCat.Name = category.Name;
            dbCat.Type = category.Type;
            dbCat.DisplayOrder = category.DisplayOrder;
            dbCat.ItemSortMode = category.ItemSortMode;

            // add/update items
            foreach (var item in category.FilterItems.OrderBy(i => i.SortOrder))
            {
                if (item.Id == 0)
                    dbCat.FilterItems.Add(item);
                else
                {
                    var existing = dbCat.FilterItems.FirstOrDefault(f => f.Id == item.Id);
                    if (existing != null)
                    {
                        existing.Title = item.Title;
                        existing.Info = item.Info;
                        existing.Instruction = item.Instruction;
                        existing.SortOrder = item.SortOrder;
                    }
                }
            }

            // remove deleted items
            var keepIds = category.FilterItems.Select(i => i.Id).ToHashSet();
            var toRemove = dbCat.FilterItems
                                .Where(f => !keepIds.Contains(f.Id))
                                .ToList();
            foreach (var rem in toRemove)
                _unitOfWork.FilterItem.Remove(rem);
        }

        _unitOfWork.Save();

        // Directly (idempotently) embed all items of this category. The service will skip unchanged content.
        try
        {
            var savedCat = _unitOfWork.FilterCategory
                                      .GetFirstOrDefault(c => c.Id == category.Id, includeProperties: "FilterItems");
            var items = savedCat?.FilterItems ?? Enumerable.Empty<FilterItem>();
            var ct = HttpContext?.RequestAborted ?? default;
            foreach (var fi in items.DistinctBy(i => i.Id))
            {
                await _semIndex.UpsertFilterItemAsync(fi, group: null, ownerUserId: null, ct);
            }
        }
        catch { }

        return RedirectToAction(nameof(Index), new { type = category.Type });
    }

    // NEW – read‑only list of a student's Eigenfilter/Framework/Meta
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> AdminList(string uid, string? type = null)
    {
        if (string.IsNullOrWhiteSpace(uid))
            return BadRequest();

        var targetUser = await _userMgr.FindByIdAsync(uid);
        if (targetUser is null)
            return NotFound();

        // Parse the type parameter, default to Eigenfilter
        var promptType = Enum.TryParse<PromptType>(type, true, out var parsed)
                         ? parsed
                         : PromptType.Eigenfilter;

        // Only allow user-private types (Eigenfilter, Framework, Meta)
        var allowedTypes = new[] { PromptType.Eigenfilter, PromptType.Framework, PromptType.Meta };
        if (!allowedTypes.Contains(promptType))
            promptType = PromptType.Eigenfilter;

        // Get categories for the selected type
        var list = await _db.FilterCategories
                            .Include(c => c.FilterItems)
                            .Where(c => c.Type == promptType &&
                                        c.UserId == uid)
                            .OrderBy(c => c.DisplayOrder)
                            .ThenBy(c => c.Name)
                            .ToListAsync();

        ViewBag.ReadOnly = true;
        ViewBag.TargetUser = targetUser;
        ViewBag.TargetUserId = uid;
        ViewBag.Type = promptType;

        return View("AdminList", list);
    }


    // Helper: map PromptType enum to display label for filenames and UI
    private static string GetTypeLabel(PromptType pt) => pt switch
    {
        PromptType.Bildung => "Domäne",
        PromptType.Beruf => "KI-Agent",
        _ => pt.ToString()
    };

    [HttpGet]
    public IActionResult Export(string? type = null)
    {
        var promptType = Enum.TryParse<PromptType>(type, true, out var parsed)
                         ? parsed
                         : PromptType.Text;

        var cats = _unitOfWork.FilterCategory
            .GetAll(includeProperties: "FilterItems")
            .Where(c => c.Type == promptType)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToList();

        // 1) Build DTOs
        var dtoList = cats.Select(c => new FilterCategoryDto
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
                                SortOrder = i.SortOrder,
                                StableKey = ComputeStableKey(i.Title, i.Info, i.Instruction)
                            })
                            .ToList()
        }).ToList();

        // 2) Serialize with consistent options
        string dataJson = JsonSerializer.Serialize(dtoList, HashSerializationOptions);

        // 3) Compute SHA256(Secret + dataJson)
        using var sha = SHA256.Create();
        byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(Secret + dataJson));
        string hashHex = BitConverter.ToString(hashBytes)
                             .Replace("-", "")
                             .ToLowerInvariant();

        // 4) Wrap into payload
        var payload = new FilterCategoriesPayloadDto
        {
            Data = dtoList,
            Hash = hashHex
        };

        // 5) Pretty-print for download
        var fullJson = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var fileBytes = Encoding.UTF8.GetBytes(fullJson);
        var typeLabel = GetTypeLabel(promptType);
        var fileName = $"{typeLabel}_FilterCategories.json";

        return File(fileBytes, "application/json", fileName);
    }

    
    // POST: Admin/FilterCategory/Import
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(IFormFile file, string? type = null)
    {
        /* ─────────────────────────────────────────────
         * 0) Dateiprüfung
         * ────────────────────────────────────────────*/
        if (file == null || file.Length == 0)
        {
            TempData["ImportError"] = "Bitte eine JSON-Datei auswählen.";
            return RedirectToAction(nameof(Index), new { type });
        }

        /* ─────────────────────────────────────────────
         * 1) JSON einlesen
         * ────────────────────────────────────────────*/
        FilterCategoriesPayloadDto? payload;
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            var json = await reader.ReadToEndAsync();

            // ggf. UTF-8-BOM entfernen
            if (json.StartsWith("\uFEFF", StringComparison.Ordinal))
                json = json[1..];

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
        }

        if (payload?.Data == null)
        {
            TempData["ImportError"] = "Ungültiges Format.";
            return RedirectToAction(nameof(Index), new { type });
        }

        /* ─────────────────────────────────────────────
         * 2) Hash prüfen
         * ────────────────────────────────────────────*/
        var dataJson = JsonSerializer.Serialize(payload.Data, HashSerializationOptions);

        using var sha = SHA256.Create();
        var expectedHash = BitConverter.ToString(
                               sha.ComputeHash(Encoding.UTF8.GetBytes(Secret + dataJson)))
                           .Replace("-", "")
                           .ToLowerInvariant();

        if (expectedHash != payload.Hash)
        {
            TempData["ImportError"] = "Integritätsprüfung fehlgeschlagen. Die Datei wurde verändert.";
            return RedirectToAction(nameof(Index), new { type });
        }

        /* ─────────────────────────────────────────────
         * 3) Kategorien anlegen/aktualisieren
         *    (Pass 1) – wir merken uns jede Kategorie,
         *    damit wir in Pass 2 schnell darauf zugreifen können
         * ────────────────────────────────────────────*/
        var catMap = new Dictionary<FilterCategoryDto, FilterCategory>();

        // Determine target type: use URL parameter if provided, otherwise use type from JSON
        // This allows admins to import categories from one type (e.g., Text) into another (e.g., Bild)
        var targetType = Enum.TryParse<PromptType>(type, true, out var parsedType)
                         ? parsedType
                         : (PromptType?)null;

        foreach (var dto in payload.Data)
        {
            // Use target type from URL if specified, otherwise use type from JSON
            var effectiveType = targetType ?? dto.Type;

            // 3a) Versuch: vorhandene Kategorie über Id finden (only if same type)
            FilterCategory? cat = null;
            if (dto.Id.HasValue && dto.Id.Value != 0)
            {
                cat = _unitOfWork.FilterCategory
                                 .GetFirstOrDefault(c => c.Id == dto.Id.Value && c.Type == effectiveType);
            }

            // 3b) Fallback: gleiche (Name + effectiveType) suchen
            if (cat == null)
            {
                cat = _unitOfWork.FilterCategory
                                 .GetFirstOrDefault(c => c.Name == dto.Name &&
                                                         c.Type == effectiveType);
            }

            // 3c) Falls nichts gefunden → neue Kategorie
            if (cat == null)
            {
                cat = new FilterCategory
                {
                    FilterItems = new List<FilterItem>()
                };
                _unitOfWork.FilterCategory.Add(cat);
            }

            // 3d) Eigenschaften setzen - use effective type (from URL or JSON)
            cat.Name = dto.Name;
            cat.Type = effectiveType;
            cat.DisplayOrder = dto.DisplayOrder;

            catMap[dto] = cat;   // für Pass 2 merken
        }

        _unitOfWork.Save();     // erzeugt neue Ids

        /* ─────────────────────────────────────────────
         * 4) Items verarbeiten (Pass 2)
         * ────────────────────────────────────────────*/
        var changedItems = new List<FilterItem>();
        var removedIds = new List<int>();
        foreach (var dto in payload.Data)
        {
            var cat = catMap[dto];

            // Existierende Items zu dieser Kategorie abfragen
            // (wir brauchen KEINE Navigationseigenschaft)
            var existingItems = _unitOfWork.FilterItem
                                           .GetAll()
                                           .Where(i => i.FilterCategoryId == cat.Id)
                                           .ToList();

            foreach (var itmDto in dto.FilterItems.OrderBy(i => i.SortOrder))
            {
                FilterItem? item = null;

                // 4a) Nach Id suchen …
                if (itmDto.Id.HasValue && itmDto.Id.Value != 0)
                    item = existingItems.FirstOrDefault(i => i.Id == itmDto.Id.Value);

                // 4aa) StableKey (inhaltlich stabil über Exports)
                if (item == null && !string.IsNullOrWhiteSpace(itmDto.StableKey))
                {
                    item = existingItems.FirstOrDefault(i => ComputeStableKey(i.Title, i.Info, i.Instruction) == itmDto.StableKey);
                }

                // 4b) … oder nach (Titel + SortOrder), falls Id fehlt/nicht passt
                if (item == null)
                    item = existingItems.FirstOrDefault(i => i.Title == itmDto.Title &&
                                                             i.SortOrder == itmDto.SortOrder);

                // 4c) Fallback: Gleiches Instruction (normalisiert, HTML entfernt)
                if (item == null)
                {
                    var dtoInstr = Normalize(itmDto.Instruction ?? string.Empty);
                    item = existingItems.FirstOrDefault(i => Normalize(i.Instruction ?? string.Empty) == dtoInstr);
                }

                // 4d) Neu anlegen, wenn nichts gefunden
                if (item == null)
                {
                    item = new FilterItem
                    {
                        FilterCategoryId = cat.Id
                    };
                    _unitOfWork.FilterItem.Add(item);
                }

                // 4e) Eigenschaften setzen / aktualisieren
                item.Title = itmDto.Title;
                item.Info = itmDto.Info;
                item.Instruction = itmDto.Instruction;
                item.SortOrder = itmDto.SortOrder;
                // Only enqueue for embedding if content changed vs existing entry
                bool needsEmbedding = item.Id == 0; // new item definitely needs embedding
                if (!needsEmbedding)
                {
                    var computed = BuildEmbeddedContent(item.Title, item.Info, item.Instruction);
                    var entry = await _db.SemanticIndexEntries
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync(e => e.EntityType == "FilterItem" && e.EntityId == item.Id.ToString());
                    if (entry == null || !ContentEquivalent(entry.Content ?? string.Empty, computed))
                        needsEmbedding = true;
                }
                if (needsEmbedding)
                    changedItems.Add(item);
            }

            /* 4e) Nicht mehr vorhandene Items entfernen */
            var dtoIds = dto.FilterItems
                            .Where(i => i.Id.HasValue)
                            .Select(i => i.Id!.Value)
                            .ToHashSet();

            foreach (var orphan in existingItems
                                    .Where(i => i.Id != 0 && !dtoIds.Contains(i.Id))
                                    .ToList())
            {
                removedIds.Add(orphan.Id);
                _unitOfWork.FilterItem.Remove(orphan);
            }
        }

        /* ─────────────────────────────────────────────
         * 5) Commit
         * ────────────────────────────────────────────*/
        _unitOfWork.Save();

        // Semantic indexing: only new/changed items (UpsertFilterItemAsync skips unchanged content)
        try
        {
            var ct = HttpContext?.RequestAborted ?? default;
            var list = changedItems.DistinctBy(x => x.Id).ToList();
            var total = list.Count;
            var userId = _userMgr.GetUserId(User) ?? string.Empty;

            // notify start
            await _importHub.Clients.User(userId).SendAsync("ImportProgress", new
            {
                phase = "embedding",
                current = 0,
                total,
                message = "Starte Vektorisierung …"
            }, ct);

            for (int idx = 0; idx < list.Count; idx++)
            {
                var fi = list[idx];
                await _importHub.Clients.User(userId).SendAsync("ImportProgress", new
                {
                    phase = "embedding",
                    current = idx + 1,
                    total,
                    message = fi.Title
                }, ct);

                await _semIndex.UpsertFilterItemAsync(fi, group: null, ownerUserId: null, ct);
            }
            foreach (var rid in removedIds.Distinct())
            {
                await _semIndex.RemoveForFilterItemAsync(rid, ct);
            }

            await _importHub.Clients.User(userId).SendAsync("ImportProgress", new
            {
                phase = "done",
                current = total,
                total,
                message = "Fertig"
            }, ct);
        }
        catch (Exception ex)
        {
            try
            {
                var userId = _userMgr.GetUserId(User) ?? string.Empty;
                await _importHub.Clients.User(userId).SendAsync("ImportProgress", new
                {
                    phase = "error",
                    current = 0,
                    total = 0,
                    message = ex.Message
                });
            }
            catch { }
        }

        TempData["ImportSuccess"] = "Import erfolgreich durchgeführt.";
        return RedirectToAction(nameof(Index), new { type });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CleanupIndex()
    {
        try
        {
            // Remove duplicates: keep newest per (EntityType, EntityId)
            var duplicateKeys = await _db.SemanticIndexEntries
                                         .GroupBy(e => new { e.EntityType, e.EntityId })
                                         .Where(g => g.Count() > 1)
                                         .Select(g => new { g.Key.EntityType, g.Key.EntityId })
                                         .ToListAsync();

            int deleted = 0;
            foreach (var key in duplicateKeys)
            {
                var dupes = await _db.SemanticIndexEntries
                                     .Where(e => e.EntityType == key.EntityType && e.EntityId == key.EntityId)
                                     .OrderByDescending(e => e.UpdatedAt)
                                     .ToListAsync();
                var keep = dupes.First();
                var remove = dupes.Skip(1).ToList();
                if (remove.Count > 0)
                {
                    _db.SemanticIndexEntries.RemoveRange(remove);
                    deleted += remove.Count;
                }
            }

            // Remove orphans for FilterItem
            var filterEntries = await _db.SemanticIndexEntries
                                         .Where(e => e.EntityType == "FilterItem")
                                         .Select(e => new { e.Id, e.EntityId })
                                         .ToListAsync();

            var orphanIds = new List<int>();
            foreach (var e in filterEntries)
            {
                if (int.TryParse(e.EntityId, out var itemId))
                {
                    var exists = await _db.Set<FilterItem>().AnyAsync(i => i.Id == itemId);
                    if (!exists) orphanIds.Add(e.Id);
                }
            }
            if (orphanIds.Count > 0)
            {
                var orphans = await _db.SemanticIndexEntries.Where(x => orphanIds.Contains(x.Id)).ToListAsync();
                _db.SemanticIndexEntries.RemoveRange(orphans);
                deleted += orphans.Count;
            }

            if (deleted > 0) await _db.SaveChangesAsync();

            TempData["success"] = deleted > 0
                ? $"Index bereinigt: {deleted} Einträge entfernt."
                : "Index bereinigt: keine Duplikate gefunden.";
        }
        catch (Exception ex)
        {
            TempData["error"] = "Bereinigung fehlgeschlagen: " + ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    private static string StripHtml(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        return Regex.Replace(input, "<.*?>", string.Empty);
    }

    private static string BuildEmbeddedContent(string? title, string? info, string? instruction)
    {
        var t = StripHtml(title ?? string.Empty).Trim();
        var i = StripHtml(info ?? string.Empty).Trim();
        var ins = StripHtml(instruction ?? string.Empty).Trim();
        return string.Join("\n", new[] { t, i, ins }.Where(s => !string.IsNullOrWhiteSpace(s)));
    }

    private static string Canonicalize(string? s)
    {
        s ??= string.Empty;
        var t = s.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\\n", "\n");
        t = Regex.Replace(t, "\n+", "\n");
        t = Regex.Replace(t, "[\t ]+", " ");
        return t.Trim();
    }

    private static bool ContentEquivalent(string? a, string? b)
    {
        return string.Equals(Canonicalize(a), Canonicalize(b), StringComparison.Ordinal);
    }

    private static string Normalize(string s)
    {
        s ??= string.Empty;
        // Strip HTML
        var noHtml = Regex.Replace(s, "<.*?>", string.Empty);
        // Normalize whitespace
        noHtml = Regex.Replace(noHtml, "\\s+", " ").Trim();
        return noHtml.ToLowerInvariant();
    }

    private static string ComputeStableKey(string? title, string? info, string? instruction)
    {
        var t = Normalize(title ?? string.Empty);
        var i = Normalize(info ?? string.Empty);
        var ins = Normalize(instruction ?? string.Empty);
        var raw = $"{t}|{i}|{ins}";
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
        return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();
    }

    // POST: Admin/FilterCategory/DeleteType
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteType(string? type = null, string? panel = null)
    {
        var promptType = Enum.TryParse<PromptType>(type, true, out var parsed)
                         ? parsed
                         : PromptType.Text;

        var cats = _unitOfWork.FilterCategory
                    .GetAll(includeProperties: "FilterItems")
                    .Where(c => c.Type == promptType)
                    .ToList();

        foreach (var c in cats)
            _unitOfWork.FilterCategory.Remove(c);
        _unitOfWork.Save();

        TempData["DeleteSuccess"] = $"Alle Kategorien vom Typ „{promptType}“ wurden gelöscht.";
        return RedirectToAction(nameof(Index), new { type = promptType, panel });
    }

    // POST: Admin/FilterCategory/DeleteAll
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteAll()
    {
        var allCats = _unitOfWork.FilterCategory
                       .GetAll(includeProperties: "FilterItems")
                       .ToList();

        foreach (var c in allCats)
            _unitOfWork.FilterCategory.Remove(c);
        _unitOfWork.Save();

        TempData["DeleteSuccess"] = "Alle Kategorien wurden gelöscht.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BackfillIndex(string? type = null)
    {
        var promptType = Enum.TryParse<PromptType>(type, true, out var parsed)
                         ? parsed
                         : (PromptType?)null;

        try
        {
            var ct = HttpContext?.RequestAborted ?? default;
            var count = await _semIndex.BackfillAllFilterItemsAsync(promptType, ct);
            TempData["success"] = $"Vektorisierung abgeschlossen: {count} Einträge.";
        }
        catch (Exception ex)
        {
            TempData["error"] = "Backfill fehlgeschlagen: " + ex.Message;
        }

        return RedirectToAction(nameof(Index), new { type });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id, string? type = null, string? panel = null)
    {
        var cat = _unitOfWork.FilterCategory
                             .GetFirstOrDefault(c => c.Id == id,
                                                includeProperties: "FilterItems");

        if (cat == null)
            return Json(new
            {
                success = false,
                message = "Kategorie nicht gefunden."
            });

        _unitOfWork.FilterCategory.Remove(cat);
        _unitOfWork.Save();

        // Stay on the same type tab and panel after deletion
        return RedirectToAction(nameof(Index), new { type = cat.Type, panel });
    }

}

