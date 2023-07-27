namespace FacebookCommunityAnalytics.Api.Accounts
{
    public static class AccountConsts
    {
        private const string DefaultSorting = "{0}Username asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Account." : string.Empty);
        }

    }
}