using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class SyncCampaignPostCountJob : BackgroundJobBase
    {
        private readonly ICampaignDomainService _campaignDomainService;
        private readonly ICampaignRepository _campaignRepository;

        public SyncCampaignPostCountJob(ICampaignDomainService campaignDomainService, ICampaignRepository campaignRepository, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _campaignDomainService = campaignDomainService;
            _campaignRepository = campaignRepository;
        }

        protected override async Task DoExecute()
        {
            var campaigns = await _campaignRepository.GetListAsync();
            foreach (var campaign in campaigns.Where(_ => _.Status == CampaignStatus.Started || _.Status == CampaignStatus.Hold))
            {
                await _campaignDomainService.UpdateCampaignPostCount(campaign.Id);
            }
        }
    }
}