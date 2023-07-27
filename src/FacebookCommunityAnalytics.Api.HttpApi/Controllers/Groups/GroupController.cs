using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Shared;

namespace FacebookCommunityAnalytics.Api.Controllers.Groups
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Group")]
    [Route("api/app/groups")]

    public class GroupController : AbpController, IGroupsAppService
    {
        private readonly IGroupsAppService _groupsAppService;

        public GroupController(IGroupsAppService groupsAppService)
        {
            _groupsAppService = groupsAppService;
        }

        [HttpGet]
        [Route("get-list")]
        public virtual Task<PagedResultDto<GroupDto>> GetListAsync(GetGroupsInput input)
        {
            return _groupsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<GroupDto> GetAsync(Guid id)
        {
            return _groupsAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<GroupDto> CreateAsync(GroupCreateDto input)
        {
            return _groupsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<GroupDto> UpdateAsync(Guid id, GroupUpdateDto input)
        {
            return _groupsAppService.UpdateAsync(id, input);
        }

        [HttpPut]
        [Route("group-deactivated")]
        public Task DeactivatedAsync(Guid id)
        {
            return _groupsAppService.DeactivatedAsync(id);
        }

        [HttpPut]
        [Route("group-activated")]
        public Task ActivatedAsync(Guid id)
        {
            return _groupsAppService.ActivatedAsync(id);
        }

        [HttpGet]
        [Route("get-partner-user-lookup")]
        public Task<List<LookupDto<Guid>>> GetPartnerUserLookupAsync(LookupRequestDto input)
        {
            return _groupsAppService.GetPartnerUserLookupAsync(input);
        }
        
        
        [HttpGet]
        [Route("get-staff-user-lookup")]
        public Task<List<LookupDto<Guid>>> GetStaffUserLookupAsync(LookupRequestDto input)
        {
            return _groupsAppService.GetStaffUserLookupAsync(input);
        }

        [HttpGet]
        [Route("get-group-fids")]
        public Task<string> GetGroupFids(Guid userId)
        {
            return _groupsAppService.GetGroupFids(userId);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _groupsAppService.DeleteAsync(id);
        }
    }
}