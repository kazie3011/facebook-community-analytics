using System;
using FacebookCommunityAnalytics.Api.Groups;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.GroupStatsHistories
{
    [Authorize]
    [RemoteService(IsEnabled = false)]
    public class GroupStatsHistoryAppService : CrudAppService<GroupStatsHistory,GroupStatsHistoryDto,Guid,GetGroupStatsHistoryInput, GroupStatsHistoryCreateUpdateDto>,IGroupStatsHistoryAppService
    {
        public GroupStatsHistoryAppService(IRepository<GroupStatsHistory, Guid> repository) : base(repository)
        {
        }
    }
}