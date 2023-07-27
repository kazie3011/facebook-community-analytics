using FacebookCommunityAnalytics.Api.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Medias;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;

namespace FacebookCommunityAnalytics.Api.Controllers.UserInfos
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserInfo")]
    [Route("api/app/user-infos")]
    public class UserInfoController : AbpController, IUserInfosAppService
    {
        private readonly IUserInfosAppService _userInfosAppService;

        public UserInfoController(IUserInfosAppService userInfosAppService)
        {
            _userInfosAppService = userInfosAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<UserInfoWithNavigationPropertiesDto>> GetListAsync(GetUserInfosInput input)
        {
            return _userInfosAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<UserInfoWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _userInfosAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<UserInfoDto> GetAsync(Guid id)
        {
            return _userInfosAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("/get-by-code/{userCode}")]
        public Task<UserInfoWithNavigationPropertiesDto> GetByCodeAsync(string userCode)
        {
            return _userInfosAppService.GetByCodeAsync(userCode);
        }

        [HttpGet]
        [Route("/users")]
        public Task<List<AppUserDto>> GetUsers()
        {
            return _userInfosAppService.GetUsers();
        }

        [HttpGet]
        [Route("/get-by-userid/{userId}")]
        public Task<UserInfoWithNavigationPropertiesDto> GetByUserIdAsync(Guid userId)
        {
            return _userInfosAppService.GetByUserIdAsync(userId);
        }

        [HttpGet]
        [Route("app-user-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input)
        {
            return _userInfosAppService.GetAppUserLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<UserInfoDto> CreateAsync(UserInfoCreateDto input)
        {
            return _userInfosAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<UserInfoDto> UpdateAsync(Guid id, UserInfoUpdateDto input)
        {
            return _userInfosAppService.UpdateAsync(id, input);
        }

        [HttpPost]
        [Route("seeding-account/{id}")]
        public Task AddSeedingAccount(Guid id, UserInfoAccount account)
        {
            return _userInfosAppService.AddSeedingAccount(id, account);
        }

        [HttpGet("seeding-accounts-existed/{fid}")]
        public Task<bool> SeedingAccountExisted(string fid)
        {
            return _userInfosAppService.SeedingAccountExisted(fid);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _userInfosAppService.DeleteAsync(id);
        }

        [HttpPost]
        [Route("sync-user-infos")]
        public Task SyncUserInfos()
        {
            return _userInfosAppService.SyncUserInfos();
        }

        [HttpPost]
        [Route("update-avatar")]
        public Task<MediaDto> UpdateAvatarAsync(UserInfoUpdateAvatarDto input)
        {
            return _userInfosAppService.UpdateAvatarAsync(input);
        }

        [HttpGet]
        [Route("get-list-extend")]
        public Task<PagedResultDto<UserInfoWithNavigationPropertiesDto>> GetListExtendAsync(GetExtendUserInfosInput input)
        {
            return _userInfosAppService.GetListExtendAsync(input);
        }

        [HttpGet]
        [Route("user-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetUserLookupAsync(LookupRequestDto input)
        {
            return _userInfosAppService.GetUserLookupAsync(input);
        }
    }
}