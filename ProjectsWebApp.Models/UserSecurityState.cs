using System;

namespace ProjectsWebApp.Models
{
    /// <summary>
    /// Tracks additional security state per Identity user: how often a lockout window
    /// has been triggered and whether the account is permanently locked and requires
    /// manual admin intervention.
    /// </summary>
    public class UserSecurityState
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        /// <summary>
        /// Number of lockout windows that have been triggered for this account.
        /// </summary>
        public int LockoutWindowCount { get; set; }

        /// <summary>
        /// When true, the account is considered permanently locked and only
        /// an admin can reactivate it.
        /// </summary>
        public bool IsPermanentlyLocked { get; set; }

        public DateTime? FirstLockoutAtUtc { get; set; }
        public DateTime? LastLockoutAtUtc { get; set; }
    }
}
