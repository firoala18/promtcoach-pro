using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.Models;
using ProjectsWebApp.Utility;


namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Restrict access to admin users
    public class FakultaetController : BaseController<Fakultaet>
    {

        public FakultaetController(IUnitOfWork unitOfWork) : base(unitOfWork, "Fakultaet") { }
    }
}

