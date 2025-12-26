using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectsWebApp.Models
{
    public class GlobalPromptConfig
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "Konfiguration";
        public bool IsActive { get; set; }

        public string SystemPreamble { get; set; } = string.Empty;
        public string UserInstruction { get; set; } = string.Empty;
        public string KiAssistantSystemPrompt { get; set; } = string.Empty;
        public string FilterSystemPreamble { get; set; } = string.Empty;
        public string FilterFirstLine { get; set; } = string.Empty;
        public string SmartSelectionSystemPreamble { get; set; } = string.Empty;
        public string SmartSelectionUserPrompt { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<GlobalPromptConfigTypeGuidance> TypeGuidances { get; set; } = new();
    }
}
