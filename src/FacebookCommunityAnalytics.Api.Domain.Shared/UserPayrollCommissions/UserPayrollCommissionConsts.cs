namespace FacebookCommunityAnalytics.Api.UserPayrollCommissions
{
    public static class UserPayrollCommissionConsts
    {
        private const string DefaultSorting = "{0}OrganizationId asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "UserPayrollCommission." : string.Empty);
        }

    }
}