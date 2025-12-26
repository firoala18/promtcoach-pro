using Dto.PromptFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dto
{
    // what the model sends back
    public sealed class PromptFilterPayloadDto
    {
        [JsonPropertyName("data")]
        public List<PromptFilterCategoryDto> Data { get; init; } = new();

        [JsonPropertyName("hash")]
        public string Hash { get; init; } = "";
    }
}
