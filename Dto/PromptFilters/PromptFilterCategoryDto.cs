using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dto.PromptFilters
{

    // Dto/PromptFilters/PromptFilterCategoryDto.cs
    public sealed class PromptFilterCategoryDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = "";


        // NEW: mirror JSON "type" (int). We keep it numeric to match schema easily.
        [JsonPropertyName("type")]
        public int Type { get; init; } = 0;

        public int DisplayOrder { get; init; }
        [JsonPropertyName("filterItems")]
        public List<PromptFilterItemDto> Items { get; init; } = new();
    }
}
