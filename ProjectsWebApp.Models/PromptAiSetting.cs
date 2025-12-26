using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectsWebApp.Models
{
    public class PromptAiSetting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SystemPreamble { get; set; } = string.Empty; // free-text system prompt intro

        public string KiAssistantSystemPrompt { get; set; } = string.Empty;

        // Separate system preamble for Filter generieren
        public string FilterSystemPreamble { get; set; } = string.Empty;

        // Smart Selection prompts
        public string SmartSelectionSystemPreamble { get; set; } = string.Empty;
        public string SmartSelectionUserPrompt { get; set; } = string.Empty;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
