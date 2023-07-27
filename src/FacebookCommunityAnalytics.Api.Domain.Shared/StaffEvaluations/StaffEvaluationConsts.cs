namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class StaffEvaluationConsts
    {
        private const string DefaultSorting = "{0}CreatedDateTime asc";
        public const string True = "True";
        public const string False = "False";
        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "StaffEvaluation." : string.Empty);
        }
    }
}