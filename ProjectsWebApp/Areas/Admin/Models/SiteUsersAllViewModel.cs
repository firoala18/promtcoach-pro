using System.Collections.Generic;

namespace ProjectsWebApp.Areas.Admin.Models
{
    public sealed class SiteUsersAllViewModel
    {
        public List<SiteUsersUserEntry> Users { get; set; } = new();
        public int TotalUsers { get; set; }

        public string? SearchString { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
