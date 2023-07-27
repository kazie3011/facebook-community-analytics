namespace FacebookCommunityAnalytics.Api.UserWaves
{
    public static class UserWaveConsts
    {
        private const string DefaultSorting = "{0}WaveType asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "UserWave." : string.Empty);
        }

    }
}