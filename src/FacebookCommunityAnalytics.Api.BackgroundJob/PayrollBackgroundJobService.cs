using FacebookCommunityAnalytics.Api.Services;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace FacebookCommunityAnalytics.Api.BackgroundJob
{
    public class PayrollBackgroundJobService : ITransientDependency
    {
        private readonly IPayrollDomainService _payrollDomainService;

        public PayrollBackgroundJobService(IPayrollDomainService payrollDomainService)
        {
            _payrollDomainService = payrollDomainService;
        }

        public async Task Execute()
        {
            Console.WriteLine("Calculate payroll!");
           await  _payrollDomainService.CalculateUserPayrolls(true, true);
        }
    }
}
