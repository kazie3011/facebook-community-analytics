using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.ScheduledPostGroups;
using FacebookCommunityAnalytics.Api.ScheduledPosts;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.ScheduledPosts
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ScheduledPost")]
    [Route("api/app/schedule-posts")]
    public class ScheduledPostController : AbpController, IScheduledPostAppService
    {
        private readonly IScheduledPostAppService _scheduledPostsAppService;

        public ScheduledPostController(IScheduledPostAppService scheduledPostsAppService)
        {
            _scheduledPostsAppService = scheduledPostsAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<ScheduledPostDto>> GetListAsync(GetScheduledPostInput input)
        {
            return _scheduledPostsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public Task<ScheduledPostDto> GetAsync(Guid id)
        {
            return _scheduledPostsAppService.GetAsync(id);
        }

        [HttpDelete]
        [Route("{id}")]
        public Task DeleteAsync(Guid id)
        {
            return _scheduledPostsAppService.DeleteAsync(id);
        }

        [HttpPost]
        public Task<ScheduledPostDto> CreateAsync(ScheduledPostCreateDto input)
        {
            return _scheduledPostsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public Task<ScheduledPostDto> UpdateAsync(Guid id, ScheduledPostUpdateDto input)
        {
            return _scheduledPostsAppService.UpdateAsync(id, input);
        }

        [HttpGet]
        [Route("gets-unpost")]
        public Task<List<ScheduledPostDto>> GetUnPostScheduledPosts()
        {
            return _scheduledPostsAppService.GetUnPostScheduledPosts();
        }

        [HttpPut("posted")]
        public Task UpdatePostedGroup(PostedGroupDto input)
        {
            return _scheduledPostsAppService.UpdatePostedGroup(input);
        }

        [HttpGet("gets-scheduled-post-navigation-properties")]
        public Task<PagedResultDto<SchedulePostWithNavigationPropertiesDto>> GetListWithNavigationPropertiesAsync(GetScheduledPostInput input)
        {
            return _scheduledPostsAppService.GetListWithNavigationPropertiesAsync(input);
        }
    }
}
