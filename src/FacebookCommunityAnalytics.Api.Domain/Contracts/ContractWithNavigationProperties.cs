using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Users;

namespace FacebookCommunityAnalytics.Api.Contracts
{
    public class ContractWithNavigationProperties
    {
        public Contract Contract { get; set; }
        public Campaign Campaign { get; set; }
        public Partner Partner { get; set; }
        public AppUser SalePerson { get; set; }
        public List<ContractTransaction> ContractTransactions { get; set; }

        public ContractWithNavigationProperties()
        {
            ContractTransactions = new List<ContractTransaction>();
        }
    }
}