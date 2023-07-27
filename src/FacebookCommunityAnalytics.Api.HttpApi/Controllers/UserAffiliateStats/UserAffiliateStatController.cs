using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AppUserAffiliateStats;
using FacebookCommunityAnalytics.Api.UserAffiliateStats;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.UserAffiliateStats
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserAffiliateStat")]
    [Route("api/app/user-affiliate-stat")]
    public class UserAffiliateStatController : AbpController, IUserAffiliateAppService
    {
        private readonly IUserAffiliateAppService _userAffiliateAppService;

        public UserAffiliateStatController(IUserAffiliateAppService userAffiliateAppService)
        {
            _userAffiliateAppService = userAffiliateAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<UserAffiliateStatDto> GetAsync(Guid id)
        {
            return _userAffiliateAppService.GetAsync(id);
        }

        [HttpGet]
        public Task<PagedResultDto<UserAffiliateStatDto>> GetListAsync(GetUserAffiliateStatsInput input)
        {
            return _userAffiliateAppService.GetListAsync(input);
        }

        [HttpPost]
        public Task<UserAffiliateStatDto> CreateAsync(UserAffiliateStatCreateAndUpdateDto input)
        {
            return _userAffiliateAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public Task<UserAffiliateStatDto> UpdateAsync(Guid id, UserAffiliateStatCreateAndUpdateDto input)
        {
            return _userAffiliateAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public Task DeleteAsync(Guid id)
        {
            return _userAffiliateAppService.DeleteAsync(id);
        }
    }
}