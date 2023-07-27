namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public static class UserCompensationConsts
    {
        private const string DefaultSorting = "{0}UserId asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "UserCompensation." : string.Empty);
        }

    }
}