namespace FacebookCommunityAnalytics.Api.Posts
{
    public static class PostConsts
    {
        private const string DefaultSorting = "{0}PostContentType asc";
        public const string True = "True";
        public const string False = "False";
        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Post." : string.Empty);
        }
    }
}