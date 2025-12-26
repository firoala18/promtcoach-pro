using System;

namespace ProjectsWebApp.Models
{
    public class AssistantGroup
    {
        public int Id { get; set; }
        public int AssistantId { get; set; }
        public int? GroupId { get; set; }
        public string Group { get; set; } = string.Empty;
    }
}
