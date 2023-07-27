using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Integrations.Tiktok;
using FacebookCommunityAnalytics.Api.Medias;
using FacebookCommunityAnalytics.Api.TikTokMCNs;
using FacebookCommunityAnalytics.Api.Tiktoks;

namespace FacebookCommunityAnalytics.Api.Controllers.Groups
{
    [RemoteService]
    [Area("app")]
    [ControllerName("GroupExtend")]
    [Route("api/app/groups-extend")]
    public class GroupExtendController : AbpController,IGroupExtendAppService
    {
        private readonly IGroupExtendAppService _groupExtendAppService;

        public GroupExtendController(IGroupExtendAppService groupExtendAppService)
        {    
            _groupExtendAppService = groupExtendAppService;
        }

        [HttpPost]
        public virtual Task<List<GroupDto>> CreateGroupsAsync(List<GroupCreateDto> groupCreateDtos)
        {
            return _groupExtendAppService.CreateGroupsAsync(groupCreateDtos);
        }

        [HttpGet]
        [Route("get-list")]
        public Task<List<GroupDto>> GetListAsync()
        {
            return _groupExtendAppService.GetListAsync();
        }
        
        [HttpGet]
        [Route("get")]
        public Task<GroupDto> GetAsync(GetGroupApiRequest request)
        {
            return _groupExtendAppService.GetAsync(request);
        }

        [HttpGet]
        [Route("get-many")]
        public Task<List<GroupDto>> GetManyAsync(GetGroupApiRequest request)
        {
            return _groupExtendAppService.GetManyAsync(request);
        }

        [HttpPost]
        [Route("update-statistics")]
        public Task UpdateStats(GroupStatsRequest request)
        {
            return _groupExtendAppService.UpdateStats(request);
        }

        [HttpPost] 
        [Route("clean-group-stats")]
        public Task CleanGroupStats()
        {
            return _groupExtendAppService.CleanGroupStats();
        }

        [HttpGet]
        [Route("get-tiktok-channels")]
        public Task<PagedResultDto<GroupDto>> GetListTikTokAsync(GetGroupsInput input)
        {
            return _groupExtendAppService.GetListTikTokAsync(input);
        }
        
        [HttpPost] 
        [Route("create-tiktok-channel")]
        public Task CreateTikTokChannelAsync(TikTokChannelCreateDto input)
        {
            return _groupExtendAppService.CreateTikTokChannelAsync(input);
        }

        [HttpGet] 
        [Route("get-tiktok-channel-kpi-models")]
        public Task<List<TikTokChannelKpiModel>> GetTikTokChannelKpiModels(GetTikTokChannelKpiRequest request)
        {
            return _groupExtendAppService.GetTikTokChannelKpiModels(request);
        }
        
        [HttpGet("get-channel-image-file-{channelId}")]
        public Task<FileResultDto> GetChannelImage(Guid channelId)
        {
            return _groupExtendAppService.GetChannelImage(channelId);
        }
        
        [HttpGet("get-top-view-channels-two-week")]
        public Task<List<GroupStatsViewDto>> GetTopViewChannelsTwoWeek(int numberChannel)
        {
            return _groupExtendAppService.GetTopViewChannelsTwoWeek(20);
        }

        [HttpGet("get-mcn-gdl-tiktok")]
        public Task<TikTokMCNDto> GetMCNGDLTikTok()
        {
            return _groupExtendAppService.GetMCNGDLTikTok();
        }

        [Route("tiktok-channel-image-{id}")]
        [HttpGet]
        public async Task<ActionResult> GetTikTokChannelImage(Guid id)
        {
            var result = await _groupExtendAppService.GetChannelImage(id);
            return File(result.FileData, contentType: result.ContentType);
        }
    }
}