using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectsWebApp.Models
{
    public class GroupApiKeySetting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Group { get; set; } = string.Empty; // normalized group name ("Ohne Gruppe" allowed)

        // Provider keys
        public string? KisskiApiKey { get; set; }
        public string? KisskiBaseUrl { get; set; }
        public string? KisskiModel { get; set; }

        public string? OpenAIKey { get; set; }
        public string? OpenAIBaseUrl { get; set; }
        public string? OpenAIModel { get; set; }

        public string? GeminiApiKey { get; set; }
        public string? GeminiModel { get; set; }
        public string? ClaudeApiKey { get; set; }
        public string? ClaudeModel { get; set; }

        // Which provider is active for this group: "kisski" | "openai" | "gemini" | "claude"
        [MaxLength(32)]
        public string? ActiveProvider { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? UpdatedByUserId { get; set; }
    }
}
