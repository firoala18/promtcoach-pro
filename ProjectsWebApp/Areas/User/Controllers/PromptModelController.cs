using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.Models;
using Newtonsoft.Json;

namespace ProjectsWebApp.Areas.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromptModelController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PromptModelController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var models = _unitOfWork.GetRepository<PromptModel>()
                            .GetAll()
                            .Select(m => new {
                                titel = m.Titel,
                                redirectUrl = m.RedirectUrl
                            });

            return Ok(models);
        }

    
    }

}
