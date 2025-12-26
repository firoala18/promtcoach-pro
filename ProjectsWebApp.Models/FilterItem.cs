using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models
{
    public class FilterItem
    {
        public int Id { get; set; }
        public string Title { get; set; }  // Text on the checkbox
        public string Info { get; set; }   // Hover tooltip text
        public string Instruction { get; set; }

        public int FilterCategoryId { get; set; }

        public int SortOrder { get; set; } = 0;

        [ValidateNever]
        public FilterCategory FilterCategory { get; set; }
    }


}
