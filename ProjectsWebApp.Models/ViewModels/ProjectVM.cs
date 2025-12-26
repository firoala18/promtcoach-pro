using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectsWebApp.Models.ViewModels
{
    public class ProjectVM
    {
        public Project Project { get; set; }
        //the variable name must match what in the product creat function
        [ValidateNever]
        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> CategoryList { get; set; }
        [ValidateNever]
        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> FakulteatList { get; set; }
        [ValidateNever]
        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> FachgruppenList { get; set; }
        [ValidateNever]
        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> TechAnforderungList { get; set; }
        [ValidateNever]
        // List of last 100 years
        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ReleaseYearList { get; set; }

        public List<int> SelectedCategories { get; set; } = new List<int>();
        public List<int> SelectedFakultaets { get; set; } = new List<int>();
        public List<int> SelectedFachgruppen { get; set; } = new List<int>();
        public List<int> SelectedTechAnforderungen { get; set; } = new List<int>();

        [BindNever]
        [ValidateNever]
        public List<string> ExistingConception { get; set; }

        [BindNever]
        [ValidateNever]
        public List<string> ExistingVersion { get; set; }

        [BindNever]
        [ValidateNever]
        public List<string> ExistingLizenz { get; set; }

        [BindNever]
        [ValidateNever]
        public List<string> ExistingOpenSource { get; set; }
        [BindNever]
        [ValidateNever]
        public List<string> ExistingCreativeCommons { get; set; }
        [BindNever]
        [ValidateNever]
        public List<string> ExistingProjectResponsibility { get; set; }
        [BindNever]
        [ValidateNever]
        public List<string> ExistingFoerderung { get; set; }
        [BindNever]
        [ValidateNever]
        public List<string> ExistingProjectDevelopment { get; set; }
        [BindNever]
        [ValidateNever]
        public List<string> ExistingProjectLeadership { get; set; }
        [BindNever]
        [ValidateNever]
        public List<string> ExistingProjectManagement { get; set; }
        [BindNever]
        [ValidateNever]
        public List<string> ExistingProjectPartners { get; set; }
        [BindNever]
        [ValidateNever]
        public List<string> ExistingNetzwerk { get; set; }
        [BindNever]
        [ValidateNever]
        public List<string> ExistingExpertise { get; set; }
        [BindNever]
        [ValidateNever]
        public List<string> ExistingProjectCoordination { get; set; }
        [BindNever]
        [ValidateNever]
        public List<string> ExistingProjectConception { get; set; }
        [BindNever]
        [ValidateNever]
        public List<string> ExistingProjectSupport { get; set; }

        [BindNever]
        [ValidateNever]
        public List<string> ExistingTaxonomyLevel { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingDevelopment { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingSoftwareDevelopers { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingProgramming { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingDesign { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingDidacticDesignTeam { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingMediaDesign { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingUXDesign { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingInteractionDesign { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingSoundDesign { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingThreeDArtist { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingDidactics { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingContentDevelopment { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingStoryDesign { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingResearch { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingResearchTeam { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingEvaluation { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingEvaluationTeam { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingPrimaryTargetGroup { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingProjectGoals { get; set; }

        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingMethods { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingApplications { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingDidaktischerAnsatz { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingDidacticDesign { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingRecommendations { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingDocuments { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingReferences { get; set; }
        //[BindNever]
        //[ValidateNever]
        //public List<string> ExistingMedia { get; set; }



    }
}
