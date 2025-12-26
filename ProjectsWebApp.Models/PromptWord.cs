using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models
{
    public class PromptWord
    {
        public int Id { get; set; }

        [Required, MaxLength(40)]
        public string Text { get; set; }   // e.g. „Analysiere“

        //public int SortOrder { get; set; } // 1, 2, 3 …  (for the drag‑n‑drop list)
    }
}

