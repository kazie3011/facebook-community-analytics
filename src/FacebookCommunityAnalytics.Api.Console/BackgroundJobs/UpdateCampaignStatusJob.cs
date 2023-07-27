using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class UpdateCampaignStatusJob : BackgroundJobBase
    {
        private readonly ICampaignDomainService _campaignDomainService;

        public UpdateCampaignStatusJob(ICampaignDomainService campaignDomainService)
        {
            _campaignDomainService = campaignDomainService;
        }

        protected override async Task DoExecute()
        {
            await _campaignDomainService.UpdateStatuses();
        }
    }
}