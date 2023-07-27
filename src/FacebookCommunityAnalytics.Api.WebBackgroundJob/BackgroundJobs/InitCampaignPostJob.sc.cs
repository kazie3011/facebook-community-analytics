using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class InitCampaignPostsJob : BackgroundJobBase
    {
        private readonly ICrawlDomainService _crawlDomainService;

        public InitCampaignPostsJob(ICrawlDomainService crawlDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _crawlDomainService = crawlDomainService;
        }

        protected override async Task DoExecute()
        {
            await _crawlDomainService.InitCampaignPosts();
        }
    }
}