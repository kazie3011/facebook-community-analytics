using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Users;

namespace FacebookCommunityAnalytics.Api.ContractTransactions
{
    public class ContractTransactionWithNavigationPropertiesDto
    {
        public ContractTransactionDto ContractTransaction { get; set; }
        public AppUserDto             SalePerson          { get; set; }
        public ContractDto            Contract            { get; set; }
        public CampaignDto            ContractCampaign    { get; set; }
        public PartnerDto             ContractPartner     { get; set; }
        public AppUserDto             ContractSalePerson  { get; set; }

        public ContractTransactionWithNavigationPropertiesDto()
        {
            ContractTransaction = new ContractTransactionDto();
            SalePerson          = new AppUserDto();
            Contract            = new ContractDto();
            ContractCampaign    = new CampaignDto();
            ContractPartner     = new PartnerDto();
            ContractSalePerson  = new AppUserDto();
        }
    }
}