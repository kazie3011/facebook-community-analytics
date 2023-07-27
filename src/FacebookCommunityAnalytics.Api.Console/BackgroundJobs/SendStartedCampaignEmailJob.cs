using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services.Emails;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class SendStartedCampaignEmailJob : BackgroundJobBase
    {
        private readonly ICampaignEmailDomainService _campaignEmailDomainService;

        public SendStartedCampaignEmailJob(ICampaignEmailDomainService campaignEmailDomainService)
        {
            _campaignEmailDomainService = campaignEmailDomainService;
        }

        protected override async Task DoExecute()
        {
            await _campaignEmailDomainService.SendAll();
        }
    }
}
