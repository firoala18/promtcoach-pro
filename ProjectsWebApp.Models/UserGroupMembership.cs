using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectsWebApp.Models
{
    public class UserGroupMembership
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Group name the user belongs to (derived from RegistrationCode.Group)
        /// </summary>
        public string? Group { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
