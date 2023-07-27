namespace FacebookCommunityAnalytics.Api.UserSalaryConfigurations
{
    public static class UserSalaryConfigurationConsts
    {
        private const string DefaultSorting = "{0}UserId asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "UserSalaryConfiguration." : string.Empty);
        }

    }
}