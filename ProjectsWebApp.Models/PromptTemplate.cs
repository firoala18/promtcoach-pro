using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models
{
    public class PromptTemplate
    {
        public int Id { get; set; }

        public string Akronym { get; set; }
        public string Title { get; set; }  // Name for the template
        public string Thema { get; set; }
        public string Beschreibung { get; set; } = string.Empty;
        [ValidateNever]
        public string UsedModels { get; set; } = string.Empty;
        public string Schluesselbegriffe { get; set; } = string.Empty;
        public string Ziele { get; set; }
        public double Temperatur { get; set; }   // non‑nullable
        public int MaxZeichen { get; set; }   // non‑nullable
        public string? GeneratedImagePath { get; set; }
        [ValidateNever]
        public string? GeneratedVideoUrl { get; set; }
        [ValidateNever]
        public string? GeneratedAudioUrl { get; set; }
        public string PromptHtml { get; set; }
        public string PromptType { get; set; }  // Save as string: "Text", "Bild", "Video"
        [ValidateNever]
        public string FilterJson { get; set; }  // Serialized selected filters
        [ValidateNever]
        public string MetaHash { get; set; }    // Optional: hash to prevent duplication
        [ValidateNever]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? UserId { get; set; }
        [ValidateNever]
        public string? Autorin { get; set; }
        [ValidateNever]
        public string? Lizenz { get; set; }

        [ValidateNever]
        public string? Reflektion { get; set; }

        public virtual ICollection<PromptVariation> PromptVariations { get; set; }
           = new List<PromptVariation>();

        [ValidateNever]
        public virtual ICollection<PromptImage> PromptImages { get; set; }
           = new List<PromptImage>();
    }

}
