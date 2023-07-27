using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.ApiConfigurations;
using FacebookCommunityAnalytics.Api.Configs;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.ApiConfigurations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ApiConfiguration")]
    [Route("api/app/api-configurations")]
    public class ApiConfigurationController : AbpController, IApiConfigurationAppService
    {
        private readonly IApiConfigurationAppService _apiConfigurationAppService;

        public ApiConfigurationController(IApiConfigurationAppService apiConfigurationAppService)
        {
            _apiConfigurationAppService = apiConfigurationAppService;
        }

        [HttpPost]
        public Task CreateAsync(ApiConfigurationCreateDto input)
        {
            return _apiConfigurationAppService.CreateAsync(input);
        }

        [HttpGet]
        public Task<ApiConfigurationDto> GetAsync()
        {
            return _apiConfigurationAppService.GetAsync();
        }

        // [HttpGet("get-payroll-default-from-to-date-time")]
        // public Task<DateTimeRangeResponse> GetDefaultPayrollDateTime()
        // {
        //     return _apiConfigurationAppService.GetDefaultPayrollDateTime();
        // }

        [HttpGet("get-payroll-configuration")]
        public Task<PayrollConfiguration> GetPayrollConfiguration()
        {
            return _apiConfigurationAppService.GetPayrollConfiguration();
        }

        [HttpPut("update-payroll-configuration")]
        public Task UpdatePayrollConfiguration(PayrollConfiguration payrollConfiguration)
        {
            return _apiConfigurationAppService.UpdatePayrollConfiguration(payrollConfiguration);
        }
    }
}
