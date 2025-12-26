using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models
{
    public class RegistrationCode
    {
        public int Id { get; set; }

        /// <summary>
        /// Der Einladungscode
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Ist der Code aktuell aktiv (gültig)?
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// (Optional) Beschreibung oder Notiz
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// Optionale Gruppenzuordnung für diesen Einladungscode
        /// z. B. "Gruppe A", "Kurs 2025/WS", etc.
        /// </summary>
        public string? Group { get; set; }

        /// <summary>
        /// Wenn true, handelt es sich um einen speziellen Dozenten‑Einladungscode.
        /// Neue Benutzer, die diesen Code bei der Registrierung verwenden,
        /// erhalten standardmäßig die Rolle "Dozent".
        /// </summary>
        public bool IsDozentCode { get; set; } = false;

        /// <summary>
        /// Gilt nur für Dozenten‑Codes. Wenn true, werden neu registrierte
        /// Dozent:innen zusätzlich als Besitzer:in der zugehörigen Gruppe
        /// eingetragen (DozentGroupOwnership + Mitgliedschaft).
        /// </summary>
        public bool DozentBecomesOwner { get; set; } = false;
    }
}
