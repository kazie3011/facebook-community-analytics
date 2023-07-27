namespace FacebookCommunityAnalytics.Api.UserInfos
{
    public static class UserInfoConsts
    {
        private const string DefaultSorting = "{0}Code asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "UserInfo." : string.Empty);
        }

    }
}