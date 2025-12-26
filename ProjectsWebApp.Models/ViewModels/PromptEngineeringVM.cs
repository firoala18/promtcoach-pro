using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectsWebApp.Models.ViewModels
{
    public class PromptEngineeringVM
    {
        public PromptType Type { get; set; } = PromptType.Text;
        public string Thema { get; set; }
        public string Ziele { get; set; }

        public List<FilterCategory> Categories { get; set; } = new();
    }
}
