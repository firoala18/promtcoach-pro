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
    public class FeatureCard
    {
        public int Id { get; set; }
        public int HomePageSettingsId { get; set; }
        public string IconClass { get; set; }    // e.g. "bi bi-sliders"
        public string Title { get; set; }
        public string Description { get; set; }
    }

}
