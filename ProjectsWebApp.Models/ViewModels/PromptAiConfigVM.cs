using System.Collections.Generic;
using ProjectsWebApp.Models;

namespace ProjectsWebApp.Models.ViewModels
{
    public class PromptAiConfigVM
    {
        public string SystemPreamble { get; set; } = string.Empty;
        public string FilterSystemPreamble { get; set; } = string.Empty;
        public string UserInstruction { get; set; } = string.Empty;
        public string KiAssistantSystemPrompt { get; set; } = string.Empty;
        // Filter generieren: editable first line for BuildUserTask (stored under PromptType.Eigenfilter)
        public string FilterFirstLine { get; set; } = string.Empty;
        // Dynamic per-type guidance used by BuildTypeGuidance
        public Dictionary<PromptType, string> TypeGuidances { get; set; } = new();

        // Smart Selection
        public string SmartSelectionSystemPreamble { get; set; } = string.Empty;
        public string SmartSelectionUserPrompt { get; set; } = string.Empty;
    }
}
