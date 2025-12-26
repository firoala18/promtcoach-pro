using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectsWebApp.Models
{
    public class GroupPromptTypeGuidance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Group { get; set; } = string.Empty;

        [Required]
        public PromptType Type { get; set; }

        [Required]
        public string GuidanceText { get; set; } = string.Empty;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
