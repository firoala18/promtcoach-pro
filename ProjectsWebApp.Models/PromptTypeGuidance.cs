using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectsWebApp.Models
{
    public class PromptTypeGuidance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public PromptType Type { get; set; }

        [Required]
        public string GuidanceText { get; set; } = string.Empty;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
