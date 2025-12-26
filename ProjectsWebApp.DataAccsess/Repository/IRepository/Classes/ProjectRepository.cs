using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.DataAccsess.Repository.IRepository.Classes
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {

        private ApplicationDbContext _db;

        public ProjectRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Project obj)
        {
            var objFromDb = _db.Projects.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                // General Project Overview
                objFromDb.Title = obj.Title;
                objFromDb.ProjectAcronym = obj.ProjectAcronym;
                objFromDb.KurzeBeschreibung = obj.KurzeBeschreibung;
                //  objFromDb.Status = obj.Status;
                objFromDb.ReleaseYear = obj.ReleaseYear;
                objFromDb.Version = obj.Version;

                // Licensing Information
                objFromDb.Lizenz = obj.Lizenz;
                objFromDb.OpenSource = obj.OpenSource;
                objFromDb.CreativeCommons = obj.CreativeCommons;

                // Links
                objFromDb.Url = obj.Url;
                objFromDb.DokuLink = obj.DokuLink;
                objFromDb.DownloadURL = obj.DownloadURL;
                //objFromDb.VideoUrl = obj.VideoUrl;

                // Team Information
                objFromDb.ProjectResponsibility = obj.ProjectResponsibility;
                objFromDb.ProjectDevelopment = obj.ProjectDevelopment;
                //objFromDb.ProjectLeadership = obj.ProjectLeadership;
                //objFromDb.ProjectManagement = obj.ProjectManagement;
                //objFromDb.ProjectPartners = obj.ProjectPartners;
                //objFromDb.Netzwerk = obj.Netzwerk;
                //objFromDb.Expertise = obj.Expertise;
                //objFromDb.ProjectCoordination = obj.ProjectCoordination;
                //objFromDb.ProjectConception = obj.ProjectConception;
                //objFromDb.ProjectSupport = obj.ProjectSupport;

                // Technical Team
                //objFromDb.Development = obj.Development;
                //objFromDb.SoftwareDevelopers = obj.SoftwareDevelopers;
                //objFromDb.Programming = obj.Programming;

                // Creative Team
                //objFromDb.Design = obj.Design;
                //objFromDb.MediaDesign = obj.MediaDesign;
                //objFromDb.UXDesign = obj.UXDesign;
                //objFromDb.InteractionDesign = obj.InteractionDesign;
                //objFromDb.SoundDesign = obj.SoundDesign;

                // Didactic Team
                //objFromDb.Didactics = obj.Didactics;
                //objFromDb.ContentDevelopment = obj.ContentDevelopment;
                //objFromDb.StoryDesign = obj.StoryDesign;

                // Research Team
                //objFromDb.Research = obj.Research;
                //objFromDb.Evaluation = obj.Evaluation;

                //// Project Description
                //objFromDb.PrimaryTargetGroup = obj.PrimaryTargetGroup;
                objFromDb.ProjectGoals = obj.ProjectGoals;
                //objFromDb.TaxonomyLevel = obj.TaxonomyLevel;
                //objFromDb.Methods = obj.Methods;
                objFromDb.Applications = obj.Applications;
                //objFromDb.Recommendations = obj.Recommendations;

                // Additional Information
                objFromDb.Documents = obj.Documents;
                objFromDb.References = obj.References;
                objFromDb.Media = obj.Media;

                // Foreign Key and Multi-Selection Fields
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.FakultaetId = obj.FakultaetId;
                objFromDb.FachgruppenId = obj.FachgruppenId;
                objFromDb.TechAnforderungId = obj.TechAnforderungId;
                objFromDb.CategoryIds = obj.CategoryIds;
                objFromDb.FakultaetIds = obj.FakultaetIds;
                objFromDb.FachgruppenIds = obj.FachgruppenIds;
                objFromDb.TechAnforderungIds = obj.TechAnforderungIds;


                // Images
                objFromDb.Images = obj.Images;
            }
        }

    }
}
