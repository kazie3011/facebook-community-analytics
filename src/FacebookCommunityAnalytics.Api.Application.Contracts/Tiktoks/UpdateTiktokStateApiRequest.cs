namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public class UpdateTiktokStateApiRequest : GetTiktoksInputExtend
    {
        public bool IsNew { get; set; }
    }
}