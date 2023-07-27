using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class CalculateWaveMultiplierJob : BackgroundJobBase
    {
        private readonly IPayrollDomainService _payrollDomainService;

        public CalculateWaveMultiplierJob(IPayrollDomainService payrollDomainService)
        {
            _payrollDomainService = payrollDomainService;
        }
        protected override async Task DoExecute()
        {
            await _payrollDomainService.RecalculateWaveMultipliers();
        }
    }
}
