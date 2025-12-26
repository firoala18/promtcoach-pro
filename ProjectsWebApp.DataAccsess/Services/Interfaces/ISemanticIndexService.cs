using System.Threading;
using System.Threading.Tasks;
using ProjectsWebApp.Models;

namespace ProjectsWebApp.DataAccsess.Services.Interfaces
{
    public interface ISemanticIndexService
    {
        Task UpsertFilterItemAsync(FilterItem item, string? group, string? ownerUserId, CancellationToken ct);
        Task RemoveForFilterItemAsync(int itemId, CancellationToken ct);
        Task<int> BackfillAllFilterItemsAsync(PromptType? type, CancellationToken ct);
        Task<int> BackfillUserFilterItemsAsync(string ownerUserId, PromptType? type, CancellationToken ct);
    }
}
