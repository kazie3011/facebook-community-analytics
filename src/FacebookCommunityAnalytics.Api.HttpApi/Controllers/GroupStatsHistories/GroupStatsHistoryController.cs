using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.GroupStatsHistories
{
    [RemoteService]
    [Area("app")]
    [ControllerName("GroupStatsHistory")]
    [Route("api/app/group-stats-history")]
    public class GroupStatsHistoryController : AbpController, IGroupStatsHistoryAppService
    {
        private readonly IGroupStatsHistoryAppService _groupStatsHistoryAppService;

        public GroupStatsHistoryController(IGroupStatsHistoryAppService groupStatsHistoryAppService)
        {
            _groupStatsHistoryAppService = groupStatsHistoryAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<GroupStatsHistoryDto> GetAsync(Guid id)
        {
            return _groupStatsHistoryAppService.GetAsync(id);
        }

        [HttpGet]
        public Task<PagedResultDto<GroupStatsHistoryDto>> GetListAsync(GetGroupStatsHistoryInput input)
        {
            return _groupStatsHistoryAppService.GetListAsync(input);
        }

        [HttpPost]
        public async Task<GroupStatsHistoryDto> CreateAsync(GroupStatsHistoryCreateUpdateDto input)
        {
            return await _groupStatsHistoryAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public Task<GroupStatsHistoryDto> UpdateAsync(Guid id, GroupStatsHistoryCreateUpdateDto  input)
        {
            return _groupStatsHistoryAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public Task DeleteAsync(Guid id)
        {
            return _groupStatsHistoryAppService.DeleteAsync(id);
        }
    }
}