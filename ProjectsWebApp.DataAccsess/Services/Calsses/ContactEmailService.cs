using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.DataAccsess.Services.Calsses
{
    public class ContactEmailService : IContactEmailService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContactEmailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string GetEmail()
        {
            return _unitOfWork.GetRepository<ContactEmail>()
                .Get(e => e.Id == 1)?.Email ?? "kontakt@example.com";
        }
    }

}
