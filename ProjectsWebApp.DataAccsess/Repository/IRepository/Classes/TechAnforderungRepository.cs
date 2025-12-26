using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.DataAccsess.Repository.IRepository.Classes
{
    public class TechAnforderungRepository : Repository<TechAnforderung>, ITechAnforderungRepository
    {
        private ApplicationDbContext _db;

        public TechAnforderungRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(TechAnforderung obj)
        {
            _db.TechAnforderung.Update(obj);
        }
    }
}
