namespace FacebookCommunityAnalytics.Api.AccountProxies
{
    public static class AccountProxyConsts
    {
        private const string DefaultSorting = "{0}Description asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "AccountProxy." : string.Empty);
        }

    }
}