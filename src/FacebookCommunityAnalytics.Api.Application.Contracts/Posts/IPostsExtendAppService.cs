using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Shared;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public interface IPostsExtendAppService : IPostsAppService
    {
        Task CreateManyAsync(PostCreateDto input);
        Task<PostDto> CreateExtendAsync(PostCreateDto input);
        Task CreatePostWithSchedule();
        
        Task<List<PostDto>> GetChartPosts(ExportExcelPostInput input);
        Task<PagedResultDto<PostWithNavigationPropertiesDto>> GetListAsync(GetPostsInputExtend input);
        Task<PagedResultDto<LookupDto<Guid?>>> GetCampaignLookupAsync(LookupRequestDto input);
        Task<PagedResultDto<LookupDto<Guid?>>> GetRunningCampaignLookup(LookupRequestDto input);
        Task<PagedResultDto<LookupDto<Guid?>>> GetPartnerLookupAsync(LookupRequestDto input);
        Task<List<PostDetailExportRow>> GetPostDetailExportRow(GetPostsInputExtend input);
        
        Task<byte[]> ExportExcelPosts(ExportExcelPostInput input);
        Task<byte[]> ExportExcelTopReactionPosts(ExportExcelTopReactionPostInput input);
    }
}