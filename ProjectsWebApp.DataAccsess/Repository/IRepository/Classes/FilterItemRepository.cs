//using Microsoft.AspNetCore.Mvc.Filters;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.DataAccsess.Repository.IRepository.Classes
{
    public class FilterItemRepository : Repository<FilterItem>, IFilterItemRepository
    {
        private readonly ApplicationDbContext _db;
        public FilterItemRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(FilterItem category)
        {
            _db.FilterItems.Update(category);
        }
    }

}
