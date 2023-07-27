namespace FacebookCommunityAnalytics.Api.Payrolls
{
    public static class PayrollConsts
    {
        private const string DefaultSorting = "{0}Code asc";
        public const string HappyDay = "HAPPYDAY";
        public const string HappyDay_ = "HAPPYDAY_";
        public const string Draft = "DRAFT";
        public const string Draft_ = "DRAFT_";
        public const string Payroll = "Payroll";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Payroll." : string.Empty);
        }

    }
}