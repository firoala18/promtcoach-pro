using System;

namespace ProjectsWebApp.Models
{
    public class ApiKeySetting
    {
        public int Id { get; set; }
        public string? OpenAIKey { get; set; }
        public string? OpenAIEmbeddingsKey { get; set; }
        public string? OpenAIBaseUrl { get; set; }
        public string? OpenAIModel { get; set; }

        public string? KisskiApiKey { get; set; }
        public string? KisskiBaseUrl { get; set; }
        public string? KisskiModel { get; set; }

        public string? GeminiApiKey { get; set; }
        public string? GeminiModel { get; set; }

        public string? ActiveProvider { get; set; } // "kisski" | "openai" | "gemini"
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedByUserId { get; set; }

        public DateTime? OpenAIUpdatedAt { get; set; }
        public DateTime? KisskiUpdatedAt { get; set; }
        public DateTime? GeminiUpdatedAt { get; set; }
    }
}
