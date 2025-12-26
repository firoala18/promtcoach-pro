using System;

namespace ProjectsWebApp.Models
{
    public class DozentGroupOwnership
    {
        public int Id { get; set; }
        public string DozentUserId { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
