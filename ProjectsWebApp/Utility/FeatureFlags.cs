namespace ProjectsWebApp.Utility
{
    public static class FeatureFlags
    {
        // Admin‑toggleable at runtime (no DB persistence in this implementation)
        // Default: enabled
        public static volatile bool EnableFilterGeneration = true;
        public static volatile bool EnableSmartSelection = true;
        public static volatile bool EnablePromptTechnique = true;

        // Global analytics toggle (user activity tracking)
        // When false, no new UserActivityEvents are written.
        public static volatile bool EnableAnalytics = true;
    }
}
