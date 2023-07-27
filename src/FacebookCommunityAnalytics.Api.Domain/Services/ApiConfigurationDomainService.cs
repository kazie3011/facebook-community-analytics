using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.ApiConfigurations;
using FacebookCommunityAnalytics.Api.Configs;
using Newtonsoft.Json;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api.Services
{
    public interface IApiConfigurationDomainService : IDomainService
    {
        Task CreateAsync(ApiConfiguration entity);
        PayrollConfiguration GetPayrollConfiguration();
        Task<ApiConfiguration> GetAsync();
        //KeyValuePair<DateTime, DateTime> GetDefaultPayrollDateTime();
        Task UpdatePayrollConfiguration(PayrollConfiguration payrollConfiguration);
    }

    public class ApiConfigurationDomainService : BaseDomainService, IApiConfigurationDomainService
    {
        private readonly IRepository<ApiConfiguration> _apiConfigurationRepository;
        public ApiConfigurationDomainService(IRepository<ApiConfiguration> apiConfigurationRepository)
        {
            _apiConfigurationRepository = apiConfigurationRepository;
        }
        public async Task CreateAsync(ApiConfiguration entity)
        {
            var apiConfiguration = await _apiConfigurationRepository.FirstOrDefaultAsync();
            if (apiConfiguration != null) return;
            await _apiConfigurationRepository.InsertAsync(entity);
        }
        
        public PayrollConfiguration GetPayrollConfiguration()
        {
            var apiConfig = _apiConfigurationRepository.FirstOrDefault();
            return apiConfig?.PayrollConfiguration;
        }
        
        public Task<ApiConfiguration> GetAsync()
        {
            return _apiConfigurationRepository.FirstOrDefaultAsync();
        }

        // public KeyValuePair<DateTime, DateTime> GetDefaultPayrollDateTime()
        // {
        //     var now = DateTime.UtcNow;
        //     var from = now.AddMonths(-1);
        //     var to = now;
        //
        //     var payrollConfig = GetPayrollConfiguration();
        //     // if (now.Day > payrollConfig.EndDay)
        //     // {
        //     //     from = now;
        //     //     to = now.AddMonths(1);
        //     // }
        //
        //     var fromDateTime = DateTime.SpecifyKind(new DateTime(@from.Year, @from.Month, payrollConfig.StartDay, payrollConfig.StartHour, 0, 0), DateTimeKind.Utc);
        //     var toDateTime = DateTime.SpecifyKind(new DateTime(to.Year, to.Month, payrollConfig.EndDay, payrollConfig.EndHour, 0, 0), DateTimeKind.Utc);
        //
        //     return new KeyValuePair<DateTime, DateTime>(fromDateTime, toDateTime);
        // } 
        
        public async Task UpdatePayrollConfiguration(PayrollConfiguration payrollConfiguration)
        {
            var apiConfiguration = await _apiConfigurationRepository.FirstOrDefaultAsync();
            if (apiConfiguration == null) return;
            apiConfiguration.PayrollConfiguration = payrollConfiguration;

            await _apiConfigurationRepository.UpdateAsync(apiConfiguration);
        }
    }
}
