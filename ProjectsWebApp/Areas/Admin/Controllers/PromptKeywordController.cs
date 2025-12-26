using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.Models;
using ProjectsWebApp.Utility;
using System.Linq;

namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public sealed class PromptKeywordController : Controller
    {
        private readonly IUnitOfWork _uow;
        public PromptKeywordController(IUnitOfWork uow) => _uow = uow;

        // GET: /Admin/PromptKeyword
        public IActionResult Index()
        {
            var curated = _uow.GetRepository<PromptKeyword>()
                              .GetAll()
                              .OrderBy(k => k.Text)
                              .ToList();

            // Collect distinct Bibliothek terms from PromptTemplate.Schluesselbegriffe
            var biblTerms = _uow.GetRepository<PromptTemplate>()
                                .GetAll()
                                .Select(t => t.Schluesselbegriffe ?? string.Empty)
                                .SelectMany(s => s.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                                .Select(s => s.Trim())
                                .Where(s => !string.IsNullOrWhiteSpace(s))
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .OrderBy(s => s)
                                .ToList();

            ViewBag.BibliothekKeywords = biblTerms;
            return View(curated);
        }

        // GET: /Admin/PromptKeyword/Upsert/{id?}
        public IActionResult Upsert(int? id)
        {
            var repo = _uow.GetRepository<PromptKeyword>();
            if (id is null)
                return View(new PromptKeyword());

            var entity = repo.Get(k => k.Id == id);
            if (entity == null) return NotFound();
            return View(entity);
        }

        // POST: /Admin/PromptKeyword/Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(PromptKeyword model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var repo = _uow.GetRepository<PromptKeyword>();
            if (model.Id == 0)
                repo.Add(model);
            else
                repo.Update(model);

            _uow.Save();
            TempData["success"] = "Gespeichert";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/PromptKeyword/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var repo = _uow.GetRepository<PromptKeyword>();
            var entity = repo.Get(k => k.Id == id);
            if (entity == null)
                return Json(new { success = false });

            repo.Remove(entity);
            _uow.Save();
            return Json(new { success = true });
        }

        // POST: /Admin/PromptKeyword/RenameBibliothekKeyword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RenameBibliothekKeyword(string oldText, string newText)
        {
            if (string.IsNullOrWhiteSpace(oldText) || string.IsNullOrWhiteSpace(newText))
                return BadRequest();

            if (string.Equals(oldText.Trim(), newText.Trim(), StringComparison.OrdinalIgnoreCase))
                return Ok();

            var tplRepo = _uow.GetRepository<PromptTemplate>();
            var templates = tplRepo.GetAll().ToList();
            int updates = 0;

            foreach (var t in templates)
            {
                var raw = t.Schluesselbegriffe ?? string.Empty;
                var terms = raw.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => s.Trim())
                               .ToList();

                bool changed = false;
                for (int i = 0; i < terms.Count; i++)
                {
                    if (string.Equals(terms[i], oldText, StringComparison.OrdinalIgnoreCase))
                    {
                        terms[i] = newText.Trim();
                        changed = true;
                    }
                }

                if (changed)
                {
                    // De-duplicate and normalize
                    t.Schluesselbegriffe = string.Join(", ", terms
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(s => s));
                    tplRepo.Update(t);
                    updates++;
                }
            }

            _uow.Save();
            return Ok(new { success = true, updated = updates });
        }

        // POST: /Admin/PromptKeyword/DeleteBibliothekKeyword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteBibliothekKeyword(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return BadRequest();

            var tplRepo = _uow.GetRepository<PromptTemplate>();
            var templates = tplRepo.GetAll().ToList();
            int updates = 0;

            foreach (var t in templates)
            {
                var raw = t.Schluesselbegriffe ?? string.Empty;
                var terms = raw.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => s.Trim())
                               .ToList();

                int before = terms.Count;
                terms = terms.Where(s => !string.Equals(s, text, StringComparison.OrdinalIgnoreCase)).ToList();
                if (terms.Count != before)
                {
                    t.Schluesselbegriffe = string.Join(", ", terms
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(s => s));
                    tplRepo.Update(t);
                    updates++;
                }
            }

            _uow.Save();
            return Ok(new { success = true, updated = updates });
        }
    }
}
