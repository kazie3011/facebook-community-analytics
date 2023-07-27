using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services.Emails;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class SendPostsEmailJob : BackgroundJobBase
    {
        private readonly IPostEmailDomainService _postEmailDomainService;

        public SendPostsEmailJob(IPostEmailDomainService postEmailDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _postEmailDomainService = postEmailDomainService;
        }

        protected override async Task DoExecute()
        {
            await _postEmailDomainService.SendUncrawlsPosts();
        }
    }
}