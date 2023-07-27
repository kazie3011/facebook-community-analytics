using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.ScheduledPostGroups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.ScheduledPosts
{
    public interface IScheduledPostAppService : IApplicationService
    {
        Task<PagedResultDto<ScheduledPostDto>> GetListAsync(GetScheduledPostInput input);

        Task<ScheduledPostDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<ScheduledPostDto> CreateAsync(ScheduledPostCreateDto input);

        Task<ScheduledPostDto> UpdateAsync(Guid id, ScheduledPostUpdateDto input);

        Task<List<ScheduledPostDto>> GetUnPostScheduledPosts();
        Task<PagedResultDto<SchedulePostWithNavigationPropertiesDto>> GetListWithNavigationPropertiesAsync(GetScheduledPostInput input);
        Task UpdatePostedGroup(PostedGroupDto input);
    }
}
