// Dto/FilterCategoriesPayloadDto.cs
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dto
{
    public class FilterCategoriesPayloadDto
    {
        [JsonPropertyName("data")]
        [JsonPropertyOrder(1)]
        public List<FilterCategoryDto> Data { get; set; } = new();

        [JsonPropertyName("hash")]
        [JsonPropertyOrder(2)]
        public string Hash { get; set; } = "";
    }
}