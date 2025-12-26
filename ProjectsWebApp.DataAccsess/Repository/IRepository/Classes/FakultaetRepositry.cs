using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.DataAccsess.Repository.IRepository.Classes
{

    public class FakultaetRepositry : Repository<Fakultaet>, IFakultaetRepositry
    {

        private ApplicationDbContext _db;

        public FakultaetRepositry(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Fakultaet obj)
        {
            _db.Fakultaet.Update(obj);
        }
    }

}
