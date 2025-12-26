using System;

namespace ProjectsWebApp.Models
{
    public class PromptFeatureSetting
    {
        public int Id { get; set; }
        public bool EnableFilterGeneration { get; set; } = true;
        public bool EnableSmartSelection { get; set; } = true;
        public bool EnableAnalytics { get; set; } = true;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? UpdatedByUserId { get; set; }
    }
}
