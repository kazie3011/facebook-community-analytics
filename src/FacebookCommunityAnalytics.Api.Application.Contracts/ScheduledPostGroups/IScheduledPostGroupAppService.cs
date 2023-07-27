using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.ScheduledPostGroups
{
    public interface IScheduledPostGroupAppService:ICrudAppService<ScheduledPostGroupDto,Guid, GetScheduledPostGroupInput,ScheduledPostGroupCreateUpdateDto>
    {
        Task<IEnumerable<ScheduledPostGroupDto>> GetPendingScheduledPost();
    }
}