using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.ApiConfigurations
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.ApiConfigurations.Default)]
    public class ApiConfigurationAppService : ApplicationService, IApiConfigurationAppService
    {
        private readonly IApiConfigurationDomainService _apiConfigurationDomainService;

        public ApiConfigurationAppService(IApiConfigurationDomainService apiConfigurationDomainService)
        {
            _apiConfigurationDomainService = apiConfigurationDomainService;
        }

        public Task CreateAsync(ApiConfigurationCreateDto input)
        {
            var entity = ObjectMapper.Map<ApiConfigurationCreateDto, ApiConfiguration>(input);
            entity.TenantId = CurrentTenant.Id;
            return _apiConfigurationDomainService.CreateAsync(entity);
        }

        public async Task<ApiConfigurationDto> GetAsync()
        {
            return ObjectMapper.Map<ApiConfiguration, ApiConfigurationDto>(await _apiConfigurationDomainService.GetAsync());
        }

        public async Task<PayrollConfiguration> GetPayrollConfiguration()
        {
            return _apiConfigurationDomainService.GetPayrollConfiguration();
        }
        
        public Task UpdatePayrollConfiguration(PayrollConfiguration payrollConfiguration)
        {
            return _apiConfigurationDomainService.UpdatePayrollConfiguration(payrollConfiguration);
        }
        
        // public async Task<DateTimeRangeResponse> GetDefaultPayrollDateTime()
        // {
        //     var value = _apiConfigurationDomainService.GetDefaultPayrollDateTime();
        //     return new DateTimeRangeResponse
        //     {
        //         FromDateTime = value.Key,
        //         ToDateTime = value.Value
        //     };
        // }
    }
}
