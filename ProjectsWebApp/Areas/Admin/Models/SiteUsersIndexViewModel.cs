using System.Collections.Generic;

namespace ProjectsWebApp.Areas.Admin.Models
{
    public sealed class SiteUsersIndexViewModel
    {
        public List<SiteUsersGroupBucket> Groups { get; set; } = new();
        public int TotalUsers { get; set; }

        // Search
        public string? SearchString { get; set; }

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

    public sealed class SiteUsersGroupBucket
    {
        public string GroupName { get; set; } = string.Empty;
        public List<SiteUsersUserEntry> Users { get; set; } = new();
    }

    public sealed class SiteUsersUserEntry
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
