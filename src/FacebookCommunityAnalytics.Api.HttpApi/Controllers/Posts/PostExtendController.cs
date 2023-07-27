using FacebookCommunityAnalytics.Api.Posts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.Controllers.Posts
{
    [RemoteService]
    [Area("app")]
    [ControllerName("PostExtend")]
    [Route("api/app/posts-extend")]
    public class PostExtendController : PostController, IPostsExtendAppService
    {
        private readonly IPostsExtendAppService _postsExtendAppService;
        private readonly IdentityUserManager _userManager;
        private readonly OrganizationUnitManager _organizationUnitManager;

        public PostExtendController(IPostsExtendAppService postsExtendAppService, IdentityUserManager userManager, OrganizationUnitManager organizationUnitManager) : base(postsExtendAppService)
        {
            _postsExtendAppService = postsExtendAppService;
            _userManager = userManager;
            _organizationUnitManager = organizationUnitManager;
        }
        
        [HttpGet("export-posts")]
        public Task<byte[]> ExportExcelPosts(ExportExcelPostInput input)
        {
            return _postsExtendAppService.ExportExcelPosts(input);
        }

        [HttpGet("export-top-reaction-affiliate-posts")]
        public Task<byte[]> ExportExcelTopReactionPosts(ExportExcelTopReactionPostInput input)
        {
            return _postsExtendAppService.ExportExcelTopReactionPosts(input);
        }

        [HttpGet]
        [Route("chart-posts")]
        public virtual Task<List<PostDto>> GetChartPosts(ExportExcelPostInput input)
        {
            return _postsExtendAppService.GetChartPosts(input);
        }

        [HttpGet("extend-list")]
        public async Task<PagedResultDto<PostWithNavigationPropertiesDto>> GetListAsync(GetPostsInputExtend input)
        {
            return await _postsExtendAppService.GetListAsync(input);
        }

        [HttpPost]
        [Route("create-many")]
        public Task CreateManyAsync(PostCreateDto input)
        {
            return _postsExtendAppService.CreateManyAsync(input);
        }

        [HttpPost]
        [Route("create-extend")]
        public Task<PostDto> CreateExtendAsync(PostCreateDto input)
        {
            return _postsExtendAppService.CreateExtendAsync(input);
        }

        [HttpGet("create-posts-with-scheduled-posts")]
        public Task CreatePostWithSchedule()
        {
            return _postsExtendAppService.CreatePostWithSchedule();
        }
        
        [HttpGet]
        [Route("campaign-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetCampaignLookupAsync(LookupRequestDto input)
        {
            return _postsExtendAppService.GetCampaignLookupAsync(input);
        }

        [HttpGet]
        [Route("running-campaign-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetRunningCampaignLookup(LookupRequestDto input)
        {
            return _postsExtendAppService.GetRunningCampaignLookup(input);
        }

        [HttpGet]
        [Route("partner-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetPartnerLookupAsync(LookupRequestDto input)
        {
            return _postsExtendAppService.GetPartnerLookupAsync(input);
        }

        [HttpGet("get-post-detail-export")]
        public Task<List<PostDetailExportRow>> GetPostDetailExportRow(GetPostsInputExtend input)
        {
            return _postsExtendAppService.GetPostDetailExportRow(input);
        }
    }
}
