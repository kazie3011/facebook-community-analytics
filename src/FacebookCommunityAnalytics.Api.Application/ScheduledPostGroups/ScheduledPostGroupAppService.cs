using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.ScheduledPostGroups
{
    [RemoteService(IsEnabled = true)]
    [Authorize]
    public class ScheduledPostGroupAppService :
        CrudAppService<ScheduledPostGroup, ScheduledPostGroupDto, Guid, GetScheduledPostGroupInput, ScheduledPostGroupCreateUpdateDto>,
        IScheduledPostGroupAppService
    {
        public ScheduledPostGroupAppService(IRepository<ScheduledPostGroup, Guid> repository) : base(repository)
        {
        }

        public async Task<IEnumerable<ScheduledPostGroupDto>> GetPendingScheduledPost()
        {
            var scheduledPostGroups = await Repository.GetListAsync(x => x.IsPosted == false);
            return ObjectMapper.Map<List<ScheduledPostGroup>, List<ScheduledPostGroupDto>>(scheduledPostGroups);
        }
    }
}