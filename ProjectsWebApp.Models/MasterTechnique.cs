using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectsWebApp.Models
{
    public class MasterTechnique
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(160)]
        public string Name { get; set; } = string.Empty;

        public bool Enabled { get; set; } = true;

        public int DisplayOrder { get; set; } = 0;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
