using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProjectsWebApp.Models
{
    public class PromptImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ValidateNever]
        public string ImagePath { get; set; } = string.Empty;

        public int PromptTemplateId { get; set; }

        [ForeignKey(nameof(PromptTemplateId))]
        [ValidateNever]
        public PromptTemplate PromptTemplate { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
