using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Posts;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class InitPostHistories : BackgroundJobBase
    {
        private readonly IPostDomainService _postDomainService;

        public InitPostHistories(IPostDomainService postDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _postDomainService = postDomainService;
        }

        protected override async Task DoExecute()
        {
            await _postDomainService.InitPostHistories();
        }
    }
}