namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public static class TiktokConsts
    {
        private const string DefaultSorting = "{0}CreatedDateTime asc";
        public const string True = "True";
        public const string False = "False";
        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Tiktok." : string.Empty);
        }
    }
    public static class TiktokMCNConsts
    {
        private const string DefaultSorting = "{0}Name asc";
        public const string True = "True";
        public const string False = "False";
        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "TikTokMCN." : string.Empty);
        }
    }
}