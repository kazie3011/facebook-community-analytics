using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Configs;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.ApiConfigurations
{
    public interface IApiConfigurationAppService : IApplicationService
    {
        Task<PayrollConfiguration> GetPayrollConfiguration();
        Task UpdatePayrollConfiguration(PayrollConfiguration payrollConfiguration);
        Task CreateAsync(ApiConfigurationCreateDto input);
        Task<ApiConfigurationDto> GetAsync();
        //Task<DateTimeRangeResponse> GetDefaultPayrollDateTime();
    }
}
