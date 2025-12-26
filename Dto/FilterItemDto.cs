// Dto/FilterItemDto.cs
using System.Text.Json.Serialization;

namespace Dto
{
    public class FilterItemDto
    {
        [JsonPropertyOrder(1)]
        public int? Id { get; set; }

        [JsonPropertyOrder(2)]
        public string Title { get; set; } = "";

        [JsonPropertyOrder(3)]
        public string Info { get; set; } = "";

        [JsonPropertyOrder(4)]
        public string Instruction { get; set; } = "";

        [JsonPropertyOrder(5)]
        public int SortOrder { get; set; }

        // Stable, content-based identifier to help match items across exports where DB Ids may differ
        [JsonPropertyOrder(6)]
        public string? StableKey { get; set; }
    }
}