namespace FacebookCommunityAnalytics.Api.Core.Enums
{
    public enum ContractStatus
    {
        FilterNoSelect = 0,
        Unknown = 1,
        
        Pending = 10,
        
        ContractPendingSend = 20,
        ContractSent = 21,
        ContractSigned = 22,
        
        MoUPendingSend = 30,
        MoUSent = 31,
        MoUSigned = 32
    }
}