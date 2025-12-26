using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using ProjectsWebApp.Models;

namespace ProjectsWebApp.DataAccsess.Services.Calsses
{
    /// <summary>
    /// Phase 2 analytics aggregation: turns the raw UserActivityEvents
    /// log into compact daily aggregates for fast dashboards.
    /// </summary>
    public sealed class UserActivityAnalyticsService : IUserActivityAnalyticsService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<UserActivityAnalyticsService> _logger;

        public UserActivityAnalyticsService(ApplicationDbContext db, ILogger<UserActivityAnalyticsService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task RebuildUserDailyStatsAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
        {
            if (toUtc < fromUtc)
                throw new ArgumentException("toUtc must be >= fromUtc");

            var fromDate = fromUtc.Date;
            var toDate = toUtc.Date;

            try
            {
                // 1) Remove existing stats in range (idempotent rebuild)
                var existing = await _db.UserDailyActivityStats
                    .Where(s => s.DateUtc >= fromDate && s.DateUtc <= toDate)
                    .ToListAsync(ct);

                if (existing.Count > 0)
                {
                    _db.UserDailyActivityStats.RemoveRange(existing);
                    await _db.SaveChangesAsync(ct);
                }

                // 2) Read raw events and group by day / user / group
                var grouped = await _db.UserActivityEvents
                    .Where(e => e.CreatedAtUtc.Date >= fromDate && e.CreatedAtUtc.Date <= toDate)
                    .GroupBy(e => new { Date = e.CreatedAtUtc.Date, e.UserId, e.Group })
                    .Select(g => new
                    {
                        g.Key.Date,
                        g.Key.UserId,
                        g.Key.Group,
                        TotalEvents = g.Count(),
                        LoginCount = g.Count(e => e.EventType == "login"),
                        PromptGenerateCount = g.Count(e => e.EventType == "prompt_generate"),
                        FilterGenerateCount = g.Count(e => e.EventType == "filter_generate"),
                        SmartSelectionCount = g.Count(e => e.EventType == "smart_selection"),
                        AssistantChatCount = g.Count(e => e.EventType == "assistant_chat"),
                        AssistantChatStreamCount = g.Count(e => e.EventType == "assistant_chat_stream"),
                        PromptSaveCollectionCount = g.Count(e => e.EventType == "prompt_save_collection"),
                        PromptPublishLibraryCount = g.Count(e => e.EventType == "prompt_publish_library"),
                        TotalDurationSeconds = g.Sum(e => e.DurationSeconds ?? 0)
                    })
                    .ToListAsync(ct);

                if (grouped.Count == 0)
                    return;

                // 3) Insert new aggregates
                foreach (var g in grouped)
                {
                    var row = new UserDailyActivityStat
                    {
                        UserId = g.UserId,
                        Group = string.IsNullOrWhiteSpace(g.Group) ? null : g.Group.Trim(),
                        DateUtc = g.Date,
                        TotalEvents = g.TotalEvents,
                        LoginCount = g.LoginCount,
                        PromptGenerateCount = g.PromptGenerateCount,
                        FilterGenerateCount = g.FilterGenerateCount,
                        SmartSelectionCount = g.SmartSelectionCount,
                        AssistantChatCount = g.AssistantChatCount,
                        AssistantChatStreamCount = g.AssistantChatStreamCount,
                        PromptSaveCollectionCount = g.PromptSaveCollectionCount,
                        PromptPublishLibraryCount = g.PromptPublishLibraryCount,
                        TotalDurationSeconds = g.TotalDurationSeconds,
                        LastUpdatedAtUtc = DateTime.UtcNow
                    };

                    await _db.UserDailyActivityStats.AddAsync(row, ct);
                }

                await _db.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to rebuild UserDailyActivityStats from {From} to {To}.", fromDate, toDate);
                throw;
            }
        }

        public async Task<IReadOnlyList<UserDailyActivityStat>> GetUserDailyStatsAsync(string userId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
        {
            var fromDate = fromUtc.Date;
            var toDate = toUtc.Date;

            return await _db.UserDailyActivityStats
                .AsNoTracking()
                .Where(s => s.UserId == userId && s.DateUtc >= fromDate && s.DateUtc <= toDate)
                .OrderBy(s => s.DateUtc)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<UserDailyActivityStat>> GetGroupDailyStatsAsync(string group, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(group))
                throw new ArgumentException("group is required", nameof(group));

            var fromDate = fromUtc.Date;
            var toDate = toUtc.Date;
            var grp = group.Trim();

            return await _db.UserDailyActivityStats
                .AsNoTracking()
                .Where(s => s.Group == grp && s.DateUtc >= fromDate && s.DateUtc <= toDate)
                .OrderBy(s => s.DateUtc)
                .ThenBy(s => s.UserId)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<UserDailyActivityStat>> GetUserDailyStatsForGroupAsync(string userId, string group, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(group))
                throw new ArgumentException("group is required", nameof(group));

            var fromDate = fromUtc.Date;
            var toDate = toUtc.Date;
            var grp = group.Trim();

            return await _db.UserDailyActivityStats
                .AsNoTracking()
                .Where(s => s.UserId == userId && s.Group == grp && s.DateUtc >= fromDate && s.DateUtc <= toDate)
                .OrderBy(s => s.DateUtc)
                .ToListAsync(ct);
        }
    }
}
