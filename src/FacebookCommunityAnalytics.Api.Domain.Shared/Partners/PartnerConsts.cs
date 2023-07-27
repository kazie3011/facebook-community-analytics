namespace FacebookCommunityAnalytics.Api.Partners
{
    public static class PartnerConsts
    {
        private const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Partner." : string.Empty);
        }

    }
}