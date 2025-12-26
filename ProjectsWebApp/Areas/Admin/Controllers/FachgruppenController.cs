using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.Models;
using ProjectsWebApp.Utility;

namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Restrict access to admin users
    //to apply the functions based on rules
    [Authorize(Roles = SD.Role_Admin)]
    public class FachgruppenController : BaseController<Fachgruppen>
    {
        public FachgruppenController(IUnitOfWork unitOfWork) : base(unitOfWork, "Fachgruppen") { }
    }
}
