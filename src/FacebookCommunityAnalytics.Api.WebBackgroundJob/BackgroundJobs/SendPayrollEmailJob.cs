using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services.Emails;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class SendPayrollEmailJob : BackgroundJobBase
    {
        private readonly IPayrollEmailDomainService _payrollEmailDomainService;

        public SendPayrollEmailJob(IPayrollEmailDomainService payrollEmailDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _payrollEmailDomainService = payrollEmailDomainService;
        }

        protected override async Task DoExecute()
        {
            await _payrollEmailDomainService.Send(true, null);
            await _payrollEmailDomainService.Send(false, null);
        }
    }
}