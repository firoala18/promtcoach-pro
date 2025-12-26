using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectsWebApp.Models
{
    public class Project : IEntity
    {
        [Key]
        public int Id { get; set; }

        // General Project Overview
        [Required]
        public string Title { get; set; } // Official project title

        public string? ProjectAcronym { get; set; } // Short identification of the project

        [Required]
        [MaxLength(1200)]
        public string KurzeBeschreibung { get; set; } // Brief description

        [Required]
        public string longDescription { get; set; } // Brief description

        [Required(ErrorMessage = "At least three tags are required.")]
        [ValidateNever]
        public string? Tags { get; set; } // Comma-separated list of tags


        public string? Conception { get; set; } // Brief description

        //[Required]
        public ProjectStatus Status { get; set; } // Current project status

        [Required(ErrorMessage = "Release year is required.")]
        public int ReleaseYear { get; set; } // Start year or creation year

        // [Required]
        public string? Version { get; set; } // Version number or phase

        // Licensing Information
        public string? Lizenz { get; set; }
        public string? OpenSource { get; set; }
        public string? CreativeCommons { get; set; }

        // Links
        [DataType(DataType.Url, ErrorMessage = "Please enter a valid URL.")]
        public string? Url { get; set; } // Productive website URL

        [DataType(DataType.Url, ErrorMessage = "Please enter a valid URL.")]
        public string? DokuLink { get; set; } // Documentation link

        [DataType(DataType.Url, ErrorMessage = "Please enter a valid URL.")]
        public string? DownloadURL { get; set; } // Download URL




        // Team Information

        public string? ProjectResponsibility { get; set; } // Main responsible person
        public string? Foerderung { get; set; } // Ongoing support
        public string? ProjectDevelopment { get; set; } // All project developers
        public string? ProjectLeadership { get; set; }  // Project leadership                                              
        public string? ProjectManagement { get; set; } // Coordination and planning
        public string? ProjectPartners { get; set; } // External and internal partners
        public string? Netzwerk { get; set; } // Cooperations and connections                             
        public string? Expertise { get; set; } // Specific competencies                                    
        public string? ProjectCoordination { get; set; } // Task description                            
        public string? ProjectConception { get; set; } // Planning and concept work                                     
        public string? ProjectSupport { get; set; } // Ongoing support
        public string? zusaetzlicheInformationen { get; set; } // Ongoing support
        public string? zusaetzlicheInformationen1 { get; set; } // Ongoing support

        //// Technical Team
        public string? Development { get; set; } // Technical description
        public string? SoftwareDevelopers { get; set; } // Names or teams
        public string? Programming { get; set; } // Used programming languages/frameworks

        // Creative Team
        public string? Design { get; set; } // General design tasks
        public string? DidacticDesignTeam { get; set; } // General design tasks
        public string? MediaDesign { get; set; } // Graphical and media design
        public string? UXDesign { get; set; } // User experience design
        public string? InteractionDesign { get; set; } // Interactive elements
        public string? SoundDesign { get; set; } // Audio and sound effects
        public string? ThreeDArtist { get; set; } // 3D Artist
        // Didactic Team
        public string? Didactics { get; set; } // Pedagogical concepts
        public string? ContentDevelopment { get; set; } // Content creation
        public string? StoryDesign { get; set; } // Narrative elements

        // Research Team
        public string? Research { get; set; } // Research goals and methods
        public string? ResearchTeam { get; set; } // Research goals and methods
        public string? Evaluation { get; set; } // Evaluation and analysis
        public string? EvaluationTeam { get; set; } // EvaluationTeam and analysis

        // Project Description
        public string? PrimaryTargetGroup { get; set; } // Target audience

        // [Required]
        public string? ProjectGoals { get; set; } // Project objectives

        public string? TaxonomyLevel { get; set; } // Taxonomy level
        public string? Methods { get; set; } // Applied methods
        public string? Applications { get; set; } // Software/technologies used

        public string? DidaktischerAnsatz { get; set; } // Software/technologies used

        public string? DidacticDesign { get; set; } // General design tasks

        public string? Recommendations { get; set; } // Tips or notes
        public string? Materialien { get; set; }
        public string? Erfolgsmessung { get; set; }

        // Additional Information
        public string? Documents { get; set; } // Links to documents/reports
        public string? References { get; set; } // External references
        public string? Media { get; set; } // Multimedia content

        // Relationships (Foreign Keys and Multi-Selection)
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category? Category { get; set; }

        public int? FakultaetId { get; set; }

        [ForeignKey("FakultaetId")]
        [ValidateNever]
        public Fakultaet? Fakultaet { get; set; }

        public int? FachgruppenId { get; set; }

        [ForeignKey("FachgruppenId")]
        [ValidateNever]
        public Fachgruppen? Fachgruppen { get; set; }

        public int? TechAnforderungId { get; set; }

        [ForeignKey("TechAnforderungId")]
        [ValidateNever]
        public TechAnforderung? TechAnforderungen { get; set; }

        [ValidateNever]
        public string CategoryIds { get; set; } // Comma-separated category IDs

        [ValidateNever]
        public string FakultaetIds { get; set; } // Comma-separated faculty IDs

        [ValidateNever]
        public string FachgruppenIds { get; set; } // Comma-separated group IDs

        [ValidateNever]
        public string TechAnforderungIds { get; set; } // Comma-separated tech IDs

        [ValidateNever]
        public List<ProjectImage>? Images { get; set; } // Project images
        public List<ProjectVideo>? Videos { get; set; } // Video URL

        [Required]
        public int DisplayOrder { get; set; } // Project display order

        [Display(Name = "Teil des Virtuellen Klassenzimmers")]
        public bool IsVirtuellesKlassenzimmer { get; set; }

        public bool IsEnabled { get; set; } = false;
    }

    public enum ProjectStatus
    {
        [Display(Name = "Anstehend")]
        Anstehend, // Pending

        [Display(Name = "In Bearbeitung")]
        InBearbeitung, // Ongoing

        [Display(Name = "In Entwicklung")]
        InEntwicklung, // In Development

        [Display(Name = "Ausgesetzt")]
        Ausgesetzt, // Paused

        [Display(Name = "Abgeschlossen")]
        Abgeschlossen, // Completed

        [Display(Name = "Archiviert")]
        Archiviert // Archived
    }
}
