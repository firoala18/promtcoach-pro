using System.Threading;
using System.Threading.Tasks;

namespace ProjectsWebApp.DataAccsess.Services.Interfaces
{
    /// <summary>
    /// Abstraction for writing fine-grained analytics events without
    /// coupling controllers directly to the data model.
    /// </summary>
    public interface IUserActivityLogger
    {
        Task LogAsync(
            string userId,
            string? group,
            string eventType,
            int? durationSeconds = null,
            object? metadata = null,
            CancellationToken ct = default);
    }
}
