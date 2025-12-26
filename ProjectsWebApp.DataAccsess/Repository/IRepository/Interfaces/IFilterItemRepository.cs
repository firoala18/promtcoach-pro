using ProjectsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.DataAccsess.Repository.IRepository
{
    public interface IFilterItemRepository : IRepository<FilterItem>
    {
        void Update(FilterItem item);
    }

}
