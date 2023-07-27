namespace FacebookCommunityAnalytics.Api.Core.Enums
{
    public enum CrawlType
    {
        None = 0,
        
        GroupInfos = 10,
        GroupUsers = 11,
        GroupPosts = 12,
        GroupSelectivePosts = 13,
        
        PageInfos = 20,
        PagePosts = 21,
        PageSelectivePosts = 22,
        
        ByHashTags = 30, 
        
        TiktokChannelStats = 40,
        TiktokVideos = 41,
        
        LoginManual = 900,
        LoginAuto = 901,
        JoinGroups = 9002,
    }
}