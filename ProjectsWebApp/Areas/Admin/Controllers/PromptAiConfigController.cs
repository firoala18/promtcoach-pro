//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using ProjectsWebApp.DataAccsess.Data;
//using ProjectsWebApp.Models;
//using ProjectsWebApp.Models.ViewModels;
//using ProjectsWebApp.DataAccsess.Services.Calsses;
//using System;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ProjectsWebApp.Areas.Admin.Controllers
//{
//    [Area("Admin")]
//    [Authorize(Roles = "Admin,SuperAdmin")]
//    public class PromptAiConfigController : Controller
//    {
//        private readonly ApplicationDbContext _db;
//        public PromptAiConfigController(ApplicationDbContext db) => _db = db;

//        [HttpGet]
//        public async Task<IActionResult> Index()
//        {
//            // Latest preamble or default from seeding
//            var preamble = await _db.PromptAiSettings
//                                    .OrderByDescending(x => x.UpdatedAt)
//                                    .Select(x => x.SystemPreamble)
//                                    .FirstOrDefaultAsync()
//                           ?? "Du bist eine expert*innengeführte Assistenz für Instructional Design, Hochschuldidaktik und wissenschaftliches Arbeiten.\nSchreibe in präzisem, formalem Deutsch für Forschung & Lehre. Nutze wissenschaftliche Terminologie.\nHalte dich strikt an das JSON-Schema (Structured Outputs). Antworte ausschließlich mit JSON.\nJedes Filter-Item muss eine überprüfbare Leistung erzeugen (Artefakt/Metrik/Kriterium, Quellenstandard).";

//            var techniques = await _db.MasterTechniques
//                                      .OrderBy(x => x.DisplayOrder)
//                                      .ThenBy(x => x.Name)
//                                      .ToListAsync();

//            // First run: seed from defaults if empty
//            if (techniques.Count == 0)
//            {
//                var defaults = PromptFilterAiService.DefaultMasterTechniques
//                    .Select((n, i) => new MasterTechnique
//                    {
//                        Name = n,
//                        Enabled = true,
//                        DisplayOrder = i,
//                        UpdatedAt = DateTime.UtcNow
//                    }).ToList();
//                if (defaults.Count > 0)
//                {
//                    _db.MasterTechniques.AddRange(defaults);
//                    await _db.SaveChangesAsync();
//                    techniques = await _db.MasterTechniques
//                        .OrderBy(x => x.DisplayOrder)
//                        .ThenBy(x => x.Name)
//                        .ToListAsync();
//                }

        
//            }

//            var sb = new StringBuilder();
//            foreach (var t in techniques.Where(t => t.Enabled))
//                sb.AppendLine(t.Name);

//            var guidances = await _db.PromptTypeGuidances
//                                     .GroupBy(x => x.Type)
//                                     .Select(g => g.OrderByDescending(x => x.UpdatedAt).First())
//                                     .ToListAsync();

//            var vm = new PromptAiConfigVM
//            {
//                SystemPreamble = preamble,
//                Techniques = techniques,
//                TechniquesMultiline = sb.ToString(),
//                TypeGuidances = guidances.ToDictionary(x => x.Type, x => x.GuidanceText)
//            };
//            return View(vm);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> SavePreamble(string systemPreamble)
//        {
//            if (string.IsNullOrWhiteSpace(systemPreamble))
//            {
//                TempData["Error"] = "System-Preamble darf nicht leer sein.";
//                return RedirectToAction(nameof(Index));
//            }

//            _db.PromptAiSettings.Add(new PromptAiSetting
//            {
//                SystemPreamble = systemPreamble.Trim(),
//                UpdatedAt = DateTime.UtcNow
//            });
//            await _db.SaveChangesAsync();

//            TempData["Success"] = "System-Preamble gespeichert.";
//            return RedirectToAction(nameof(Index));
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> SaveTechniques(string techniquesMultiline)
//        {
//            var lines = (techniquesMultiline ?? string.Empty)
//                .Replace("\r", "")
//                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
//                .Distinct(StringComparer.OrdinalIgnoreCase)
//                .ToList();

//            // Load existing
//            var existing = await _db.MasterTechniques.ToListAsync();

//            // Enable/update order for listed names, create missing
//            int order = 0;
//            foreach (var name in lines)
//            {
//                var rec = existing.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
//                if (rec == null)
//                {
//                    rec = new MasterTechnique { Name = name, Enabled = true, DisplayOrder = order++, UpdatedAt = DateTime.UtcNow };
//                    _db.MasterTechniques.Add(rec);
//                }
//                else
//                {
//                    rec.Enabled = true;
//                    rec.DisplayOrder = order++;
//                    rec.UpdatedAt = DateTime.UtcNow;
//                }
//            }

//            // Disable any not listed
//            foreach (var rec in existing)
//            {
//                if (!lines.Any(n => n.Equals(rec.Name, StringComparison.OrdinalIgnoreCase)))
//                {
//                    rec.Enabled = false;
//                    rec.UpdatedAt = DateTime.UtcNow;
//                }
//            }

//            await _db.SaveChangesAsync();
//            TempData["Success"] = "Techniken gespeichert.";
//            return RedirectToAction(nameof(Index));
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> LoadDefaults()
//        {
//            var defaults = PromptFilterAiService.DefaultMasterTechniques
//                .Select((n, i) => new MasterTechnique
//                {
//                    Name = n,
//                    Enabled = true,
//                    DisplayOrder = i,
//                    UpdatedAt = DateTime.UtcNow
//                }).ToList();

//            var existing = await _db.MasterTechniques.ToListAsync();

//            // Upsert defaults
//            foreach (var d in defaults)
//            {
//                var rec = existing.FirstOrDefault(x => x.Name.Equals(d.Name, StringComparison.OrdinalIgnoreCase));
//                if (rec == null)
//                {
//                    _db.MasterTechniques.Add(d);
//                }
//                else
//                {
//                    rec.Enabled = true;
//                    rec.DisplayOrder = d.DisplayOrder;
//                    rec.UpdatedAt = DateTime.UtcNow;
//                }
//            }

//            await _db.SaveChangesAsync();
//            TempData["Success"] = "Standard-Techniken geladen.";
//            return RedirectToAction(nameof(Index));
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> SaveTypeGuidances([FromForm] List<string> types, [FromForm] List<string> texts)
//        {
//            if (types == null || texts == null || types.Count != texts.Count)
//            {
//                TempData["Error"] = "Ungültige Eingabe für Prompt‑Typen.";
//                return RedirectToAction(nameof(Index));
//            }

//            var existing = await _db.PromptTypeGuidances.ToListAsync();

//            for (int i = 0; i < types.Count; i++)
//            {
//                if (!Enum.TryParse<ProjectsWebApp.Models.PromptType>(types[i], out var t)) continue;
//                var text = (texts[i] ?? string.Empty).Trim();
//                var row = existing.FirstOrDefault(x => x.Type == t);
//                if (row == null)
//                {
//                    _db.PromptTypeGuidances.Add(new PromptTypeGuidance
//                    {
//                        Type = t,
//                        GuidanceText = text,
//                        UpdatedAt = DateTime.UtcNow
//                    });
//                }
//                else
//                {
//                    row.GuidanceText = text;
//                    row.UpdatedAt = DateTime.UtcNow;
//                }
//            }

//            await _db.SaveChangesAsync();
//            TempData["Success"] = "Prompt‑Typen aktualisiert.";
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using ProjectsWebApp.Models.ViewModels;


namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class PromptAiConfigController : Controller
    {
        private readonly ApplicationDbContext _db;
        public PromptAiConfigController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Load Modes to get dynamic type labels
            var modes = await _db.Modes
                                 .Where(m => !string.IsNullOrEmpty(m.RouteType))
                                 .ToListAsync();

            // Build display name mapping from RouteType (enum name) to Title (display label)
            var typeDisplayNames = modes
                .GroupBy(m => m.RouteType, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.First().Title ?? g.Key,
                    StringComparer.OrdinalIgnoreCase
                );
            ViewBag.TypeDisplayNames = typeDisplayNames;

            // Load existing multi-config; if none, seed from legacy
            var configs = await _db.GlobalPromptConfigs
                                   .Include(c => c.TypeGuidances)
                                   .OrderBy(c => c.Id)
                                   .ToListAsync();

            if (configs.Count == 0)
            {
                var preamble = await _db.PromptAiSettings
                                         .Where(x => x.SystemPreamble != null && x.SystemPreamble != "")
                                         .OrderByDescending(x => x.UpdatedAt)
                                         .Select(x => x.SystemPreamble)
                                         .FirstOrDefaultAsync()
                               ?? "Du bist eine expert*innengeführte Assistenz für Instructional Design, Hochschuldidaktik und wissenschaftliches Arbeiten.\nSchreibe in präzisem, formalem Deutsch für Forschung & Lehre. Nutze wissenschaftliche Terminologie.";

                var kiAssistant = await _db.PromptAiSettings
                                           .Where(x => x.KiAssistantSystemPrompt != null && x.KiAssistantSystemPrompt != "")
                                           .OrderByDescending(x => x.UpdatedAt)
                                           .Select(x => x.KiAssistantSystemPrompt)
                                           .FirstOrDefaultAsync() ?? string.Empty;

                var userInstruction = await _db.PromptTypeGuidances
                                               .Where(x => x.Type == PromptType.Text)
                                               .OrderByDescending(x => x.UpdatedAt)
                                               .Select(x => x.GuidanceText)
                                               .FirstOrDefaultAsync() ?? string.Empty;

                var filterFirstLine = await _db.PromptTypeGuidances
                                               .Where(x => x.Type == PromptType.Eigenfilter)
                                               .OrderByDescending(x => x.UpdatedAt)
                                               .Select(x => x.GuidanceText)
                                               .FirstOrDefaultAsync() ?? string.Empty;

                var filterSystemPreamble = await _db.PromptAiSettings
                                                    .Where(x => x.FilterSystemPreamble != null && x.FilterSystemPreamble != "")
                                                    .OrderByDescending(x => x.UpdatedAt)
                                                    .Select(x => x.FilterSystemPreamble)
                                                    .FirstOrDefaultAsync() ?? string.Empty;

                var smartSys = await _db.PromptAiSettings
                                         .Where(x => x.SmartSelectionSystemPreamble != null && x.SmartSelectionSystemPreamble != "")
                                         .OrderByDescending(x => x.UpdatedAt)
                                         .Select(x => x.SmartSelectionSystemPreamble)
                                         .FirstOrDefaultAsync() ?? string.Empty;
                var smartUser = await _db.PromptAiSettings
                                          .Where(x => x.SmartSelectionUserPrompt != null && x.SmartSelectionUserPrompt != "")
                                          .OrderByDescending(x => x.UpdatedAt)
                                          .Select(x => x.SmartSelectionUserPrompt)
                                          .FirstOrDefaultAsync() ?? string.Empty;

                var typeGuidances = await _db.PromptTypeGuidances
                                             .GroupBy(x => x.Type)
                                             .Select(g => g.OrderByDescending(x => x.UpdatedAt).First())
                                             .ToListAsync();

                var cfg = new GlobalPromptConfig
                {
                    Name = "Konfiguration 1",
                    IsActive = true,
                    SystemPreamble = preamble,
                    UserInstruction = userInstruction,
                    KiAssistantSystemPrompt = kiAssistant,
                    FilterSystemPreamble = filterSystemPreamble,
                    FilterFirstLine = filterFirstLine,
                    SmartSelectionSystemPreamble = smartSys,
                    SmartSelectionUserPrompt = smartUser,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                foreach (var tg in typeGuidances)
                {
                    cfg.TypeGuidances.Add(new GlobalPromptConfigTypeGuidance
                    {
                        Type = tg.Type,
                        GuidanceText = tg.GuidanceText,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
                _db.GlobalPromptConfigs.Add(cfg);
                await _db.SaveChangesAsync();
                configs = await _db.GlobalPromptConfigs.Include(c => c.TypeGuidances).OrderBy(c => c.Id).ToListAsync();
            }

            return View(configs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConfig(string name, bool cloneActive = true)
        {
            name = string.IsNullOrWhiteSpace(name) ? $"Konfiguration {DateTime.Now:HHmmss}" : name.Trim();
            GlobalPromptConfig? src = null;
            if (cloneActive)
                src = await _db.GlobalPromptConfigs.Include(c => c.TypeGuidances).FirstOrDefaultAsync(c => c.IsActive);

            var cfg = new GlobalPromptConfig
            {
                Name = name,
                IsActive = false,
                SystemPreamble = src?.SystemPreamble ?? string.Empty,
                UserInstruction = src?.UserInstruction ?? string.Empty,
                KiAssistantSystemPrompt = src?.KiAssistantSystemPrompt ?? string.Empty,
                FilterSystemPreamble = src?.FilterSystemPreamble ?? string.Empty,
                FilterFirstLine = src?.FilterFirstLine ?? string.Empty,
                SmartSelectionSystemPreamble = src?.SmartSelectionSystemPreamble ?? string.Empty,
                SmartSelectionUserPrompt = src?.SmartSelectionUserPrompt ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            if (src?.TypeGuidances != null)
            {
                foreach (var tg in src.TypeGuidances)
                {
                    cfg.TypeGuidances.Add(new GlobalPromptConfigTypeGuidance
                    {
                        Type = tg.Type,
                        GuidanceText = tg.GuidanceText,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }
            _db.GlobalPromptConfigs.Add(cfg);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Konfiguration erstellt.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenameConfig(int id, string name)
        {
            var cfg = await _db.GlobalPromptConfigs.FindAsync(id);
            if (cfg == null) { TempData["Error"] = "Konfiguration nicht gefunden."; return RedirectToAction(nameof(Index)); }
            if (cfg.IsActive) { TempData["Error"] = "Aktive Konfiguration kann nicht umbenannt werden."; return RedirectToAction(nameof(Index)); }
            name = (name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name)) { TempData["Error"] = "Name darf nicht leer sein."; return RedirectToAction(nameof(Index)); }
            cfg.Name = name;
            cfg.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Name aktualisiert.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfig(int id)
        {
            var cfg = await _db.GlobalPromptConfigs.Include(c => c.TypeGuidances).FirstOrDefaultAsync(c => c.Id == id);
            if (cfg == null) { TempData["Error"] = "Konfiguration nicht gefunden."; return RedirectToAction(nameof(Index)); }
            if (cfg.IsActive) { TempData["Error"] = "Aktive Konfiguration kann nicht gelöscht werden."; return RedirectToAction(nameof(Index)); }
            _db.GlobalPromptConfigs.Remove(cfg);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Konfiguration gelöscht.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateConfig(int id)
        {
            var cfgs = await _db.GlobalPromptConfigs.Include(c => c.TypeGuidances).ToListAsync();
            var target = cfgs.FirstOrDefault(c => c.Id == id);
            if (target == null)
            {
                TempData["Error"] = "Konfiguration nicht gefunden.";
                return RedirectToAction(nameof(Index));
            }
            foreach (var c in cfgs) c.IsActive = (c.Id == id);
            await _db.SaveChangesAsync();

            // Mirror active to legacy tables
            _db.PromptAiSettings.Add(new PromptAiSetting
            {
                SystemPreamble = target.SystemPreamble ?? string.Empty,
                KiAssistantSystemPrompt = target.KiAssistantSystemPrompt ?? string.Empty,
                FilterSystemPreamble = target.FilterSystemPreamble ?? string.Empty,
                SmartSelectionSystemPreamble = target.SmartSelectionSystemPreamble ?? string.Empty,
                SmartSelectionUserPrompt = target.SmartSelectionUserPrompt ?? string.Empty,
                UpdatedAt = DateTime.UtcNow
            });

            var existing = await _db.PromptTypeGuidances.ToListAsync();
            // Text
            UpsertTypeGuidance(existing, PromptType.Text, target.UserInstruction ?? string.Empty);
            // Eigenfilter
            UpsertTypeGuidance(existing, PromptType.Eigenfilter, target.FilterFirstLine ?? string.Empty);
            // Others
            foreach (var tg in target.TypeGuidances)
            {
                UpsertTypeGuidance(existing, tg.Type, tg.GuidanceText ?? string.Empty);
            }
            await _db.SaveChangesAsync();

            TempData["Success"] = "Konfiguration aktiviert.";
            return RedirectToAction(nameof(Index));
        }

        private void UpsertTypeGuidance(List<PromptTypeGuidance> existing, PromptType type, string text)
        {
            var row = existing.FirstOrDefault(x => x.Type == type);
            if (row == null)
            {
                _db.PromptTypeGuidances.Add(new PromptTypeGuidance { Type = type, GuidanceText = (text ?? string.Empty).Trim(), UpdatedAt = DateTime.UtcNow });
            }
            else
            {
                row.GuidanceText = (text ?? string.Empty).Trim();
                row.UpdatedAt = DateTime.UtcNow;
            }
        }

        private bool IsAjax()
        {
            try
            {
                var xrw = Request?.Headers?["X-Requested-With"].ToString();
                if (!string.IsNullOrEmpty(xrw) && xrw.Equals("XMLHttpRequest", StringComparison.OrdinalIgnoreCase)) return true;
                var accept = Request?.Headers?["Accept"].ToString() ?? string.Empty;
                if (accept.Contains("application/json", StringComparison.OrdinalIgnoreCase)) return true;
            }
            catch { }
            return false;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveConfigPreamble(int configId, string systemPreamble)
        {
            var cfg = await _db.GlobalPromptConfigs.FindAsync(configId);
            if (cfg == null)
            {
                if (IsAjax()) return BadRequest(new { ok = false, message = "Konfiguration nicht gefunden." });
                TempData["Error"] = "Konfiguration nicht gefunden."; return RedirectToAction(nameof(Index));
            }
            if (string.IsNullOrWhiteSpace(systemPreamble))
            {
                if (IsAjax()) return BadRequest(new { ok = false, message = "System‑Preamble darf nicht leer sein." });
                TempData["Error"] = "System‑Preamble darf nicht leer sein."; return RedirectToAction(nameof(Index));
            }
            cfg.SystemPreamble = systemPreamble.Trim();
            cfg.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            if (cfg.IsActive)
            {
                _db.PromptAiSettings.Add(new PromptAiSetting { SystemPreamble = cfg.SystemPreamble, UpdatedAt = DateTime.UtcNow });
                await _db.SaveChangesAsync();
            }
            if (IsAjax()) return Json(new { ok = true, message = "System‑Preamble gespeichert." });
            TempData["Success"] = "System‑Preamble gespeichert."; return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveConfigKiAssistant(int configId, string kiAssistantSystemPrompt)
        {
            var cfg = await _db.GlobalPromptConfigs.FindAsync(configId);
            if (cfg == null)
            {
                if (IsAjax()) return BadRequest(new { ok = false, message = "Konfiguration nicht gefunden." });
                TempData["Error"] = "Konfiguration nicht gefunden."; return RedirectToAction(nameof(Index));
            }
            if (string.IsNullOrWhiteSpace(kiAssistantSystemPrompt))
            {
                if (IsAjax()) return BadRequest(new { ok = false, message = "KI‑Assistent‑Systemprompt darf nicht leer sein." });
                TempData["Error"] = "KI‑Assistent‑Systemprompt darf nicht leer sein."; return RedirectToAction(nameof(Index));
            }
            cfg.KiAssistantSystemPrompt = kiAssistantSystemPrompt.Trim();
            cfg.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            if (cfg.IsActive)
            {
                _db.PromptAiSettings.Add(new PromptAiSetting { KiAssistantSystemPrompt = cfg.KiAssistantSystemPrompt, UpdatedAt = DateTime.UtcNow });
                await _db.SaveChangesAsync();
            }
            if (IsAjax()) return Json(new { ok = true, message = "KI‑Assistent‑Systemprompt gespeichert." });
            TempData["Success"] = "KI‑Assistent‑Systemprompt gespeichert."; return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveConfigFilterPreamble(int configId, string filterSystemPreamble)
        {
            var cfg = await _db.GlobalPromptConfigs.FindAsync(configId);
            if (cfg == null)
            {
                if (IsAjax()) return BadRequest(new { ok = false, message = "Konfiguration nicht gefunden." });
                TempData["Error"] = "Konfiguration nicht gefunden."; return RedirectToAction(nameof(Index));
            }
            if (string.IsNullOrWhiteSpace(filterSystemPreamble))
            {
                if (IsAjax()) return BadRequest(new { ok = false, message = "Filter‑System‑Preamble darf nicht leer sein." });
                TempData["Error"] = "Filter‑System‑Preamble darf nicht leer sein."; return RedirectToAction(nameof(Index));
            }
            cfg.FilterSystemPreamble = filterSystemPreamble.Trim();
            cfg.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            if (cfg.IsActive)
            {
                _db.PromptAiSettings.Add(new PromptAiSetting { FilterSystemPreamble = cfg.FilterSystemPreamble, UpdatedAt = DateTime.UtcNow });
                await _db.SaveChangesAsync();
            }
            if (IsAjax()) return Json(new { ok = true, message = "Filter‑System‑Preamble gespeichert." });
            TempData["Success"] = "Filter‑System‑Preamble gespeichert."; return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveConfigFilterFirstLine(int configId, string filterFirstLine)
        {
            var cfg = await _db.GlobalPromptConfigs.FindAsync(configId);
            if (cfg == null) { TempData["Error"] = "Konfiguration nicht gefunden."; return RedirectToAction(nameof(Index)); }
            cfg.FilterFirstLine = (filterFirstLine ?? string.Empty).Trim();
            cfg.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            if (cfg.IsActive)
            {
                var existing = await _db.PromptTypeGuidances.ToListAsync();
                UpsertTypeGuidance(existing, PromptType.Eigenfilter, cfg.FilterFirstLine);
                await _db.SaveChangesAsync();
            }
            if (IsAjax()) return Json(new { ok = true, message = "Filter‑Erstzeile gespeichert." });
            TempData["Success"] = "Filter‑Erstzeile gespeichert."; return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveConfigTypeGuidances(int configId, [FromForm] List<string> types, [FromForm] List<string> texts)
        {
            var cfg = await _db.GlobalPromptConfigs.Include(c => c.TypeGuidances).FirstOrDefaultAsync(c => c.Id == configId);
            if (cfg == null)
            {
                if (IsAjax()) return BadRequest(new { ok = false, message = "Konfiguration nicht gefunden." });
                TempData["Error"] = "Konfiguration nicht gefunden."; return RedirectToAction(nameof(Index));
            }
            if (types == null || texts == null || types.Count != texts.Count)
            {
                if (IsAjax()) return BadRequest(new { ok = false, message = "Ungültige Eingabe für Prompt‑Typen." });
                TempData["Error"] = "Ungültige Eingabe für Prompt‑Typen."; return RedirectToAction(nameof(Index));
            }

            // Overwrite or upsert into config
            foreach (var pair in cfg.TypeGuidances.ToList())
                _db.GlobalPromptConfigTypeGuidances.Remove(pair);
            for (int i = 0; i < types.Count; i++)
            {
                if (!Enum.TryParse<PromptType>(types[i], out var t)) continue;
                cfg.TypeGuidances.Add(new GlobalPromptConfigTypeGuidance { Type = t, GuidanceText = (texts[i] ?? string.Empty).Trim(), UpdatedAt = DateTime.UtcNow });
            }
            cfg.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            if (cfg.IsActive)
            {
                var existing = await _db.PromptTypeGuidances.ToListAsync();
                foreach (var tg in cfg.TypeGuidances)
                    UpsertTypeGuidance(existing, tg.Type, tg.GuidanceText);
                await _db.SaveChangesAsync();
            }
            if (IsAjax()) return Json(new { ok = true, message = "Prompt‑Typen aktualisiert." });
            TempData["Success"] = "Prompt‑Typen aktualisiert."; return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveConfigSmartSelectionPreamble(int configId, string smartSelectionSystemPreamble)
        {
            var cfg = await _db.GlobalPromptConfigs.FindAsync(configId);
            if (cfg == null)
            {
                if (IsAjax()) return BadRequest(new { ok = false, message = "Konfiguration nicht gefunden." });
                TempData["Error"] = "Konfiguration nicht gefunden."; return RedirectToAction(nameof(Index));
            }
            if (string.IsNullOrWhiteSpace(smartSelectionSystemPreamble))
            {
                if (IsAjax()) return BadRequest(new { ok = false, message = "Smart‑Selection‑System‑Preamble darf nicht leer sein." });
                TempData["Error"] = "Smart‑Selection‑System‑Preamble darf nicht leer sein."; return RedirectToAction(nameof(Index));
            }
            cfg.SmartSelectionSystemPreamble = smartSelectionSystemPreamble.Trim();
            cfg.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            if (cfg.IsActive)
            {
                _db.PromptAiSettings.Add(new PromptAiSetting { SmartSelectionSystemPreamble = cfg.SmartSelectionSystemPreamble, UpdatedAt = DateTime.UtcNow });
                await _db.SaveChangesAsync();
            }
            if (IsAjax()) return Json(new { ok = true, message = "Smart‑Selection‑System‑Preamble gespeichert." });
            TempData["Success"] = "Smart‑Selection‑System‑Preamble gespeichert."; return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveConfigSmartSelectionUser(int configId, string smartSelectionUserPrompt)
        {
            var cfg = await _db.GlobalPromptConfigs.FindAsync(configId);
            if (cfg == null) { TempData["Error"] = "Konfiguration nicht gefunden."; return RedirectToAction(nameof(Index)); }
            cfg.SmartSelectionUserPrompt = (smartSelectionUserPrompt ?? string.Empty).Trim();
            cfg.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            if (cfg.IsActive)
            {
                _db.PromptAiSettings.Add(new PromptAiSetting { SmartSelectionUserPrompt = cfg.SmartSelectionUserPrompt, UpdatedAt = DateTime.UtcNow });
                await _db.SaveChangesAsync();
            }
            if (IsAjax()) return Json(new { ok = true, message = "Smart‑Selection‑User‑Prompt gespeichert." });
            TempData["Success"] = "Smart‑Selection‑User‑Prompt gespeichert."; return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveConfigUserInstruction(int configId, string userInstruction)
        {
            var cfg = await _db.GlobalPromptConfigs.FindAsync(configId);
            if (cfg == null) { TempData["Error"] = "Konfiguration nicht gefunden."; return RedirectToAction(nameof(Index)); }
            cfg.UserInstruction = (userInstruction ?? string.Empty).Trim();
            cfg.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            if (cfg.IsActive)
            {
                var existing = await _db.PromptTypeGuidances.ToListAsync();
                UpsertTypeGuidance(existing, PromptType.Text, cfg.UserInstruction);
                await _db.SaveChangesAsync();
            }
            if (IsAjax()) return Json(new { ok = true, message = "User‑Prompt gespeichert." });
            TempData["Success"] = "User‑Prompt gespeichert."; return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveKiAssistantPrompt(string kiAssistantSystemPrompt)
        {
            if (string.IsNullOrWhiteSpace(kiAssistantSystemPrompt))
            {
                TempData["Error"] = "KI‑Assistent‑Systemprompt darf nicht leer sein.";
                return RedirectToAction(nameof(Index));
            }

            _db.PromptAiSettings.Add(new PromptAiSetting
            {
                KiAssistantSystemPrompt = kiAssistantSystemPrompt.Trim(),
                UpdatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = "KI‑Assistent‑Systemprompt gespeichert.";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveUserInstruction(string userInstruction)
        {
            if (userInstruction == null) userInstruction = string.Empty;

            // Upsert for global user instruction stored under PromptType.Text
            var row = await _db.PromptTypeGuidances
                               .FirstOrDefaultAsync(x => x.Type == PromptType.Text);
            if (row == null)
            {
                _db.PromptTypeGuidances.Add(new PromptTypeGuidance
                {
                    Type = PromptType.Text,
                    GuidanceText = userInstruction.Trim(),
                    UpdatedAt = DateTime.UtcNow
                });
            }
            else
            {
                row.GuidanceText = userInstruction.Trim();
                row.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "User‑Prompt gespeichert.";
            return RedirectToAction(nameof(Index));
        }
    }
}
