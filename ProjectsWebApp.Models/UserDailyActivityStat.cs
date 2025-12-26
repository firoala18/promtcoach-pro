using System;

namespace ProjectsWebApp.Models
{
    /// <summary>
    /// Daily aggregated analytics per user and (optional) group.
    /// Built from <see cref="UserActivityEvent"/> and used for fast
    /// Admin/Dozent dashboards.
    /// </summary>
    public class UserDailyActivityStat
    {
        public int Id { get; set; }

        /// <summary>
        /// Identity user id (AspNetUsers.Id).
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Optional logical group name (Group.Name) for the day.
        /// </summary>
        public string? Group { get; set; }

        /// <summary>
        /// UTC date bucket (00:00:00 of the day in UTC).
        /// </summary>
        public DateTime DateUtc { get; set; }

        /// <summary>
        /// Total number of analytics events for this user/group/day.
        /// </summary>
        public int TotalEvents { get; set; }

        public int LoginCount { get; set; }
        public int PromptGenerateCount { get; set; }
        public int FilterGenerateCount { get; set; }
        public int SmartSelectionCount { get; set; }
        public int AssistantChatCount { get; set; }
        public int AssistantChatStreamCount { get; set; }
        public int PromptSaveCollectionCount { get; set; }
        public int PromptPublishLibraryCount { get; set; }

        /// <summary>
        /// Sum of DurationSeconds from underlying events (if used).
        /// </summary>
        public int TotalDurationSeconds { get; set; }

        /// <summary>
        /// When this aggregate row was last rebuilt.
        /// </summary>
        public DateTime LastUpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
