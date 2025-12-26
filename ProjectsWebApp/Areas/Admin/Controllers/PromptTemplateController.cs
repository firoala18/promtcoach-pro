using Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.Dto;
using ProjectsWebApp.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace ProjectsWebApp.Areas.User.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PromptTemplateController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userMgr;


        public PromptTemplateController(IUnitOfWork unitOfWork, ApplicationDbContext db, UserManager<IdentityUser> userMgr)
        {
            _unitOfWork = unitOfWork;
            _userMgr = userMgr;
            _db = db;
        }

        public IActionResult Index()
        {
            var templates = _unitOfWork.GetRepository<PromptTemplate>().GetAll().OrderByDescending(t => t.CreatedAt);
            return View(templates);
        }

        // UPDATED  Save(): insert‑or‑update instead of “always add”
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] PromptTemplate dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Akronym) ||
                string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Akronym und Titel dürfen nicht leer sein.");

            dto.UserId = _userMgr.GetUserId(User);

            var existing = await _db.PromptTemplate
                                    .FirstOrDefaultAsync(t => t.Akronym == dto.Akronym);

            if (existing is null)
            {
                dto.CreatedAt = DateTime.UtcNow;
                // Ensure FilterJson includes parsed techniques from PromptHtml
                dto.FilterJson = MergeWithTechniqueTags(dto.FilterJson, dto.PromptHtml);
                dto.UsedModels = dto.UsedModels ?? string.Empty;
                _db.PromptTemplate.Add(dto);
            }
            else
            {
                // nur aktualisieren, *keinen* zweiten Datensatz erzeugen
                existing.Title = dto.Title;
                existing.Beschreibung = dto.Beschreibung;
                existing.Schluesselbegriffe = dto.Schluesselbegriffe;
                existing.Thema = dto.Thema;
                existing.Ziele = dto.Ziele;
                existing.PromptHtml = dto.PromptHtml;
                existing.PromptType = dto.PromptType;
                existing.FilterJson = MergeWithTechniqueTags(dto.FilterJson, dto.PromptHtml);
                existing.Temperatur = dto.Temperatur;
                existing.MaxZeichen = dto.MaxZeichen;
                existing.MetaHash = dto.MetaHash;
                existing.UsedModels = dto.UsedModels ?? existing.UsedModels ?? string.Empty;
                
            }

            await _db.SaveChangesAsync();
            return Ok();          // { status:200 }
        }

        private static string MergeWithTechniqueTags(string? filterJson, string? promptHtml)
        {
            var items = new List<Dictionary<string, string>>();
            // 1) existing items
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
                            if (!string.IsNullOrWhiteSpace(val))
                                items.Add(new Dictionary<string, string> { { "value", val.Trim() }, { "instruction", ins.Trim() } });
                        }
                    }
                }
                catch { }
            }

            var existing = new HashSet<string>(items.Select(x => x["value"]), StringComparer.OrdinalIgnoreCase);

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

      

        // NEW  probe:  /Admin/PromptTemplate/Exists?akronym=XYZ

        [HttpGet]
        public async Task<IActionResult> Exists(string akronym)
        {
            if (string.IsNullOrWhiteSpace(akronym))
                return BadRequest(new AkronymExistsDto
                {
                    Exists = false,
                    Message = "Akronym darf nicht leer sein."
                });

            var exists = await _db.PromptTemplate     // <-- DbContext already injected
                                  .AnyAsync(t => t.Akronym == akronym);

            return Ok(new AkronymExistsDto { Exists = exists });
        }


        [HttpPost]
        public async Task<IActionResult> AddVariation([FromBody] AddVariationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Akronym))
                return BadRequest(new { success = false, message = "Akronym darf nicht leer sein." });

            var template = await _db.PromptTemplate
                                    .FirstOrDefaultAsync(t => t.Akronym == dto.Akronym);
            if (template == null)
                return NotFound(new { success = false, message = $"Kein Prompt mit Akronym '{dto.Akronym}' gefunden." });

            var variation = new PromptVariation
            {
                PromptTemplateId = template.Id,
                VariationJson = JsonSerializer.Serialize(dto.Data)
            };
            _db.PromptVariations.Add(variation);
            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "Variation erfolgreich gespeichert." });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var template = _unitOfWork.GetRepository<PromptTemplate>().GetFirstOrDefault(t => t.Id == id);
            if (template == null) return NotFound();

            _unitOfWork.GetRepository<PromptTemplate>().Remove(template);
            _unitOfWork.Save();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVariation(int id)
        {
            // 1) load the variation – include template only if you need it
            var variation = await _db.PromptVariations
                                     .FirstOrDefaultAsync(v => v.Id == id);
            if (variation is null)
                return NotFound();                       // 404 for invalid id

            // 2) delete + save
            _db.PromptVariations.Remove(variation);
            await _db.SaveChangesAsync();

            // 3) let the JS know everything went well
            return Ok();                                 // 200
        }

    }

}
