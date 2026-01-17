using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models
{
    public class Skill
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int DisplayOrder { get; set; }
        public int DisplayOrder2 { get; set; }

        public string? IconPath { get; set; } // Optional for icons

        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }

}
