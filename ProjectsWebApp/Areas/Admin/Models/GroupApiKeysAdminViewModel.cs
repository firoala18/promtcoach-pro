using System.ComponentModel.DataAnnotations;

namespace ProjectsWebApp.Areas.Admin.Models
{
    public class GroupApiKeysAdminViewModel
    {
        public int GroupId { get; set; }

        [Required]
        public string GroupName { get; set; } = string.Empty;

        [Display(Name = "Aktiver Anbieter")]
        public string? ActiveProvider { get; set; }

        // Kisski
        public string? KisskiApiKey { get; set; }
        public string? KisskiModel { get; set; }

        // OpenAI-compatible
        public string? OpenAIKey { get; set; }
        public string? OpenAIModel { get; set; }

        // Gemini
        public string? GeminiApiKey { get; set; }
        public string? GeminiModel { get; set; }

        // Claude
        public string? ClaudeApiKey { get; set; }
        public string? ClaudeModel { get; set; }
    }
}
