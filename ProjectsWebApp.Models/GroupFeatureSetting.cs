using System;

namespace ProjectsWebApp.Models
{
    public class GroupFeatureSetting
    {
        public int Id { get; set; }
        public string Group { get; set; } = string.Empty; // e.g. "Seminar1" or "Ohne Gruppe"
        public bool EnableFilterGeneration { get; set; } = true;
        public bool EnableSmartSelection { get; set; } = true;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? UpdatedByUserId { get; set; }
    }
}
