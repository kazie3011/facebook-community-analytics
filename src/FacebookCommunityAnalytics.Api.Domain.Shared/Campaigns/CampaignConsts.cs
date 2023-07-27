namespace FacebookCommunityAnalytics.Api.Campaigns
{
    public static class CampaignConsts
    {
        private const string DefaultSorting = "{0}Name asc";
        public const string HashTagFontSize = "font-size: 13px";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Campaign." : string.Empty);
        }

    }
}