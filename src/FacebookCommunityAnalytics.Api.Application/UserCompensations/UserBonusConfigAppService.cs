using System;
using FacebookCommunityAnalytics.Api.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.UserBonusConfigs.Default)]
    public class UserBonusConfigAppService : BaseCrudApiAppService<UserBonusConfig, UserBonusConfigDto, Guid, GetUserBonusConfigsInput, CreateUpdateUserBonusConfigDto>, IUserBonusConfigAppService
    {
        public UserBonusConfigAppService(IRepository<UserBonusConfig, Guid> repository) : base(repository)
        {
        }
    }
}