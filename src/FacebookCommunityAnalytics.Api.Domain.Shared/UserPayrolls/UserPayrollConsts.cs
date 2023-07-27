namespace FacebookCommunityAnalytics.Api.UserPayrolls
{
    public static class UserPayrollConsts
    {
        private const string DefaultSorting = "{0}Code asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "UserPayroll." : string.Empty);
        }

    }
}