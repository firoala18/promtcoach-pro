using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ProjectsWebApp.Models.Landing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models.ViewModels
{
    // ViewModels/LandingPageVM.cs
    public class LandingPageVM
    {
        public Hero Hero { get; set; } = new Hero();
        public List<Mode> Modes { get; set; } = new List<Mode>();
        public List<Feature> Features { get; set; } = new List<Feature>();

        [ValidateNever]
        public IFormFile? HeroBackgroundFile { get; set; }

        [ValidateNever]
        public List<IFormFile>? ModeImageFiles { get; set; } = new List<IFormFile>();
    }

   


}
