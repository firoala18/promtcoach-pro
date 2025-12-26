using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectsWebApp.Models
{
    public class SemanticIndexEntry
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string EntityType { get; set; } = string.Empty;

        [Required]
        [MaxLength(64)]
        public string EntityId { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? EmbeddingJson { get; set; }

        public double VectorNorm { get; set; }

        [MaxLength(100)]
        public string? Group { get; set; }

        [MaxLength(100)]
        public string? OwnerUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
