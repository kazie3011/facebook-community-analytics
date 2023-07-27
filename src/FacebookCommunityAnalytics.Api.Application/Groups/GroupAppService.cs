using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Exceptions;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Timing;

namespace FacebookCommunityAnalytics.Api.Groups
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.Groups.Default)]
    public class GroupsAppService : ApiAppService, IGroupsAppService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IdentityUserManager _userManager;
        public GroupsAppService(
            IGroupRepository groupRepository,
            IRepository<AppUser, Guid> userRepository,
            IdentityUserManager userManager)
        {
            _groupRepository = groupRepository;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public virtual async Task<PagedResultDto<GroupDto>> GetListAsync(GetGroupsInput input)
        {
            var totalCount = await _groupRepository.GetCountAsync
            (
                input.FilterText,
                input.Title,
                input.Fid,
                input.Name,
                input.AltName,
                input.Url,
                input.IsActive,
                input.GroupSourceType,
                input.GroupOwnershipType,
                input.GroupCategoryType,
                input.ContractStatus
            );

            var items = await _groupRepository.GetListAsync
            (
                input.FilterText,
                input.Title,
                input.Fid,
                input.Name,
                input.AltName,
                input.Url,
                input.IsActive,
                input.GroupSourceType,
                input.GroupOwnershipType,
                input.GroupCategoryType,
                input.ContractStatus,
                null,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );

            return new PagedResultDto<GroupDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Group>, List<GroupDto>>(items)
            };
        }

        public virtual async Task<GroupDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Group, GroupDto>(await _groupRepository.GetAsync(id));
        }

        public virtual async Task<string> GetGroupFids(Guid userId)
        {
            var groups = await _groupRepository.GetListAsync();
            var userGroup = groups.Where(x => x.ModeratorIds.Contains(userId)).Select(x=>x.Fid).ToList();
            return userGroup.IsNotNullOrEmpty() ? string.Join(", ",userGroup) : string.Empty;
        }

        [Authorize(ApiPermissions.Groups.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _groupRepository.DeleteAsync(id);
        }

        [Authorize(ApiPermissions.Groups.Create)]
        public virtual async Task<GroupDto> CreateAsync(GroupCreateDto input)
        {
            if (input.Url.IsNullOrWhiteSpace()) throw new ApiException(LD[ApiDomainErrorCodes.Groups.NoUrl, input.Url]);
            
            input.Url = input.Url.Trim().Trim('/').Trim();
            input.GroupSourceType = FacebookHelper.GetGroupSourceTypeWithGroupUrl(input.Url);
            var existing = _groupRepository.FirstOrDefault
            (
                g => g.Fid == input.Fid.Trim() && g.GroupSourceType == input.GroupSourceType
            );
            if (existing != null) throw new ApiException(LD[ApiDomainErrorCodes.Groups.DuplicateGroup, existing.Name]);

            var group = ObjectMapper.Map<GroupCreateDto, Group>(input);
            group = CleanUp(group);
            group.TenantId = CurrentTenant.Id;
            group = await _groupRepository.InsertAsync(group, autoSave: true);
            return ObjectMapper.Map<Group, GroupDto>(group);
        }

        [Authorize(ApiPermissions.Groups.Edit)]
        public virtual async Task<GroupDto> UpdateAsync(Guid id, GroupUpdateDto input)
        {
            var group = await _groupRepository.GetAsync(id);
            ObjectMapper.Map(input, group);
            group = CleanUp(group);
            group = await _groupRepository.UpdateAsync(group);
            return ObjectMapper.Map<Group, GroupDto>(group);
        }

        public async Task DeactivatedAsync(Guid id)
        {
            var group = await _groupRepository.GetAsync(id);
            if (group != null)
            {
                group.IsActive = false;
                group.DeactivatedAt = DateTime.UtcNow;
                await _groupRepository.UpdateAsync(group);
            }
        }
        
        public async Task ActivatedAsync(Guid id)
        {
            var group = await _groupRepository.GetAsync(id);
            if (group != null)
            {
                group.IsActive = true;
                await _groupRepository.UpdateAsync(group);
            }
        }

        private Group CleanUp(Group community)
        {
            community.Fid = community.Fid.Trim();
            community.Name = community.Name.Trim();
            community.Url = community.Url.Trim().Trim('/').Trim();

            return community;
        }

        public async Task<List<LookupDto<Guid>>> GetPartnerUserLookupAsync(LookupRequestDto input)
        {
            var userIds = (await _userManager.GetUsersInRoleAsync(RoleConsts.Partner)).Select(x => x.Id).ToList();
            var query = _userRepository.AsQueryable()
                .Where(x => userIds.Contains(x.Id))
                .WhereIf
                (
                    !string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null && x.Name.Contains(input.Filter)
                )
                .OrderBy(x => x.UserName);

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AppUser>();
            return ObjectMapper.Map<List<AppUser>, List<LookupDto<Guid>>>(lookupData);
        }
        
        public async Task<List<LookupDto<Guid>>> GetStaffUserLookupAsync(LookupRequestDto input)
        {
            var userIds = (await _userManager.GetUsersInRoleAsync(RoleConsts.Staff)).Select(x => x.Id).ToList();
            var query = _userRepository.AsQueryable()
                .Where(x => userIds.Contains(x.Id))
                .WhereIf
                (
                    !string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null && x.Name.Contains(input.Filter)
                )
                .OrderBy(x => x.UserName);

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AppUser>();
            return ObjectMapper.Map<List<AppUser>, List<LookupDto<Guid>>>(lookupData);
        }
    }
}