using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectsWebApp.Models
{
    public class AssistantEmbedding
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AssistantId { get; set; }

        [ForeignKey(nameof(AssistantId))]
        public Assistant Assistant { get; set; } = null!;

        [MaxLength(260)]
        public string? SourceFileName { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public string EmbeddingJson { get; set; } = string.Empty;

        public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
