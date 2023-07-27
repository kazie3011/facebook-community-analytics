using FacebookCommunityAnalytics.Api.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Campaigns
{
    public interface ICampaignsAppService : IApplicationService
    {
        Task<PagedResultDto<CampaignWithNavigationPropertiesDto>> GetListAsync(GetCampaignsInput input);
        Task<CampaignWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);
        Task<CampaignDto> GetByIdOrCode(string idOrCode);
        Task<PagedResultDto<LookupDto<Guid?>>> GetPartnerLookupAsync(LookupRequestDto input);
        Task<List<CampaignPostDto>> GetPosts(Guid campaignId);
        Task<List<UserAffiliateWithNavigationPropertiesDto>> GetAffiliatesAsync(List<string> shortLinks);

        Task<CampaignDto> CreateAsync(CampaignCreateDto input);
        Task<CampaignDto> UpdateAsync(Guid id, CampaignUpdateDto input);
        Task UpdateCampaignPrizes(Guid id, CampaignUpdateDto input);
        Task CreateCampaignPosts(PostCreateDto input);
        Task RemoveCampaignPost(Guid postId);
        Task DeleteAsync(Guid id);
        Task SendCampaignEmail(Guid campaignId);
        Task<byte[]> ExportCampaign(Guid campaignId);
        Task<List<TiktokWithNavigationPropertiesDto>> GetTikToks(GetTiktoksInputExtend input);
        Task UpdateCampaignTiktok(TiktokCreateUpdateDto input, Guid id);
        Task<List<CampaignDto>> GetCampsByTime(DateTime from,DateTime to);
        Task<CampaignDailyChartResponse> GetCampaignDailyChartStats(Guid campaignId, DateTimeOffset fromDate, DateTimeOffset toDate); 
        Task<PieChartDataSource<double>> GetPostCountGroupsChart(DateTime fromDate, DateTime toDate); 
        Task<PieChartDataSource<double>> GetReactionGroupsChart(DateTime fromDate, DateTime toDate); 
        Task<List<AuthorStatistic>> GetAuthorStatistic();   

    }
}