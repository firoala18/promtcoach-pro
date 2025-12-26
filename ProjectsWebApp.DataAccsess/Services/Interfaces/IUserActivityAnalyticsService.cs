using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectsWebApp.Models;

namespace ProjectsWebApp.DataAccsess.Services.Interfaces
{
    /// <summary>
    /// Phase 2: analytics aggregation service that turns the raw
    /// UserActivityEvents log into compact per-day/per-user/per-group
    /// statistics for fast dashboards.
    /// </summary>
    public interface IUserActivityAnalyticsService
    {
        /// <summary>
        /// Rebuilds daily stats from UserActivityEvents for the given
        /// UTC date range (inclusive). Existing stats in this range
        /// are replaced.
        /// </summary>
        Task RebuildUserDailyStatsAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);

        /// <summary>
        /// Returns daily stats for a specific user across all groups
        /// in the given UTC date range (inclusive).
        /// </summary>
        Task<IReadOnlyList<UserDailyActivityStat>> GetUserDailyStatsAsync(string userId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);

        /// <summary>
        /// Returns daily stats for a specific group (all users) in the
        /// given UTC date range (inclusive).
        /// </summary>
        Task<IReadOnlyList<UserDailyActivityStat>> GetGroupDailyStatsAsync(string group, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);

        /// <summary>
        /// Returns daily stats for a specific user constrained to a
        /// specific group in the given UTC date range (inclusive).
        /// </summary>
        Task<IReadOnlyList<UserDailyActivityStat>> GetUserDailyStatsForGroupAsync(string userId, string group, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);
    }
}
