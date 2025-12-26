using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectsWebApp.Models
{
    public class ProjectVideo
    {
        [Key]
        public int Id { get; set; } // Primary key

        [Required]
        [ValidateNever]
        public string Url { get; set; } // URL of the video

        public int ProjectId { get; set; } // Foreign key

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
    }
}
