using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.DataAccsess.Repository.IRepository.Intefaces;
using ProjectsWebApp.DataAccsess.Repository.IRepository.Interfaces;
using ProjectsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.DataAccsess.Repository.IRepository.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private readonly Dictionary<Type, object> _repositories;

        public IProjectRepository Project { get; private set; }
        public IProjectImageRepository ProjectImage { get; private set; }
        public IProjectVideoRepository ProjectVideo { get; private set; }
        public IPortalCardRepository PortalCard { get; private set; }
        public IPortalVideoRepository PortalVideo { get; private set; }
        public IMitmachenContentRepository mitMachenContent { get; private set; }

        public IImpressumContentRepository ImpressumContent { get; private set; }
        public IDatenschutzContentRepository DatenschutzContent { get; private set; }

        public ISliderItemRepository SliderItem { get; private set; }

        public IUrheberechtContentRepository UrheberechtContent { get; private set; }
        public IMakerSpaceDescriptionRepository MakerSpaceDescription { get; }
        public IMakerSpaceRepository MakerSpaceProject { get; private set; }
        public ISkillRepository Skill { get; private set; }

        //------------ Prompt engineering section ------------
        public IFilterCategoryRepository FilterCategory { get; }
        public IFilterItemRepository FilterItem { get; }

        public IPromptWordRepository PromptWord { get; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            _repositories = new Dictionary<Type, object>();
            Project = new ProjectRepository(_db);
            PortalCard = new PortalCardRepository(_db);
            mitMachenContent = new MitmachenContentRepository(_db);
            ImpressumContent = new ImpressumContentRepository(_db);
            DatenschutzContent = new DatenschutzContentRepository(_db);
            UrheberechtContent = new UrheberechtContentRepository(_db);
            SliderItem = new SliderItemRepository(_db);
            PortalVideo = new PortalVideoRepository(_db);
            MakerSpaceProject = new MakerSpaceRepository(_db);
            MakerSpaceDescription = new MakerSpaceDescriptionRepository(_db);
            Skill = new SkillRepository(_db);
            PromptWord = new PromptWordRepository(_db);


            //------------ Prompt engineering section ------------
            FilterCategory = new FilterCategoryRepository(_db);
            FilterItem = new FilterItemRepository(_db);
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
                return (IRepository<T>)_repositories[typeof(T)];

            var repository = new Repository<T>(_db);
            _repositories[typeof(T)] = repository;
            return repository;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }

}

