namespace FacebookCommunityAnalytics.Api.Campaigns
{
    public class AuthorStatistic
    {
        public string Author { get; set; }
        public int PostCount { get; set; }
        public int TotalReaction { get; set; }
        public int CampaignCount { get; set; }
        public decimal EvaluationRate { get; set; }
    }
}