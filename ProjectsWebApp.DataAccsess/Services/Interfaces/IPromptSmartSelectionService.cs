using System.Threading;
using System.Threading.Tasks;
using Dto.PromptFilters;
using ProjectsWebApp.Models;

namespace ProjectsWebApp.DataAccsess.Services.Interfaces
{
    public interface IPromptSmartSelectionService
    {
        Task<ProjectsWebApp.DataAccsess.Services.Calsses.SmartSelectionResultDto> SelectAsync(
            PromptFormDto form,
            PromptType targetType,
            CancellationToken ct = default);

        Task<ProjectsWebApp.DataAccsess.Services.Calsses.SmartSelectionResultDto> SelectAsync(
            PromptFormDto form,
            PromptType targetType,
            string opId,
            CancellationToken ct = default);

        // Deep mode overloads (use higher similarity threshold and stricter selection)
        Task<ProjectsWebApp.DataAccsess.Services.Calsses.SmartSelectionResultDto> SelectAsync(
            PromptFormDto form,
            PromptType targetType,
            bool deep,
            CancellationToken ct = default);

        Task<ProjectsWebApp.DataAccsess.Services.Calsses.SmartSelectionResultDto> SelectAsync(
            PromptFormDto form,
            PromptType targetType,
            bool deep,
            string opId,
            CancellationToken ct = default);

        // Model override variants with distinct name to avoid signature clashes
        Task<ProjectsWebApp.DataAccsess.Services.Calsses.SmartSelectionResultDto> SelectWithModelAsync(
            PromptFormDto form,
            PromptType targetType,
            bool deep,
            string? modelIdOverride,
            CancellationToken ct = default);

        Task<ProjectsWebApp.DataAccsess.Services.Calsses.SmartSelectionResultDto> SelectWithModelAsync(
            PromptFormDto form,
            PromptType targetType,
            bool deep,
            string? modelIdOverride,
            string opId,
            CancellationToken ct = default);
    }
}
