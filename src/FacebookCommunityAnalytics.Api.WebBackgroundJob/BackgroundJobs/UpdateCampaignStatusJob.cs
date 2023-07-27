using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class UpdateCampaignStatusJob : BackgroundJobBase
    {
        private readonly ICampaignDomainService _campaignDomainService;

        public UpdateCampaignStatusJob(ICampaignDomainService campaignDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _campaignDomainService = campaignDomainService;
        }

        protected override async Task DoExecute()
        {
            await _campaignDomainService.UpdateStatuses();
        }
    }
}