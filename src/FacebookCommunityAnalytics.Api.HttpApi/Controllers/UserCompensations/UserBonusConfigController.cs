using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.UserCompensations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Controllers.UserCompensations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserBonusConfig")]
    [Route("api/app/user-bonus-configs")]
    public class UserBonusConfigController : ApiController, IUserBonusConfigAppService
    {
        private readonly IUserBonusConfigAppService _userBonusConfigAppService;

        public UserBonusConfigController(IUserBonusConfigAppService userBonusConfigAppService)
        {
            _userBonusConfigAppService = userBonusConfigAppService;
        }
        
        [HttpGet]
        [Route("{id}")]
        public async Task<UserBonusConfigDto> GetAsync(Guid id)
        {
            return await _userBonusConfigAppService.GetAsync(id);
        }
        [HttpGet]
        public async Task<PagedResultDto<UserBonusConfigDto>> GetListAsync(GetUserBonusConfigsInput input)
        {
            return await _userBonusConfigAppService.GetListAsync(input);
        }
        [HttpPost]
        public async Task<UserBonusConfigDto> CreateAsync(CreateUpdateUserBonusConfigDto input)
        {
            return await _userBonusConfigAppService.CreateAsync(input);
        }
        
        [HttpPut]
        [Route("{id}")]
        public async Task<UserBonusConfigDto> UpdateAsync(Guid id, CreateUpdateUserBonusConfigDto input)
        {
            return await _userBonusConfigAppService.UpdateAsync(id, input);
        }
        
        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _userBonusConfigAppService.DeleteAsync(id);
        }
    }
}