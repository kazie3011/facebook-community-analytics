using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class CalculateWaveMultiplierJob : BackgroundJobBase
    {
        private readonly IPayrollDomainService _payrollDomainService;

        public CalculateWaveMultiplierJob(IPayrollDomainService payrollDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _payrollDomainService = payrollDomainService;
        }
        protected override async Task DoExecute()
        {
            await _payrollDomainService.RecalculateWaveMultipliers();
        }
    }
}
