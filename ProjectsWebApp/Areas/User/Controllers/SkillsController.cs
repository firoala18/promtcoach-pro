using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Repository.IRepository;

namespace ProjectsWebApp.Areas.User.Controllers
{
    [Area("User")]
    public class SkillsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SkillsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var skills = _unitOfWork.Skill.GetAll().OrderBy(s => s.DisplayOrder);
            var video = _unitOfWork.PortalVideo.GetAll().FirstOrDefault() ?? new PortalVideo();

            return View(skills); // Just the list

        }
    }

}
