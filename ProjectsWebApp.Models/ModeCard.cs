using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using ProjectsWebApp;
using ProjectsWebApp.DataAccsess.Repository.IRepository;

namespace ProjectsWebApp.Models
{
    public class ModeCard
    {
        public int Id { get; set; }
        public int HomePageSettingsId { get; set; }
        public string ImageUrl { get; set; }
        public string RouteType { get; set; }    // e.g. "Text", "Sound", etc.
        public string Label { get; set; }        // "Text-Prompts"
        public string IconClass { get; set; }    // "bi bi-fonts"
    }


}
