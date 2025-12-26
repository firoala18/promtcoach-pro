using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.Models;


namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SkillController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SkillController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var skills = _unitOfWork.Skill.GetAll().OrderBy(s => s.DisplayOrder);
            return View(skills);
        }

        public IActionResult Upsert(int? id)
        {
            if (id == null) return View(new Skill());

            var skill = _unitOfWork.Skill.Get(x => x.Id == id.GetValueOrDefault());

            if (skill == null) return NotFound();

            return View(skill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Skill skill)
        {
            if (!ModelState.IsValid) return View(skill);

            if (skill.Id == 0)
                _unitOfWork.Skill.Add(skill);
            else
                _unitOfWork.Skill.Update(skill);

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var skill = _unitOfWork.Skill.Get(x => x.Id == id); 


            if (skill == null)
                return Json(new { success = false, message = "Skill not found." });

            _unitOfWork.Skill.Remove(skill);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Skill deleted successfully." });
        }
    }

}
