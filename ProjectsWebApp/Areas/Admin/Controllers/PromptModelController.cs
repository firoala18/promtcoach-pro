using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.Models;
using ProjectsWebApp.Utility;


namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PromptModelController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PromptModelController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var models = _unitOfWork.GetRepository<PromptModel>().GetAll().ToList();
            return View(models);
        }

        public IActionResult Upsert(int? id)
        {
            var model = id == null ? new PromptModel() : _unitOfWork.GetRepository<PromptModel>().Get(p => p.Id == id.Value);
            return View(model);
        }

        [HttpPost]
        public IActionResult Upsert(PromptModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.Id == 0)
                _unitOfWork.GetRepository<PromptModel>().Add(model);
            else
                _unitOfWork.GetRepository<PromptModel>().Update(model);

            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var model = _unitOfWork.GetRepository<PromptModel>().Get(p => p.Id == id);

            if (model != null)
            {
                _unitOfWork.GetRepository<PromptModel>().Remove(model);
                _unitOfWork.Save();
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult DeleteVariation(int id)
        {
            var repo = _unitOfWork.GetRepository<PromptVariation>();
            var variation = repo.GetFirstOrDefault(v => v.Id == id);
            if (variation == null) return NotFound();

            repo.Remove(variation);
            _unitOfWork.Save();
            return Json(new { success = true });
        }



    }



}

