using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectsWebApp.Models
{
    public class GlobalPromptConfigTypeGuidance
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Config))]
        public int GlobalPromptConfigId { get; set; }
        public GlobalPromptConfig Config { get; set; }

        public PromptType Type { get; set; }
        public string GuidanceText { get; set; } = string.Empty;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
