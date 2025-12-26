using ProjectsWebApp.DataAccsess.Repository.IRepository.Intefaces;
using ProjectsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.DataAccsess.Repository.IRepository.Intefaces
{
    public interface IMakerSpaceDescriptionRepository : IRepository<MakerSpaceDescription>
    {
        void Update(MakerSpaceDescription card);
    }
}

