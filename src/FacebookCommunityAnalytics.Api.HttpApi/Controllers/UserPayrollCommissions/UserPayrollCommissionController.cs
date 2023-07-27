using FacebookCommunityAnalytics.Api.Shared;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.UserPayrollCommissions;

namespace FacebookCommunityAnalytics.Api.Controllers.UserPayrollCommissions
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserPayrollCommission")]
    [Route("api/app/user-payroll-commissions")]

    public class UserPayrollCommissionController : AbpController, IUserPayrollCommissionsAppService
    {
        private readonly IUserPayrollCommissionsAppService _userPayrollCommissionsAppService;

        public UserPayrollCommissionController(IUserPayrollCommissionsAppService userPayrollCommissionsAppService)
        {
            _userPayrollCommissionsAppService = userPayrollCommissionsAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<UserPayrollCommissionWithNavigationPropertiesDto>> GetListAsync(GetUserPayrollCommissionsInput input)
        {
            return _userPayrollCommissionsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<UserPayrollCommissionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _userPayrollCommissionsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<UserPayrollCommissionDto> GetAsync(Guid id)
        {
            return _userPayrollCommissionsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("app-user-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input)
        {
            return _userPayrollCommissionsAppService.GetAppUserLookupAsync(input);
        }

        [HttpGet]
        [Route("payroll-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetPayrollLookupAsync(LookupRequestDto input)
        {
            return _userPayrollCommissionsAppService.GetPayrollLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<UserPayrollCommissionDto> CreateAsync(UserPayrollCommissionCreateDto input)
        {
            return _userPayrollCommissionsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<UserPayrollCommissionDto> UpdateAsync(Guid id, UserPayrollCommissionUpdateDto input)
        {
            return _userPayrollCommissionsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _userPayrollCommissionsAppService.DeleteAsync(id);
        }
    }
}