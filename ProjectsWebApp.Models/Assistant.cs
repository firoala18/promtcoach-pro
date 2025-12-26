using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectsWebApp.Models
{
    public class Assistant
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;

       
      
        public string? Description { get; set; }

        [MaxLength(512)]
        public string? AvatarUrl { get; set; }

        [Required]
        public string SystemPrompt { get; set; } = string.Empty;

        [MaxLength(2048)]
        public string? Goals { get; set; }

        [MaxLength(512)]
        public string? Licenses { get; set; }

        // Optional reflective notes about this assistant (e.g. didactic hints, usage reflections)
        [MaxLength(4000)]
        public string? Reflektion { get; set; }

        
        public string? AuthorName { get; set; }

        [MaxLength(100)]
        public string? CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // When true, assistant is visible across all groups (admin/coach toggled)
        public bool IsGlobal { get; set; } = false;
    }
}
