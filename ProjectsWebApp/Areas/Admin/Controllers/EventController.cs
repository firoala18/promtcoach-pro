using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;

namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Events.OrderByDescending(e => e.Termin).ToList());
        }

        public IActionResult Upsert(int? id)
        {
            if (id == null)
                return View(new EventEntry());

            var ev = _context.Events.Find(id);
            return ev == null ? NotFound() : View(ev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(EventEntry ev)
        {
            if (!ModelState.IsValid)
                return View(ev);

            if (ev.Id == 0)
                _context.Events.Add(ev);
            else
                _context.Events.Update(ev);

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var ev = _context.Events.Find(id);
            if (ev == null)
                return NotFound();

            _context.Events.Remove(ev);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
