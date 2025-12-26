using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectsWebApp.Models
{
    public class AssistantKnowledgeChunk
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AssistantId { get; set; }

        [ForeignKey(nameof(AssistantId))]
        public Assistant Assistant { get; set; }

        [Required]
        public int FileId { get; set; }

        [ForeignKey(nameof(FileId))]
        public AssistantKnowledgeFile File { get; set; }

        [Required]
        public int OrderIndex { get; set; }

        [MaxLength(256)]
        public string SourcePage { get; set; } // e.g., "p. 3" or range

        [MaxLength(256)]
        public string Heading { get; set; }

        [Required]
        public string Content { get; set; }

        public int ApproxTokens { get; set; }
        public int OverlapTokens { get; set; }

        // Store embedding as JSON string to keep EF simple
        [Required]
        public string EmbeddingJson { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
