using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectsWebApp.Models
{
    public class AssistantKnowledgeFile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AssistantId { get; set; }

        [ForeignKey(nameof(AssistantId))]
        public Assistant Assistant { get; set; }

        [Required, MaxLength(260)]
        public string FileName { get; set; }

        [MaxLength(128)]
        public string ContentType { get; set; }

        [Required]
        public long SizeBytes { get; set; }

        [Required]
        public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;

        // Physical storage under wwwroot/uploads/assistant_knowledge/{assistantId}/{guid}_{filename}
        [Required, MaxLength(512)]
        public string StoragePath { get; set; }

        // Optional original pages count (if PDF)
        public int? PageCount { get; set; }

        // Processing state flags
        public bool Processed { get; set; }
        [MaxLength(2000)]
        public string LastError { get; set; }

        public ICollection<AssistantKnowledgeChunk> Chunks { get; set; }
    }
}
