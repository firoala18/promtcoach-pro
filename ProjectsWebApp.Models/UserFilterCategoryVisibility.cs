using System;

namespace ProjectsWebApp.Models
{
    public class UserFilterCategoryVisibility
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int FilterCategoryId { get; set; }
        public bool IsHidden { get; set; }
        public DateTime CreatedAt { get; set; }

        public FilterCategory FilterCategory { get; set; }
    }
}
