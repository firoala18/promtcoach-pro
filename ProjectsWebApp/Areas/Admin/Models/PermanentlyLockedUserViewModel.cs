using System;

namespace ProjectsWebApp.Areas.Admin.Models
{
    public class PermanentlyLockedUserViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? UserName { get; set; }

        public int LockoutWindowCount { get; set; }
        public DateTime? FirstLockoutAtUtc { get; set; }
        public DateTime? LastLockoutAtUtc { get; set; }
    }
}
