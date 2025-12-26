using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.Models;
using ProjectsWebApp.Utility;

namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Restrict access to admin users
    //to apply the functions based on rules
    [Authorize(Roles = SD.Role_Admin)]
    public class TechAnforderungController : BaseController<TechAnforderung>
    {
        public TechAnforderungController(IUnitOfWork unitOfWork) : base(unitOfWork, "Category") { }
    }
}

