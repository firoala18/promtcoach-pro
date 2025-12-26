using System;
using System.Collections.Generic;

namespace Dto.PromptFilters
{
    public sealed class GenerateFiltersRequestDto
    {
        public PromptFormDto Form { get; init; }
        public string TargetType { get; init; } = "Text"; // Text, Bild, Video, Sound, Akademisch, Meta
        public string? CategoryTitleMode { get; init; } = "ai"; // 'ai' or 'manual'
        public string? CategoryTitle { get; init; }
        public string? ModelId { get; init; }
        public string? GroupName { get; init; }
    }
}
