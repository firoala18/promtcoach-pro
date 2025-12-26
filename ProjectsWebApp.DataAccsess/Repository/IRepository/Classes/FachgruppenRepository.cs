using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.DataAccsess.Repository.IRepository.Classes
{
    public class FachgruppenRepository : Repository<Fachgruppen>, IFachgruppenRepository
    {

        private ApplicationDbContext _db;

        public FachgruppenRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Fachgruppen obj)
        {
            _db.fachgruppen.Update(obj);
        }
    }
}
