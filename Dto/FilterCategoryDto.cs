// Dto/FilterCategoryDto.cs
using ProjectsWebApp.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dto
{
    public class FilterCategoryDto
    {
        [JsonPropertyOrder(1)]
        public int? Id { get; set; }

        [JsonPropertyOrder(2)]
        public string Name { get; set; } = "";

        [JsonPropertyOrder(3)]
        public PromptType Type { get; set; }

        [JsonPropertyOrder(4)]
        public int DisplayOrder { get; set; }

        [JsonPropertyOrder(5)]
        public List<FilterItemDto> FilterItems { get; set; } = new();
    }
}