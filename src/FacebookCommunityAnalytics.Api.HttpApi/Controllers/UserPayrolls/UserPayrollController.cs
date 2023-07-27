using FacebookCommunityAnalytics.Api.Shared;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.UserPayrolls;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.Controllers.UserPayrolls
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserPayroll")]
    [Route("api/app/user-payrolls")]
    public class UserPayrollController : AbpController, IUserPayrollsAppService
    {
        private readonly IUserPayrollsAppService _userPayrollsAppService;

        public UserPayrollController(IUserPayrollsAppService userPayrollsAppService)
        {
            _userPayrollsAppService = userPayrollsAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<UserPayrollWithNavigationPropertiesDto>> GetListAsync(GetUserPayrollsInput input)
        {
            return _userPayrollsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<UserPayrollWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _userPayrollsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<UserPayrollDto> GetAsync(Guid id)
        {
            return _userPayrollsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("payroll-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetPayrollLookupAsync(LookupRequestDto input)
        {
            return _userPayrollsAppService.GetPayrollLookupAsync(input);
        }

        [HttpGet]
        [Route("app-user-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input)
        {
            return _userPayrollsAppService.GetAppUserLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<UserPayrollDto> CreateAsync(UserPayrollCreateDto input)
        {
            return _userPayrollsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<UserPayrollDto> UpdateAsync(Guid id, UserPayrollUpdateDto input)
        {
            return _userPayrollsAppService.UpdateAsync(id, input);
        }

        [HttpPost("payslip")]
        public Task<UserPayrollWithNavigationPropertiesDto> GenerateUserPayroll(UserPayrollRequest userPayrollRequest)
        {
            return _userPayrollsAppService.GenerateUserPayroll(userPayrollRequest);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _userPayrollsAppService.DeleteAsync(id);
        }

    }
}