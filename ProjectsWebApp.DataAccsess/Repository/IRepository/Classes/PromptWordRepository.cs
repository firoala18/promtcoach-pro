using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.DataAccsess.Repository.IRepository.Interfaces;
using ProjectsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.DataAccsess.Repository.IRepository.Classes
{
    public class PromptWordRepository : Repository<PromptWord>, IPromptWordRepository
    {
        private readonly ApplicationDbContext _db;
        public PromptWordRepository(ApplicationDbContext db) : base(db) => _db = db;

        public void UpdateRange(IEnumerable<PromptWord> entities)
        {
            _db.PromptWords.UpdateRange(entities);
        }
    }

}
