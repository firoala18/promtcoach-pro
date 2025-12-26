using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using ProjectsWebApp.Models;

namespace ProjectsWebApp.DataAccsess.Services.Calsses
{
    /// <summary>
    /// Default implementation that persists <see cref="UserActivityEvent"/> rows.
    /// Any failure in logging is swallowed (debug-logged) so analytics never
    /// breaks the main user flow.
    /// </summary>
    public sealed class UserActivityLogger : IUserActivityLogger
    {
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        private readonly ApplicationDbContext _db;
        private readonly ILogger<UserActivityLogger> _logger;

        public UserActivityLogger(ApplicationDbContext db, ILogger<UserActivityLogger> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task LogAsync(
            string userId,
            string? group,
            string eventType,
            int? durationSeconds = null,
            object? metadata = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(eventType))
                return;

            try
            {
                string? json = null;
                if (metadata != null)
                {
                    try
                    {
                        json = JsonSerializer.Serialize(metadata, JsonOpts);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ex, "Failed to serialize UserActivityEvent metadata.");
                    }
                }

                var ev = new UserActivityEvent
                {
                    UserId = userId,
                    Group = string.IsNullOrWhiteSpace(group) ? null : group.Trim(),
                    EventType = eventType,
                    DurationSeconds = durationSeconds,
                    MetadataJson = json,
                    CreatedAtUtc = DateTime.UtcNow
                };

                await _db.UserActivityEvents.AddAsync(ev, ct);
                await _db.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                // Never throw from analytics, just log on debug level
                _logger.LogDebug(ex, "Failed to log UserActivityEvent {EventType} for user {UserId}.", eventType, userId);
            }
        }
    }
}
