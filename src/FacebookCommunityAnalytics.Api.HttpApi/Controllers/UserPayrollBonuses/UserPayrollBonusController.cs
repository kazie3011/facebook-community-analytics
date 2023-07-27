using FacebookCommunityAnalytics.Api.Shared;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.UserPayrollBonuses;

namespace FacebookCommunityAnalytics.Api.Controllers.UserPayrollBonuses
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserPayrollBonus")]
    [Route("api/app/user-payroll-bonuses")]

    public class UserPayrollBonusController : AbpController, IUserPayrollBonusesAppService
    {
        private readonly IUserPayrollBonusesAppService _userPayrollBonusesAppService;

        public UserPayrollBonusController(IUserPayrollBonusesAppService userPayrollBonusesAppService)
        {
            _userPayrollBonusesAppService = userPayrollBonusesAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<UserPayrollBonusWithNavigationPropertiesDto>> GetListAsync(GetUserPayrollBonusesInput input)
        {
            return _userPayrollBonusesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<UserPayrollBonusWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _userPayrollBonusesAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<UserPayrollBonusDto> GetAsync(Guid id)
        {
            return _userPayrollBonusesAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("app-user-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input)
        {
            return _userPayrollBonusesAppService.GetAppUserLookupAsync(input);
        }

        [HttpGet]
        [Route("payroll-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetPayrollLookupAsync(LookupRequestDto input)
        {
            return _userPayrollBonusesAppService.GetPayrollLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<UserPayrollBonusDto> CreateAsync(UserPayrollBonusCreateDto input)
        {
            return _userPayrollBonusesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<UserPayrollBonusDto> UpdateAsync(Guid id, UserPayrollBonusUpdateDto input)
        {
            return _userPayrollBonusesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _userPayrollBonusesAppService.DeleteAsync(id);
        }
    }
}