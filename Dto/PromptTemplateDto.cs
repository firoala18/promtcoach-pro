using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public sealed class PromptTemplateDto
    {
        public string Akronym { get; set; } = default!;
        public string Title { get; set; } = default!;

        // ── optionale Felder nullable machen ─────────────
        public string? Beschreibung { get; set; }
        public string? Schluesselbegriffe { get; set; }
        public string? Thema { get; set; }
        public string? Ziele { get; set; }

        public string PromptHtml { get; set; } = default!;
        public string PromptType { get; set; } = default!;
        public string? FilterJson { get; set; }

        public double Temperatur { get; set; }
        public int? MaxZeichen { get; set; }

        public string? UsedModels { get; set; }
        public string? GeneratedImagePath { get; set; }

        public string? MetaHash { get; set; }

        // Optional: selected groups to publish this prompt to
        public List<string>? Groups { get; set; }
    }
}
