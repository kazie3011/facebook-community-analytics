using FacebookCommunityAnalytics.Api.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Medias;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.UserInfos
{
    public interface IUserInfosAppService : IApplicationService
    {
        Task<PagedResultDto<UserInfoWithNavigationPropertiesDto>> GetListAsync(GetUserInfosInput input);
        Task<PagedResultDto<UserInfoWithNavigationPropertiesDto>> GetListExtendAsync(GetExtendUserInfosInput input);

        Task<UserInfoWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<UserInfoDto> GetAsync(Guid id);
        Task<UserInfoWithNavigationPropertiesDto> GetByUserIdAsync(Guid userId);
        Task<UserInfoWithNavigationPropertiesDto> GetByCodeAsync(string userCode);
        Task<List<AppUserDto>> GetUsers();

        Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input);
        Task<PagedResultDto<LookupDto<Guid?>>> GetUserLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<UserInfoDto> CreateAsync(UserInfoCreateDto input);

        Task<UserInfoDto> UpdateAsync(Guid id, UserInfoUpdateDto input);

        Task AddSeedingAccount(Guid id, UserInfoAccount account);
        Task<bool> SeedingAccountExisted(string fid);
        Task SyncUserInfos();
        Task<MediaDto> UpdateAvatarAsync(UserInfoUpdateAvatarDto input);
    }
}