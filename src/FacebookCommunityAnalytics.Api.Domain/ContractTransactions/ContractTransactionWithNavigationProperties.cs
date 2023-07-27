using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Users;

namespace FacebookCommunityAnalytics.Api.ContractTransactions
{
    public class ContractTransactionWithNavigationProperties
    {
        public ContractTransaction ContractTransaction { get; set; }
        public AppUser             SalePerson          { get; set; }
        public Contract            Contract            { get; set; }
        public Campaign            ContractCampaign    { get; set; }
        public Partner             ContractPartner     { get; set; }
        public AppUser             ContractSalePerson  { get; set; }
    }
}