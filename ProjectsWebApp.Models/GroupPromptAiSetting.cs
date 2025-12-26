using System;

namespace ProjectsWebApp.Models
{
    public sealed class GroupPromptAiSetting
    {
        public int Id { get; set; }
        public string Group { get; set; } = string.Empty;
        public bool? UseGlobal { get; set; }
        public string? SystemPreamble { get; set; }
        public string? KiAssistantSystemPrompt { get; set; }
        public string? UserInstructionText { get; set; }
        public string? FilterSystemPreamble { get; set; }
        public string? FilterFirstLine { get; set; }
        public string? SmartSelectionSystemPreamble { get; set; }
        public string? SmartSelectionUserPrompt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedByUserId { get; set; }
    }
}
