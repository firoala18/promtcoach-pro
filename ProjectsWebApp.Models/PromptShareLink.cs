using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectsWebApp.Models
{
    public class PromptShareLink
    {
        public int Id { get; set; }

        [Required]
        public int PromptTemplateId { get; set; }

        /// <summary>
        /// Normalized Gruppenname (z.B. "Ohne Gruppe" oder exakter Gruppenname).
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// Öffentlicher Token für die URL (unguessable, url-safe).
        /// </summary>
        [Required]
        [MaxLength(128)]
        public string PublicId { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? CreatedByUserId { get; set; }

        public DateTime? ExpiresAtUtc { get; set; }
    }
}
