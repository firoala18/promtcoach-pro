using System.ComponentModel.DataAnnotations;

namespace ProjectsWebApp.Areas.Admin.Models
{
    public class GroupPromptsAdminViewModel
    {
        public int GroupId { get; set; }

        [Required]
        public string GroupName { get; set; } = string.Empty;

        [Display(Name = "Globale System-Prompts verwenden")]
        public bool UseGlobal { get; set; } = true;

        [Display(Name = "System-Preamble")]
        public string? SystemPreamble { get; set; }

        [Display(Name = "User-Instruction (Text)")]
        public string? UserInstructionText { get; set; }

        [Display(Name = "KI-Assistent Systemprompt")]
        public string? KiAssistantSystemPrompt { get; set; }

        // Filter prompts
        public string? FilterSystemPreamble { get; set; }
        public string? FilterFirstLine { get; set; }

        // Smart selection prompts
        public string? SmartSelectionSystemPreamble { get; set; }
        public string? SmartSelectionUserPrompt { get; set; }

        // Per PromptType guidance (key = PromptType.ToString())
        public Dictionary<string, string> TypeGuidances { get; set; } = new Dictionary<string, string>();
    }
}
