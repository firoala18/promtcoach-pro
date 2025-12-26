using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.DataAccsess.Repository.IRepository.Classes
{
    public class ProjectVideoRepository : Repository<ProjectVideo>, IProjectVideoRepository
    {
        private readonly ApplicationDbContext _db;

        public ProjectVideoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(ProjectVideo obj)
        {
            _db.ProjectVideos.Update(obj);
        }
    }
}
