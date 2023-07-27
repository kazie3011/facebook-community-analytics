using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Notifications.Emails;
using FacebookCommunityAnalytics.Api.Services.Emails;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class SendSampleEmailJob : BackgroundJobBase
    {
        private readonly ISampleSendEmailService _sampleSendEmailService;

        public SendSampleEmailJob(ISampleSendEmailService sampleSendEmailService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _sampleSendEmailService = sampleSendEmailService;
        }

        protected override Task DoExecute()
        {
            _sampleSendEmailService.Send(new SampleEmailModel());

            return Task.CompletedTask;
        }
    }
}