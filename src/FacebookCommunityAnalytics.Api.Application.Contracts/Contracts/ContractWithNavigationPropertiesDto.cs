using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Users;

namespace FacebookCommunityAnalytics.Api.Contracts
{
    public class ContractWithNavigationPropertiesDto
    {
        public ContractDto Contract { get; set; }
        public CampaignDto Campaign { get; set; }
        public PartnerDto Partner { get; set; }
        public AppUserDto SalePerson { get; set; }
        public List<ContractTransactionDto> ContractTransactions { get; set; }

        public ContractWithNavigationPropertiesDto()
        {
            ContractTransactions = new List<ContractTransactionDto>();
            Contract = new ContractDto();
        }
    }
}