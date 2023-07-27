using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services.Emails;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class SendStartedCampaignEmailJob : BackgroundJobBase
    {
        private readonly ICampaignEmailDomainService _campaignEmailDomainService;

        public SendStartedCampaignEmailJob(ICampaignEmailDomainService campaignEmailDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _campaignEmailDomainService = campaignEmailDomainService;
        }

        protected override async Task DoExecute()
        {
            await _campaignEmailDomainService.SendAll();
        }
    }
}
