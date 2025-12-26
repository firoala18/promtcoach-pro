using System;

namespace ProjectsWebApp.Models
{
    /// <summary>
    /// Fine-grained analytics event for user and group activity.
    /// Phase 1: used as an append-only log from which we can later
    /// derive per-user and per-group statistics.
    /// </summary>
    public class UserActivityEvent
    {
        public int Id { get; set; }

        /// <summary>
        /// Identity user id (AspNetUsers.Id).
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Optional logical group name (Group.Name) at the time of the event.
        /// For users without group, this may stay null or contain "Ohne Gruppe".
        /// </summary>
        public string? Group { get; set; }

        /// <summary>
        /// Simple event key, e.g. "login", "filter_generate", "smart_selection".
        /// </summary>
        public string EventType { get; set; } = string.Empty;

        /// <summary>
        /// Optional duration in seconds associated with the event
        /// (e.g. a session segment), if applicable.
        /// </summary>
        public int? DurationSeconds { get; set; }

        /// <summary>
        /// Optional JSON payload with extra details (model id, feature flags, etc.).
        /// </summary>
        public string? MetadataJson { get; set; }

        /// <summary>
        /// UTC timestamp when the event was created.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
