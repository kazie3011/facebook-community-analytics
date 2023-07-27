using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class SampleJob: BackgroundJobBase
    {
        protected override Task DoExecute()
        {
            Debug.Write($"{this.GetType().Name} is running at {DateTime.UtcNow} UTC \n");
            //throw new NotImplementedException("test exception log");

            return Task.CompletedTask;
        }

        public SampleJob(ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
        }
    }
}
