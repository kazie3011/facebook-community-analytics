namespace FacebookCommunityAnalytics.Api.Core.Enums
{
    public enum UserPosition
    {
        FilterNoSelect = 0,
        Unknown = 1,
        Intern = 2,

        Community = 100,
        CommunityIntern = 101,
        CommunityStaff = 102,
        CommunityGroupLeader = 103,
        CommunityLeader = 104,
        CommunitySupervisor = 105,
        CommunityManager = 106,
        CommunityDirector = 107,
        
        CommunitySeedingStaff_ST1 = 140,
        CommunitySeedingLeader_ST1 = 142,
        CommunitySeedingStaff = 150,
        CommunitySeedingGroupLeader = 151,
        CommunitySeedingLeader = 152,
        
        CommunityAffiliateStaff = 160,
        CommunityAffiliateGroupLeader = 161,
        CommunityAffiliateLeader = 162,
        
        Content = 200,
        ContentIntern = 201,
        ContentStaff = 202,
        ContentExecutive = 203,
        ContentSeniorExecutive = 204,

        Sale = 300,
        SaleAdmin = 301,
        SaleAccount = 302,
        
        Tiktok = 400,
        TiktokCreator = 401,
        TiktokCoordinator = 402,
    }
}