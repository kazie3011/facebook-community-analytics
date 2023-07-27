namespace FacebookCommunityAnalytics.Api.AffiliateStats
{
    public static class AffiliateStatConsts
    {
        private const string DefaultSorting = "{0}AffiliateOwnershipType asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "AffiliateStat." : string.Empty);
        }

    }
}