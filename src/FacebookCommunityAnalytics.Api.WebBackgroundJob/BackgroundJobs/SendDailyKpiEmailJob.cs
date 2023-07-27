using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services.Emails;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class SendDailyKpiEmailJob : BackgroundJobBase
    {
        private readonly IDailyKPITrackingEmailDomainService _trackingEmailDomainService;

        public SendDailyKpiEmailJob(IDailyKPITrackingEmailDomainService trackingEmailDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _trackingEmailDomainService = trackingEmailDomainService;
        }

        protected override async Task DoExecute()
        {
            await _trackingEmailDomainService.DoSendEMails();
        }
    }
}