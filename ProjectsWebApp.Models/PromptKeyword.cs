using System.ComponentModel.DataAnnotations;

namespace ProjectsWebApp.Models
{
    public class PromptKeyword
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Text { get; set; } = string.Empty; // e.g. "KI-Assistent", "Infografik"
    }
}
