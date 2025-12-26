using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models
{
    public class EventEntry
    {
        public int Id { get; set; }

        [Required]
        public DateTime Termin { get; set; }

        [Required]
        public string Titel { get; set; }

        public string? Referent { get; set; }
        public string? Beschreibung { get; set; } // CKEditor content
        public string? Arbeitseinheiten { get; set; }
        public string? Veranstaltungsort { get; set; }
        public string? Organisation { get; set; }
        public string? Hinweis { get; set; }
        public string? InfosFuerTeilnehmer { get; set; }
        public string? Art { get; set; }


        [Display(Name = "Beginn")]
        public TimeSpan? Startzeit { get; set; }

        [Display(Name = "Ende")]
        public TimeSpan? Endzeit { get; set; }

    }

}
