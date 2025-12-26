using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dto;
using Dto.PromptFilters;
using ProjectsWebApp.Models;

namespace ProjectsWebApp.DataAccsess.Services.Interfaces
{
    public interface IPromptFilterAiService
    {
        Task<PromptFilterPayloadDto> GenerateAsync(PromptFormDto form, CancellationToken ct);
        Task<PromptFilterPayloadDto> GenerateAsync(PromptFormDto form, PromptType targetType, CancellationToken ct);
        Task<PromptFilterPayloadDto> GenerateAsync(PromptFormDto form, PromptType targetType, string? modelIdOverride, CancellationToken ct);
    }
}
