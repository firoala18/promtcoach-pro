using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.DataAccsess.Repository.IRepository;

namespace ProjectsWebApp.Controllers
{
    [Area("User")]
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public EventController(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var events = _context.Events.OrderBy(e => e.Termin).ToList();
            return View(events);
        }

        public IActionResult Details(int id)
        {
            var ev = _context.Events.FirstOrDefault(e => e.Id == id);

            var email = _unitOfWork.GetRepository<ContactEmail>().Get(e => e.Id == 1)?.Email ?? "h.seehagen-marx@uni-wuppertal.de";

            ViewBag.ContactEmail = email;

            //ViewBag.ContactEmail = "kontakt@deinportal.de";
            return ev == null ? NotFound() : View(ev);
        }
    }
}
