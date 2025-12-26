using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models
{
    public class ProjectImage
    {
        [Key]
        public int Id { get; set; } // Primary Key

        [Required]
        [ValidateNever]
        public string Url { get; set; } // URL of the image

        public int ProjectId { get; set; } // Foreign Key for Project


        [ForeignKey("ProjectId")]
        public Project project { get; set; }

        public bool IsMainImage { get; set; } // Indicates if this is the main image

    }
}
