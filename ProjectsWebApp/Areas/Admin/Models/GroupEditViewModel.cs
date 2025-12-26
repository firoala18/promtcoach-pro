using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectsWebApp.Areas.Admin.Models
{
    public class GroupEditViewModel
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Name der Gruppe")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        [Display(Name = "Beschreibung")]
        public string? Description { get; set; }

        public List<GroupOwnerOption> OwnerOptions { get; set; } = new List<GroupOwnerOption>();

        // Bound from checkboxes named "SelectedOwnerIds"
        public List<string> SelectedOwnerIds { get; set; } = new List<string>();

        // For display of current owners (used especially for Dozenten who cannot see the full list)
        public List<GroupOwnerOption> CurrentOwners { get; set; } = new List<GroupOwnerOption>();

        [EmailAddress]
        [Display(Name = "Neuer Besitzer (E-Mail)")]
        public string? AddOwnerEmail { get; set; }
    }

    public class GroupOwnerOption
    {
        public string UserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public class JoinGroupAdminViewModel
    {
        [Required]
        [Display(Name = "Einladungscode")]
        public string InviteCode { get; set; } = string.Empty;

        // Groups the current user already belongs to (with IDs for navigation)
        public List<MemberGroupItem> MemberGroups { get; set; } = new List<MemberGroupItem>();
    }

    public class MemberGroupItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
