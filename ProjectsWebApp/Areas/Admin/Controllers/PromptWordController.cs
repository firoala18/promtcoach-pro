using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.Models;
using ProjectsWebApp.Utility;

namespace ProjectsWebApp.Areas.Admin.Controllers
{
    // Areas/Admin/Controllers/PromptWordController.cs
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class PromptWordController : Controller
    {
        private readonly IUnitOfWork _uow;
        public PromptWordController(IUnitOfWork uow) => _uow = uow;

        /* ---------- INDEX ---------- */
        public IActionResult Index() =>
            View(_uow.PromptWord.GetAll().OrderBy(w => w.Text));

        /* ---------- UPSERT ---------- */
        public IActionResult Upsert(int? id) =>
            View(id is null
                 ? new PromptWord { }
                 : _uow.PromptWord.Get(w => w.Id == id));

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Upsert(PromptWord obj)
        {
            if (!ModelState.IsValid) return View(obj);

            if (obj.Id == 0)
                _uow.PromptWord.Add(obj);
            else
                _uow.PromptWord.Update(obj);

            _uow.Save();
            TempData["success"] = "Gespeichert";
            return RedirectToAction(nameof(Index));
        }

        /* ---------- DELETE ---------- */
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var w = _uow.PromptWord.Get(w => w.Id == id);
            if (w == null) return Json(new { success = false });
            _uow.PromptWord.Remove(w); _uow.Save();
            return Json(new { success = true });
        }

        /* ---------- SORT‑ORDER Ajax ---------- */
        //[HttpPost]
        //public IActionResult Sort([FromBody] int[] orderedIds)
        //{
        //    var words = _uow.PromptWord.GetAll()
        //                   .Where(w => orderedIds.Contains(w.Id)).ToList();
        //    for (int i = 0; i < orderedIds.Length; i++)
        //        words.First(w => w.Id == orderedIds[i]).SortOrder = i + 1;

        //    _uow.PromptWord.UpdateRange(words);
        //    _uow.Save();
        //    return Ok();
        //}
    }

}
