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
    public class HomePageSettings
    {
        public int Id { get; set; }

        // Hero
        public string HeroTitle { get; set; }
        public string HeroLead { get; set; }
        public string HeroBackgroundUrl { get; set; }
        public string HeroButtonText { get; set; }
        public string HeroButtonAction { get; set; }  // e.g. "/User/Home/Bibliothek"

        // collections
        public ICollection<ModeCard> Modes { get; set; } = new List<ModeCard>();
        public ICollection<FeatureCard> Features { get; set; } = new List<FeatureCard>();
    }

}
