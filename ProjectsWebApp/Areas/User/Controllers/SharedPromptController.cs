using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using System;
using System.Linq;

namespace ProjectsWebApp.Areas.User.Controllers
{
    [Area("User")]
    public class SharedPromptController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SharedPromptController(ApplicationDbContext db)
        {
            _db = db;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Prompt(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            PromptShareLink? link;
            try
            {
                link = _db.PromptShareLinks.FirstOrDefault(l => l.PublicId == id);
            }
            catch
            {
                return NotFound();
            }

            if (link == null) return NotFound();

            var now = DateTime.UtcNow;
            if (!link.IsActive || (link.ExpiresAtUtc.HasValue && link.ExpiresAtUtc.Value <= now))
            {
                return NotFound();
            }

            PromptTemplate? tpl;
            try
            {
                tpl = _db.PromptTemplate
                    .Include(t => t.PromptImages)
                    .Include(t => t.PromptVariations)
                    .FirstOrDefault(t => t.Id == link.PromptTemplateId);
            }
            catch
            {
                tpl = null;
            }

            if (tpl == null) return NotFound();

            return View(tpl);
        }
    }
}
