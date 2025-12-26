using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.DataAccsess.Repository.IRepository.Classes
{
    public class FilterCategoryRepository : Repository<FilterCategory>, IFilterCategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public FilterCategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(FilterCategory category)
        {
            _db.FilterCategories.Update(category);
        }
    }

}
