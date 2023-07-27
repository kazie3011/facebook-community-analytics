namespace FacebookCommunityAnalytics.Api.UncrawledPosts
{
    public static class UncrawledPostConsts
    {
        private const string DefaultSorting = "{0}Url asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "UncrawledPost." : string.Empty);
        }

    }
}