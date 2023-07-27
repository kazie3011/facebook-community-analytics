using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Proxies;
using FacebookCommunityAnalytics.Api.ScheduledPostGroups;
using FacebookCommunityAnalytics.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.ScheduledPosts
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.ScheduledPosts.Default)]
    public class ScheduledPostAppService : ApplicationService, IScheduledPostAppService
    {
        private readonly IScheduledPostRepository _scheduledPostRepository;
        private readonly IScheduledPostDomainService _scheduledPostDomainService;
        private readonly IRepository<ScheduledPostGroup> _scheduledPostGroupRepository;

        public ScheduledPostAppService(
            IScheduledPostRepository scheduledPostRepository,
            IScheduledPostDomainService scheduledPostDomainService,
            IRepository<ScheduledPostGroup> scheduledPostGroupRepository)
        {
            _scheduledPostRepository = scheduledPostRepository;
            _scheduledPostDomainService = scheduledPostDomainService;
            _scheduledPostGroupRepository = scheduledPostGroupRepository;
        }

        public virtual async Task<PagedResultDto<ScheduledPostDto>> GetListAsync(GetScheduledPostInput input)
        {
            var totalCount = await _scheduledPostRepository.GetCountAsync
            (
                input.FilterText,
                input.Content,
                input.IsAutoPost,
                input.ScheduledPostDateTimeMin,
                input.ScheduledPostDateTimeMax,
                input.PostedAtMin,
                input.PostedAtMax,
                input.GroupId
            );
            //Todoo: ScheduledPost Update Filter
            var items = await _scheduledPostRepository.GetListAsync
            (
                input.FilterText,
                input.Content,
                input.IsAutoPost,
                input.ScheduledPostDateTimeMin,
                input.ScheduledPostDateTimeMax,
                input.PostedAtMin,
                input.PostedAtMax,
                input.GroupId,
                sorting: input.Sorting,
                maxResultCount: input.MaxResultCount,
                skipCount: input.SkipCount
            );

            return new PagedResultDto<ScheduledPostDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ScheduledPost>, List<ScheduledPostDto>>(items)
            };
        }

        public virtual async Task<ScheduledPostDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<ScheduledPost, ScheduledPostDto>(await _scheduledPostRepository.GetAsync(id));
        }

        [Authorize(ApiPermissions.ScheduledPosts.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _scheduledPostRepository.DeleteAsync(id);
            var scheduledPostGroups = await _scheduledPostGroupRepository.GetListAsync(x => x.ScheduledPostId == id);
            if (scheduledPostGroups.Any())
            {
                await _scheduledPostGroupRepository.DeleteManyAsync(scheduledPostGroups);
            }
        }


        [Authorize(ApiPermissions.ScheduledPosts.Create)]
        public virtual async Task<ScheduledPostDto> CreateAsync(ScheduledPostCreateDto input)
        {
            var scheduledPost = ObjectMapper.Map<ScheduledPostCreateDto, ScheduledPost>(input);
            scheduledPost.TenantId = CurrentTenant.Id;
            scheduledPost = await _scheduledPostRepository.InsertAsync(scheduledPost, autoSave: true);

            var scheduledPostGroups = scheduledPost.GroupIds.Split(',')
                .Select
                (
                    groupId => new ScheduledPostGroup(scheduledPost.Id, groupId, false)
                );

            await _scheduledPostGroupRepository.InsertManyAsync(scheduledPostGroups);

            return ObjectMapper.Map<ScheduledPost, ScheduledPostDto>(scheduledPost);
        }

        [Authorize(ApiPermissions.ScheduledPosts.Edit)]
        public virtual async Task<ScheduledPostDto> UpdateAsync(Guid id, ScheduledPostUpdateDto input)
        {
            var scheduledPost = await _scheduledPostRepository.GetAsync(id);
            ObjectMapper.Map(input, scheduledPost);
            scheduledPost = await _scheduledPostRepository.UpdateAsync(scheduledPost);

            //Get all current scheduled post groups
            var currentScheduledPostGroups = await _scheduledPostGroupRepository.GetListAsync(x => x.ScheduledPostId == scheduledPost.Id);

            //Delete removed scheduled post groups
            var groupIds = scheduledPost.GroupIds.Split(',');
            var removedScheduledPostGroups = currentScheduledPostGroups.Where(x => !groupIds.Contains(x.Fid));
            if (removedScheduledPostGroups.Any()) { await _scheduledPostGroupRepository.DeleteManyAsync((removedScheduledPostGroups)); }

            //Insert new scheduled post groups
            var newGroupIds = groupIds.Where(groupId => !currentScheduledPostGroups.Select(y => y.Fid).Contains(groupId));
            if (newGroupIds.Any())
            {
                var newScheduledPostGroups = newGroupIds.Select(groupId => new ScheduledPostGroup(scheduledPost.Id, groupId, false));
                await _scheduledPostGroupRepository.InsertManyAsync(newScheduledPostGroups);
            }

            return ObjectMapper.Map<ScheduledPost, ScheduledPostDto>(scheduledPost);
        }

        [AllowAnonymous]
        public async Task<List<ScheduledPostDto>> GetUnPostScheduledPosts()
        {
            var unPosts = await _scheduledPostRepository.GetListAsync();
            var scheduledPostGroups = await _scheduledPostGroupRepository.GetListAsync(x => x.IsPosted == false);

            foreach (var scheduledPost in unPosts) { scheduledPost.GroupIds = scheduledPostGroups.Where(x => x.ScheduledPostId == scheduledPost.Id).Select(x => x.Fid).JoinAsString(","); }

            var results = unPosts.Where(x => !x.GroupIds.IsNullOrWhiteSpace()).ToList();
            
            return ObjectMapper.Map<List<ScheduledPost>, List<ScheduledPostDto>>(results);
        }

        public async Task<PagedResultDto<SchedulePostWithNavigationPropertiesDto>> GetListWithNavigationPropertiesAsync(GetScheduledPostInput input)
        {
            var totalCount = await _scheduledPostRepository.GetCountAsync
            (
                input.FilterText,
                input.Content,
                input.IsAutoPost,
                input.ScheduledPostDateTimeMin,
                input.ScheduledPostDateTimeMax,
                input.PostedAtMin,
                input.PostedAtMax,
                input.GroupId
            );
            var items = await _scheduledPostDomainService.GetListWithNavigationPropertiesAsync(input);
            return new PagedResultDto<SchedulePostWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<SchedulePostWithNavigationProperties>, List<SchedulePostWithNavigationPropertiesDto>>(items)
            };
        }

        public async Task UpdatePostedGroup(PostedGroupDto input)
        {
            var scheduledPostGroup = await _scheduledPostGroupRepository.FirstOrDefaultAsync(x => x.ScheduledPostId == input.ScheduledPostId && x.Fid == input.Fid);
            if (scheduledPostGroup != null)
            {
                scheduledPostGroup.IsPosted = true;
                await _scheduledPostGroupRepository.UpdateAsync(scheduledPostGroup);
            }
        }
    }
}