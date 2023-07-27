using System;
using FacebookCommunityAnalytics.Api.AppUserAffiliateStats;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.UserAffiliateStats
{
    public interface IUserAffiliateAppService : ICrudAppService<UserAffiliateStatDto, Guid, GetUserAffiliateStatsInput, UserAffiliateStatCreateAndUpdateDto>
    {
        
    }
}