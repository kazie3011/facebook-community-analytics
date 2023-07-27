namespace FacebookCommunityAnalytics.Api.Core.Enums
{
    public enum CampaignStatus
    {
        Unknown = 1,
        Draft = 10,
        Started = 11,
        Hold = 12,
        Ended = 13,
        Archived = 99
    }
    
    public enum CampaignStatusFilter
    {
        NoSelect = 0,
        Unknown = 1,
        Draft = 10,
        Started = 11,
        Hold = 12,
        Ended = 13,
        Archived = 99
    }
}