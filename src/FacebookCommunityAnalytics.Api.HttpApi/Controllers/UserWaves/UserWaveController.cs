using FacebookCommunityAnalytics.Api.Shared;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.UserWaves;

namespace FacebookCommunityAnalytics.Api.Controllers.UserWaves
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserWave")]
    [Route("api/app/user-waves")]

    public class UserWaveController : AbpController, IUserWavesAppService
    {
        private readonly IUserWavesAppService _userWavesAppService;

        public UserWaveController(IUserWavesAppService userWavesAppService)
        {
            _userWavesAppService = userWavesAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<UserWaveWithNavigationPropertiesDto>> GetListAsync(GetUserWavesInput input)
        {
            return _userWavesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<UserWaveWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _userWavesAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<UserWaveDto> GetAsync(Guid id)
        {
            return _userWavesAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("app-user-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input)
        {
            return _userWavesAppService.GetAppUserLookupAsync(input);
        }

        [HttpGet]
        [Route("payroll-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetPayrollLookupAsync(LookupRequestDto input)
        {
            return _userWavesAppService.GetPayrollLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<UserWaveDto> CreateAsync(UserWaveCreateDto input)
        {
            return _userWavesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<UserWaveDto> UpdateAsync(Guid id, UserWaveUpdateDto input)
        {
            return _userWavesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _userWavesAppService.DeleteAsync(id);
        }
    }
}