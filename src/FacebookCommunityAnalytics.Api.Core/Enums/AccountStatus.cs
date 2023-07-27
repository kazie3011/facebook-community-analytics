namespace FacebookCommunityAnalytics.Api.Core.Enums
{
    public enum AccountStatus
    {
        Unknown = 1,
        
        New = 10,
        
        Active = 20,
        Deactive = 21,
        Ready = 22,
        
        LoginApprovalNeeded = 30,
        BlockedTemporary = 31,
        
        Banned = 99
    }
}