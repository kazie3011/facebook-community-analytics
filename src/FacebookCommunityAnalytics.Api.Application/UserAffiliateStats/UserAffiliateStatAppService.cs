using System;
using FacebookCommunityAnalytics.Api.AppUserAffiliateStats;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.UserAffiliateStats
{
    [Authorize]
    [RemoteService(IsEnabled = false)]
    public class UserAffiliateStatAppService : CrudAppService<UserAffiliateStat, UserAffiliateStatDto, Guid, GetUserAffiliateStatsInput, UserAffiliateStatCreateAndUpdateDto>, IUserAffiliateAppService
    {
        public UserAffiliateStatAppService(IRepository<UserAffiliateStat, Guid> repository) : base(repository)
        {
            
        }
    }
}