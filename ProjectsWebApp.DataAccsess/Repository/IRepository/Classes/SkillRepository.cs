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
    public class SkillRepository : Repository<Skill>, ISkillRepository
    {
        private readonly ApplicationDbContext _context;

        public SkillRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Skill skill)
        {
            var objFromDb = _context.Skills.FirstOrDefault(s => s.Id == skill.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = skill.Title;
                objFromDb.Description = skill.Description;
                objFromDb.DisplayOrder = skill.DisplayOrder;
                objFromDb.IconPath = skill.IconPath;
            }
        }
    }

}
