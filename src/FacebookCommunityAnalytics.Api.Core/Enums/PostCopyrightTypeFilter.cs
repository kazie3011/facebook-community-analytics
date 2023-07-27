namespace FacebookCommunityAnalytics.Api.Core.Enums
{
    public enum PostCopyrightTypeFilter
    {
        NoSelect,
        Unknown,
        Exclusive, // for creator or the first post
        Copy, // copy
        VIA, // share VIA
        Remake // remake from exclusive post
    }
}