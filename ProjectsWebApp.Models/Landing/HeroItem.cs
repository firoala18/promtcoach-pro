using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models.Landing
{
    // Models/Hero.cs
    public class Hero
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Lead { get; set; }

        [ValidateNever]
        public string BackgroundUrl { get; set; }
    }
}
