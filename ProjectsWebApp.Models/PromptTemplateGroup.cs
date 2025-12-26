using System;

namespace ProjectsWebApp.Models
{
    public class PromptTemplateGroup
    {
        public int Id { get; set; }
        public int PromptTemplateId { get; set; }
        public int? GroupId { get; set; }
        public string Group { get; set; } = string.Empty;
    }
}
