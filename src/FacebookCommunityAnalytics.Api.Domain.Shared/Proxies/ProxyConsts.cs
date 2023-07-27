namespace FacebookCommunityAnalytics.Api.Proxies
{
    public static class ProxyConsts
    {
        private const string DefaultSorting = "{0}Ip asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Proxy." : string.Empty);
        }

    }
}