using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models.Landing
{
    // Models/Mode.cs
    public class Mode
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string IconClass { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public string RouteType { get; set; }

        public int SortOrder { get; set; }
    }
}
