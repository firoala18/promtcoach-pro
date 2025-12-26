using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models
{
    public class SavedPromptVariation
    {
        public int Id { get; set; }
        public string VariationJson { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int SavedPromptId { get; set; }
        public virtual SavedPrompt SavedPrompt { get; set; } = null!;
    }

}
