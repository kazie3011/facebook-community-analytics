using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Integrations.Tiktok;
using FacebookCommunityAnalytics.Api.Medias;
using FacebookCommunityAnalytics.Api.TikTokMCNs;
using FacebookCommunityAnalytics.Api.Tiktoks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Groups
{
    public interface IGroupExtendAppService : IApplicationService
    {
        Task<List<GroupDto>> CreateGroupsAsync(List<GroupCreateDto> groupCreateDtos);
        Task<List<GroupDto>> GetListAsync();
        Task<GroupDto> GetAsync(GetGroupApiRequest request);
        Task<List<GroupDto>> GetManyAsync(GetGroupApiRequest request);
        Task UpdateStats(GroupStatsRequest request);
        Task CleanGroupStats();
        
        // TikTok
        Task<PagedResultDto<GroupDto>> GetListTikTokAsync(GetGroupsInput input);

        Task CreateTikTokChannelAsync(TikTokChannelCreateDto input);
        Task<List<TikTokChannelKpiModel>> GetTikTokChannelKpiModels(GetTikTokChannelKpiRequest request);

        Task<FileResultDto> GetChannelImage(Guid channelId);

        Task<List<GroupStatsViewDto>> GetTopViewChannelsTwoWeek(int numberChannel);
        Task<TikTokMCNDto> GetMCNGDLTikTok();
    }
}