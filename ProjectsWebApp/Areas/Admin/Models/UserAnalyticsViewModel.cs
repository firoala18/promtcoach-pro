using System;
using System.Collections.Generic;
using ProjectsWebApp.Models;

namespace ProjectsWebApp.Areas.Admin.Models
{
    public class UserAnalyticsViewModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string Roles { get; set; } = string.Empty;

        public DateTime FromUtc { get; set; }
        public DateTime ToUtc { get; set; }

        public List<UserDailyActivityStat> DailyStats { get; set; } = new List<UserDailyActivityStat>();

        public int TotalEvents { get; set; }
        public int LoginCount { get; set; }
        public int PromptGenerateCount { get; set; }
        public int FilterGenerateCount { get; set; }
        public int SmartSelectionCount { get; set; }
        public int AssistantChatCount { get; set; }
        public int PromptSaveCollectionCount { get; set; }
        public int PromptPublishLibraryCount { get; set; }
        public int TotalDurationSeconds { get; set; }

        public bool AnalyticsEnabled { get; set; }
    }
}
