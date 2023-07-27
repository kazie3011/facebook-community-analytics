using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services.Emails;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class SendPayrollEmailJob : BackgroundJobBase
    {
        private readonly IPayrollEmailDomainService _payrollEmailDomainService;

        public SendPayrollEmailJob(IPayrollEmailDomainService payrollEmailDomainService)
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