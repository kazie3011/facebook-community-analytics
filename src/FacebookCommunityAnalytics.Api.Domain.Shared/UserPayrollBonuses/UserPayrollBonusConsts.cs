namespace FacebookCommunityAnalytics.Api.UserPayrollBonuses
{
    public static class UserPayrollBonusConsts
    {
        private const string DefaultSorting = "{0}PayrollBonusType asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "UserPayrollBonus." : string.Empty);
        }

    }
}