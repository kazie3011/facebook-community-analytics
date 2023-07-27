using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services.Emails;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class SendTiktokEmailsJob : BackgroundJobBase
    {
        private readonly ITiktokEmailDomainService _tiktokEmailDomainService;

        public SendTiktokEmailsJob(ITiktokEmailDomainService tiktokEmailDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _tiktokEmailDomainService = tiktokEmailDomainService;
        }

        protected override async Task DoExecute()
        {
            await _tiktokEmailDomainService.SendTiktokDaily();
        }
    }
}