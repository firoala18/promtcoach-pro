using ProjectsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.DataAccsess.Repository.IRepository
{
    public interface IProjectImageRepository : IRepository<ProjectImage>
    {
        void Update(ProjectImage obj);

    }
}
