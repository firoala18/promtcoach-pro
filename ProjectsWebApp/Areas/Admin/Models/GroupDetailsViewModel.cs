using System;
using System.Collections.Generic;

namespace ProjectsWebApp.Areas.Admin.Models
{
    public class GroupDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<string> Owners { get; set; } = new List<string>();

        public int MemberCount { get; set; }
        public int AdminCount { get; set; }
        public int DozentCount { get; set; }
        public int RegularUserCount { get; set; }

        public int PromptCount { get; set; }
        public int AssistantCount { get; set; }
        public int InviteCodeCount { get; set; }

        public List<GroupMemberRow> Members { get; set; } = new List<GroupMemberRow>();

        public List<GroupInviteCodeRow> InviteCodes { get; set; } = new List<GroupInviteCodeRow>();

        // Optional role aliases (e.g. Forscher*in, Gruppen‑Manager*in, Student*in)
        // keyed by Identity user id, aligned with ManageUsers.
        public Dictionary<string, string?> RoleAliasesByUserId { get; set; } = new Dictionary<string, string?>();

        // If true, this group has at least one owner with the ApiManager role;
        // in this case group-specific API keys are visually disabled and
        // global API keys are enforced.
        public bool HasApiManagerOwner { get; set; }

        public bool CanDeleteGroup { get; set; }

        // Core configuration editors used on the Admin Groups dashboard
        public GroupPromptsAdminViewModel? Prompts { get; set; }
        public GroupApiKeysAdminViewModel? ApiKeys { get; set; }

        // Simple analytics summary for the last 30 days (UTC) for this group
        public DateTime? AnalyticsFromUtc { get; set; }
        public DateTime? AnalyticsToUtc { get; set; }

        public int TotalEventsLast30Days { get; set; }
        public int TotalLoginLast30Days { get; set; }
        public int TotalPromptGenerateLast30Days { get; set; }
        public int TotalFilterGenerateLast30Days { get; set; }
        public int TotalSmartSelectionLast30Days { get; set; }
        public int TotalAssistantActionsLast30Days { get; set; }
        public int TotalPromptSaveCollectionLast30Days { get; set; }
        public int TotalPromptPublishLibraryLast30Days { get; set; }
        public int ActiveUsersLast30Days { get; set; }

        public List<GroupUserAnalyticsRow> TopUsersLast30Days { get; set; } = new List<GroupUserAnalyticsRow>();

        public bool AnalyticsEnabled { get; set; }
    }

    public class GroupInviteCodeRow
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Note { get; set; }

        public bool IsDozentCode { get; set; }
        public bool DozentBecomesOwner { get; set; }
    }

    public class GroupMemberRow
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string Roles { get; set; } = string.Empty;

        public int GroupCount { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    public class GroupUserAnalyticsRow
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalEvents { get; set; }
    }
}
