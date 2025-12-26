using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.DataAccsess.Repository.IRepository.Intefaces;
using ProjectsWebApp.Models;

namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MakerSpaceDescriptionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public MakerSpaceDescriptionController(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var description = _context.MakerSpaceDescriptions.FirstOrDefault();
            return View(description);
        }
   
        public IActionResult Upsert(int? id)
        {
            MakerSpaceDescription content = new();
            if (id != null)
            {
                content = _unitOfWork.MakerSpaceDescription.Get(x => x.Id == id);
                if (content == null)
                    return NotFound();
            }
            return View(content);
        }



        [HttpPost]
        public IActionResult Save(MakerSpaceDescription model)
        {
            if (model.Id == 0)
            {
                _context.MakerSpaceDescriptions.Add(model);
            }
            else
            {
                _context.MakerSpaceDescriptions.Update(model);
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _context.MakerSpaceDescriptions.Find(id);
            if (item != null)
            {
                _context.MakerSpaceDescriptions.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }

}
