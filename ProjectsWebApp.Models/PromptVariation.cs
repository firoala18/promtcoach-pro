using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models
{
    public class PromptVariation
    {
        public int Id { get; set; }
        public int PromptTemplateId { get; set; }
        public string VariationJson { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public PromptTemplate PromptTemplate { get; set; }
    }

}
