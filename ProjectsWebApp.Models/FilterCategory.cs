using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProjectsWebApp.Models
{

    public enum ItemSortMode
    {
        SortOrder = 0,   // manual (drag & drop / SortOrder)
        Alphabetic = 1    // A–Z by Title
    }

    public class FilterCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g. "Schlüsselbegriffe", "Zielgruppe", etc.
        public PromptType Type { get; set; }
        public int DisplayOrder { get; set; } = 0;

        public bool IsAiGenerated { get; set; }           // ← flag to split sections
        public Guid? AiBatchId { get; set; }              // ← optional: group one AI run

        public string? UserId { get; set; }

        public DateTime? CreatedAt { get; set; }

        // When true, hide from Dozent/Nutzer views; Admin/Coach still see it
        public bool IsHidden { get; set; } = false;

        public ItemSortMode ItemSortMode { get; set; } = ItemSortMode.SortOrder;
        public ICollection<FilterItem> FilterItems { get; set; }

    }

}
