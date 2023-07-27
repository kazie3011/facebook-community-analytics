using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Notifications.Emails;
using FacebookCommunityAnalytics.Api.Services.Emails;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class SendSampleEmailJob : BackgroundJobBase
    {
        private readonly ISampleSendEmailService _sampleSendEmailService;

        public SendSampleEmailJob(ISampleSendEmailService sampleSendEmailService)
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