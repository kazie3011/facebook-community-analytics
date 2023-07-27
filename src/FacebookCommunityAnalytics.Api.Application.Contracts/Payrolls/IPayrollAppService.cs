using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.UserPayrolls;
using Volo.Abp.Application.Dtos; 
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Payrolls
{
    public interface IPayrollsAppService : IApplicationService
    {
        Task<PagedResultDto<PayrollDto>> GetListAsync(GetPayrollsInput input);

        Task<PayrollDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<PayrollDto> CreateAsync(PayrollCreateDto input);

        Task<PayrollDto> UpdateAsync(Guid id, PayrollUpdateDto input);

        Task<PayrollConfiguration> GetPayrollConfiguration();
        Task<GlobalConfiguration> GetGlobalConfiguration();
        Task<PayrollDetailResponse> GetPayrollDetail(PayrollDetailRequest payrollDetailRequest);
        Task CalculatePayroll(bool isHappyDay = false);
        Task CalculateCompensation(int month, int year, bool isHappyDay = false);
        Task ConfirmPayroll(Guid payrollId);
        Task SendEmail(Guid payrollId);
    }
}