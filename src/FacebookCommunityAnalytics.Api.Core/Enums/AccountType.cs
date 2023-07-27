namespace FacebookCommunityAnalytics.Api.Core.Enums
{
    public enum AccountType
    {
        Unknown = 1,
        All = 10,
        NodeFacebook = 100,
        NodeInstagram = 101,
        NodeCampaign = 102,

        NETFacebookGroupSelectivePost = 200,
        NETFacebookGroupPost = 201,
        NETFacebookGroupUserPost = 202,

        NETFacebookPagePost = 300,

        NETInstagram = 400,

        NETTelegram = 500,

        NETWebsite = 600,

        TestLogin = 1000,
        TestLoginComplete = 1001,

        AutoPostGroup = 2000,
        AutoPostPage = 2001,
        AutoPostInstagram = 2002,
    }
}
