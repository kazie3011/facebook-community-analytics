namespace FacebookCommunityAnalytics.Api.ScheduledPosts
{
    public static class ScheduledPostConsts
    {
        private const string DefaultSorting = "{0}PostedAt desc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "ScheduledPost." : string.Empty);
        }

    }
}