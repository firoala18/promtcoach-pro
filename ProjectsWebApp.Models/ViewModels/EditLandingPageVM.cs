using ProjectsWebApp.Models.Landing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models.ViewModels
{
    // ViewModels/EditLandingPageVM.cs
    public class EditLandingPageVM
    {
        public Hero Hero { get; set; }
        public List<Mode> Modes { get; set; } = new List<Mode>();
        public List<Feature> Features { get; set; } = new List<Feature>();
    }
}
