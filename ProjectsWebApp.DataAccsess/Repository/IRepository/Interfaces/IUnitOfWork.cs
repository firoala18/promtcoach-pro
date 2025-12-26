using ProjectsWebApp.DataAccsess.Repository.IRepository.Intefaces;
using ProjectsWebApp.DataAccsess.Repository.IRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.DataAccsess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;
        IProjectRepository Project { get; }
        IProjectImageRepository ProjectImage { get; }
        IProjectVideoRepository ProjectVideo { get; }
        IPortalCardRepository PortalCard { get; }
        IPortalVideoRepository PortalVideo { get; }
        IMitmachenContentRepository mitMachenContent { get; }
        ISliderItemRepository SliderItem { get; }
        IImpressumContentRepository ImpressumContent { get; }
        IDatenschutzContentRepository DatenschutzContent { get; }
        IUrheberechtContentRepository UrheberechtContent { get; }
        IMakerSpaceRepository MakerSpaceProject { get; }
        IMakerSpaceDescriptionRepository MakerSpaceDescription { get; }
        ISkillRepository Skill { get; }

        IFilterCategoryRepository FilterCategory { get; }
        IFilterItemRepository FilterItem { get; }
        IPromptWordRepository PromptWord { get; }

        void Save();
    }
}
