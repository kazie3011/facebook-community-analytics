using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Configs;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.UserPayrolls;

namespace FacebookCommunityAnalytics.Api.Controllers.Payrolls
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Payroll")]
    [Route("api/app/payrolls")]
    public class PayrollController : AbpController, IPayrollsAppService
    {
        private readonly IPayrollsAppService _payrollsAppService;

        public PayrollController(IPayrollsAppService payrollsAppService)
        {
            _payrollsAppService = payrollsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<PayrollDto>> GetListAsync(GetPayrollsInput input)
        {
            return _payrollsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<PayrollDto> GetAsync(Guid id)
        {
            return _payrollsAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<PayrollDto> CreateAsync(PayrollCreateDto input)
        {
            return _payrollsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<PayrollDto> UpdateAsync(Guid id, PayrollUpdateDto input)
        {
            return _payrollsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _payrollsAppService.DeleteAsync(id);
        }

        [HttpGet("global-configuration")]
        public Task<GlobalConfiguration> GetGlobalConfiguration()
        {
            return _payrollsAppService.GetGlobalConfiguration();
        }

        [HttpGet("payroll-detail")]
        public Task<PayrollDetailResponse> GetPayrollDetail(PayrollDetailRequest payrollDetailRequest)
        {
            return _payrollsAppService.GetPayrollDetail(payrollDetailRequest);
        }

        [HttpGet]
        [Route("calculate")]
        public Task CalculatePayroll([FromQuery] bool isHappyDay = false)
        {
            return _payrollsAppService.CalculatePayroll(isHappyDay);
        }

        [HttpGet]
        [Route("calculate-compensation")]
        public Task CalculateCompensation(int month, int year,bool isHappyDay = false)
        {
            return _payrollsAppService.CalculateCompensation(month, year, isHappyDay);
        }

        [HttpGet("payroll-configuration")]
        public Task<PayrollConfiguration> GetPayrollConfiguration()
        {
            return _payrollsAppService.GetPayrollConfiguration();
        }

        [HttpGet("confirm-payroll/{payrollId}")]
        public Task ConfirmPayroll(Guid payrollId)
        {
            return _payrollsAppService.ConfirmPayroll(payrollId);
        }
        
        [HttpPost("send-email/{payrollId}")]
        public Task SendEmail(Guid payrollId)
        {
            return _payrollsAppService.SendEmail(payrollId);
        }
    }
}