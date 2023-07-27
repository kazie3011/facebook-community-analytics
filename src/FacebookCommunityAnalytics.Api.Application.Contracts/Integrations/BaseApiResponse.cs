namespace FacebookCommunityAnalytics.Api.Integrations
{
    public abstract class BaseApiResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}