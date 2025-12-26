using System.Threading;
using System.Threading.Tasks;

namespace ProjectsWebApp.DataAccsess.Services.Interfaces
{
    public sealed record AiProviderResolution(string Provider, string BaseUrl, string ApiKey, string ModelId);

    public interface IAiProviderResolver
    {
        Task<AiProviderResolution> ResolveChatAsync(string? userId, string? modelOverride, CancellationToken ct);
        Task<AiProviderResolution> ResolveGroupChatAsync(string groupName, string? modelOverride, CancellationToken ct);
        Task<string> ResolveEmbeddingsKeyAsync(CancellationToken ct);
    }
}
