using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.ApiNotifications;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.Services.Emails;
using FacebookCommunityAnalytics.Api.UserCompensations;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Users;

namespace FacebookCommunityAnalytics.Api.Payrolls
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.Payrolls.Default)]
    public class PayrollsAppService : ApiAppService, IPayrollsAppService
    {
        private readonly IApiConfigurationDomainService _apiConfigurationDomainService;
        private readonly IPayrollRepository _payrollRepository;
        private readonly IPayrollDomainService _payrollDomainService;
        private readonly IPayrollEmailDomainService _payrollEmailDomainService;
        private readonly IUserCompensationDomainService _userCompensationDomainService;
        private readonly IDistributedEventBus _distributedEventBus;
        public PayrollsAppService(
            IApiConfigurationDomainService apiConfigurationDomainService,
            IPayrollRepository payrollRepository, 
            IPayrollDomainService payrollDomainService,
            IPayrollEmailDomainService payrollEmailDomainService,
            IDistributedEventBus distributedEventBus,
            IUserCompensationDomainService userCompensationDomainService)
        {
            _apiConfigurationDomainService = apiConfigurationDomainService;
            _payrollRepository = payrollRepository;
            _payrollDomainService = payrollDomainService;
            _payrollEmailDomainService = payrollEmailDomainService;
            _distributedEventBus = distributedEventBus;
            _userCompensationDomainService = userCompensationDomainService;
        }

        public virtual async Task<PagedResultDto<PayrollDto>> GetListAsync(GetPayrollsInput input)
        {
            var totalCount = await _payrollRepository.GetCountAsync(input.FilterText, input.Code, input.Title, input.Description, input.FromDateTimeMin, input.FromDateTimeMax, input.ToDateTimeMin, input.ToDateTimeMax, input.IsCompensation);
            var items = await _payrollRepository.GetListAsync(input.FilterText, input.Code, input.Title, input.Description, input.FromDateTimeMin, input.FromDateTimeMax, input.ToDateTimeMin, input.ToDateTimeMax, input.IsCompensation, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<PayrollDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Payroll>, List<PayrollDto>>(items)
            };
        }

        public virtual async Task<PayrollDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Payroll, PayrollDto>(await _payrollRepository.GetAsync(id));
        }

        [Authorize(ApiPermissions.Payrolls.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _payrollDomainService.DeletePayroll(id);
        }

        [Authorize(ApiPermissions.Payrolls.Create)]
        public virtual async Task<PayrollDto> CreateAsync(PayrollCreateDto input)
        {

            var payroll = ObjectMapper.Map<PayrollCreateDto, Payroll>(input);
            payroll.TenantId = CurrentTenant.Id;
            payroll = await _payrollRepository.InsertAsync(payroll, autoSave: true);
            return ObjectMapper.Map<Payroll, PayrollDto>(payroll);
        }

        [Authorize(ApiPermissions.Payrolls.Edit)]
        public virtual async Task<PayrollDto> UpdateAsync(Guid id, PayrollUpdateDto input)
        {

            var payroll = await _payrollRepository.GetAsync(id);
            ObjectMapper.Map(input, payroll);
            payroll = await _payrollRepository.UpdateAsync(payroll);
            return ObjectMapper.Map<Payroll, PayrollDto>(payroll);
        }
        
        public async Task<PayrollDetailResponse> GetPayrollDetail(PayrollDetailRequest payrollDetailRequest)
        {
            var detail = await _payrollDomainService.GetPayrollDetail(payrollDetailRequest);
            return ObjectMapper.Map<PayrollResponse, PayrollDetailResponse>(detail);
        }

        public async Task CalculatePayroll(bool isHappyDay = false)
        {
            await _distributedEventBus.PublishAsync(new CalculatePayrollEto
            {
                IsHappyDay = isHappyDay,
                 CurrentUserId = CurrentUser.Id
            });
        }

        public async Task CalculateCompensation(int month, int year,bool isHappyDay = false)
        {
            Hangfire.BackgroundJob.Enqueue
            (
                () => DoCalculateCompensation(month, year, isHappyDay)
            );
        }

        public async Task DoCalculateCompensation(int month, int year, bool isHappyDay = false)
        {
            await _userCompensationDomainService.CalculateCompensations(true, true, isHappyDay, CurrentUser.Id, month, year);
            if (CurrentUser.Id.HasValue)
            {
                await _distributedEventBus.PublishAsync(new ReceivedMessageEto(CurrentUser.Id.Value, CurrentUser.UserName, L["Message.DoneCalculateCompensation"]));
            }
        }

        [AllowAnonymous]
        public async Task<PayrollConfiguration> GetPayrollConfiguration()
        {
            return _apiConfigurationDomainService.GetPayrollConfiguration();
        }

        [AllowAnonymous]
        public async Task<GlobalConfiguration> GetGlobalConfiguration()
        {
            return GlobalConfiguration;
        }

        public async Task ConfirmPayroll(Guid payrollId)
        {
            await _payrollDomainService.ConfirmPayroll(payrollId);
        }

        public async Task SendEmail(Guid payrollId)
        {
            await _payrollEmailDomainService.Send(true, payrollId);
        }
    }
}