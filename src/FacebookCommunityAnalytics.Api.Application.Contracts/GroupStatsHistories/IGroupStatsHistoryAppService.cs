using System;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.GroupStatsHistories
{
    public interface IGroupStatsHistoryAppService : ICrudAppService<GroupStatsHistoryDto,Guid,GetGroupStatsHistoryInput, GroupStatsHistoryCreateUpdateDto>
    {
        
    }
}