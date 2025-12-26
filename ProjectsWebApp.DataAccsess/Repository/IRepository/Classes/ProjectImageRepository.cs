using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.DataAccsess.Repository.IRepository.Classes
{
    public class ProjectImageRepository : Repository<ProjectImage>, IProjectImageRepository
    {
        private readonly ApplicationDbContext _db;

        public ProjectImageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(ProjectImage obj)
        {
            _db.ProjectImages.Update(obj);
        }
    }
}
