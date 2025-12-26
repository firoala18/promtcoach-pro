using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models
{
    public class SavedPrompt
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Akronym { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Beschreibung { get; set; } = null!;
        public string Schluesselbegriffe { get; set; } = null!;
        [ValidateNever]
        public string UsedModels { get; set; } = string.Empty;
        public string Thema { get; set; } = null!;
        public string Ziele { get; set; } = null!;
        public string? GeneratedImagePath { get; set; }
        public string PromptHtml { get; set; } = null!;
        public string PromptType { get; set; } = null!;
        public string FilterJson { get; set; } = null!;
        public string Temperatur { get; set; } = null!;
        public string MaxZeichen { get; set; } = null!;
        public string MetaHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<SavedPromptVariation> PromptVariations { get; set; }
    = new List<SavedPromptVariation>();
    }
}
