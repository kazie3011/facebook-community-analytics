using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Exceptions;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace FacebookCommunityAnalytics.Api.Posts
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.Posts.Default)]
    public class PostsExtendAppService : PostsAppService, IPostsExtendAppService
    {
        private readonly IOrganizationDomainService _organizationDomainService;
        private readonly IdentityUserManager _userManager;

        private readonly IPostDomainService _postDomainService;

        private readonly IPostRepository _postRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IUserDomainService _userDomainService;

        public PostsExtendAppService(
            IOrganizationDomainService organizationDomainService,
            IdentityUserManager userManager,
            IPostDomainService postDomainService,
            IPostRepository postRepository,
            IGroupRepository groupRepository,
            ICampaignRepository campaignRepository,
            IUserDomainService userDomainService) :
            base
            (
                postRepository,
                postDomainService
            )
        {
            _userManager = userManager;
            _postRepository = postRepository;
            _groupRepository = groupRepository;
            _campaignRepository = campaignRepository;
            _userDomainService = userDomainService;
            _postDomainService = postDomainService;
            _organizationDomainService = organizationDomainService;
        }

        [Authorize(ApiPermissions.Posts.Create)]
        public virtual async Task CreateManyAsync(PostCreateDto input)
        {
            var urls = input.Url.Split("\n").Where(_ => _.IsNotNullOrWhiteSpace()).Select(_ => _.Trim()).Distinct();
            var posts = new List<Post>();
            foreach (var url in urls.Where(FacebookHelper.IsValidGroupPostUrl))
            {
                var post = ObjectMapper.Map<PostCreateDto, Post>(input);

                post.Url = url;
                post = await _postDomainService.InitPostCreation(post, CurrentUser.GetId(),null);
                if (post == null) continue;
                if (post.CampaignId != null)
                {
                    var campaign = await _campaignRepository.GetAsync((Guid)post.CampaignId);
                    post.PartnerId = campaign.PartnerId;
                    post.IsCampaignManual = true;
                }
                post.IsPostContentTypeManual = true;
                
                posts.Add(post);
            }

            if (posts.IsNotNullOrEmpty()) await _postRepository.InsertManyAsync(posts, autoSave: true);
        }

        [Authorize(ApiPermissions.Posts.Create)]
        public virtual async Task<PostDto> CreateExtendAsync(PostCreateDto input)
        {
            if (input.AppUserId == default)
            {
                throw new ApiException(L["The {0} field is required.", L["AppUser"]]);
            }

            var post = ObjectMapper.Map<PostCreateDto, Post>(input);
            post = await _postDomainService.InitPostCreation(post, CurrentUser.GetId(),null);

            post = await _postRepository.InsertAsync(post, autoSave: true);
            return ObjectMapper.Map<Post, PostDto>(post);
        }

        public async Task CreatePostWithSchedule()
        {
            var posts = await _postDomainService.CreatePostWithSchedule();
            foreach (var post in posts)
            {
                post.TenantId = CurrentTenant.Id;
            }

            if (posts.IsNotNullOrEmpty()) await _postRepository.InsertManyAsync(posts, autoSave: true);
        }

        public async Task<byte[]> ExportExcelPosts(ExportExcelPostInput input)
        {
            return await _postDomainService.ExportExcelPosts(input);
        }

        public async Task<byte[]> ExportExcelTopReactionPosts(ExportExcelTopReactionPostInput input)
        {
            return await _postDomainService.ExportExcelTopReactionPosts(input);
        }

        public async Task<List<PostDetailExportRow>> GetPostDetailExportRow(GetPostsInputExtend input)
        {
            return await _postDomainService.GetPostDetailExportRow(input);
        }

        public async Task<List<PostDto>> GetChartPosts(ExportExcelPostInput input)
        {
            var groups = await _groupRepository.GetListAsync();
            var groupFids = Array.Empty<string>();
            if (input.GroupFids.IsNotNullOrWhiteSpace())
            {
                groupFids = input.GroupFids.Split('|');
            }

            var groupIds = groupFids.Any()
                ? groups.Where(_ => groupFids.Contains(_.Fid)).ToList().Select(_ => _.Id)
                : null;

            var identityUsers = new List<IdentityUser>();
            if (input.OrganizationUnitId != null)
            {
                var orgUnit = await _organizationDomainService.GetTeam((Guid)input.OrganizationUnitId);

                identityUsers = await _userManager.GetUsersInOrganizationUnitAsync(orgUnit);
            }

            var appUserIds = identityUsers.Any() ? identityUsers.Select(_ => _.Id) : null;

            var posts = await _postRepository.GetListExtendAsync
            (
                createdDateTimeMin: input.StartDateTime,
                createdDateTimeMax: input.EndDateTime,
                postContentType: input.PostContentType,
                appUserIds: appUserIds,
                groupIds: groupIds
            );

            return ObjectMapper.Map<List<Post>, List<PostDto>>(posts);
        }

        public async Task<PagedResultDto<PostWithNavigationPropertiesDto>> GetListAsync(GetPostsInputExtend input)
        {
            if (IsManagerRole())
            {
            }
            else if (IsLeaderRole())
            {
                if (input.AppUserId != null)
                {
                    input.AppUserIds = await _userDomainService.GetManagedUserIds(CurrentUser.GetId());
                }
            }
            else if (IsStaffRole())
            {
                input.AppUserId = CurrentUser.GetId();
            }

            if (input.Url.IsNotNullOrWhiteSpace())
            {
                input.Url = FacebookHelper.GetCleanUrl(input.Url).ToString();
            }

            if (input.FilterText.IsNotNullOrWhiteSpace() && input.FilterText.Contains("http"))
            {
                input.FilterText = FacebookHelper.GetCleanUrlString(input.FilterText);
            }

            if (IsPartnerRole() && CurrentUser.Id.HasValue)
            {
                input.AppUserIds = new[] { CurrentUser.Id.Value };
            }

            var totalCount = await _postRepository.GetCountExtendAsync
            (
                input.FilterText,
                input.PostContentType,
                input.PostCopyrightType,
                input.Url,
                input.ShortUrl,
                input.LikeCountMin,
                input.LikeCountMax,
                input.CommentCountMin,
                input.CommentCountMax,
                input.ShareCountMin,
                input.ShareCountMax,
                input.TotalCountMin,
                input.TotalCountMax,
                input.Hashtag,
                input.Fid,
                input.IsNotAvailable,
                input.IsValid,
                input.Status,
                input.PostSourceType,
                input.Note,
                input.ClientOffsetInMinutes,
                input.CreatedDateTimeMin,
                input.CreatedDateTimeMax,
                input.LastCrawledDateTimeMin,
                input.LastCrawledDateTimeMax,
                input.SubmissionDateTimeMin,
                input.SubmissionDateTimeMax,
                input.CategoryId,
                input.GroupId,
                input.AppUserId,
                input.CampaignId,
                input.PartnerId,
                input.AppUserIds,
                input.GroupIds,
                input.CampaignIds
            );
            var items = await _postRepository.GetListWithNavigationPropertiesExtendAsync
            (
                input.FilterText,
                input.PostContentType,
                input.PostCopyrightType,
                input.Url,
                input.ShortUrl,
                input.LikeCountMin,
                input.LikeCountMax,
                input.CommentCountMin,
                input.CommentCountMax,
                input.ShareCountMin,
                input.ShareCountMax,
                input.TotalCountMin,
                input.TotalCountMax,
                input.Hashtag,
                input.Fid,
                input.IsNotAvailable,
                input.IsValid,
                input.Status,
                input.PostSourceType,
                input.Note,
                input.ClientOffsetInMinutes,
                input.CreatedDateTimeMin,
                input.CreatedDateTimeMax,
                input.LastCrawledDateTimeMin,
                input.LastCrawledDateTimeMax,
                input.SubmissionDateTimeMin,
                input.SubmissionDateTimeMax,
                input.CategoryId,
                input.GroupId,
                input.AppUserId,
                input.CampaignId,
                input.PartnerId,
                input.AppUserIds,
                input.GroupIds,
                input.CampaignIds,
                input.PostSourceTypes,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );

            var result = ObjectMapper.Map<List<PostWithNavigationProperties>, List<PostWithNavigationPropertiesDto>>(items);

            return new PagedResultDto<PostWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = result
            };
        }


        public async Task<PagedResultDto<LookupDto<Guid?>>> GetCampaignLookupAsync(LookupRequestDto input)
        {
            return await _postDomainService.GetCampaignLookupAsync(input);
        }

        public async Task<PagedResultDto<LookupDto<Guid?>>> GetRunningCampaignLookup(LookupRequestDto input)
        {
            return await _postDomainService.GetRunningCampaignLookup(input);
        }

        public async Task<PagedResultDto<LookupDto<Guid?>>> GetPartnerLookupAsync(LookupRequestDto input)
        {
            return await _postDomainService.GetPartnerLookupAsync(input);
        }

        [Authorize(ApiPermissions.Posts.Edit)]
        public override async Task<PostDto> UpdateAsync(Guid id, PostUpdateDto input)
        {
            var post = await _postRepository.GetAsync(id);
            if (input.CampaignId != post.CampaignId) input.IsCampaignManual = true;
            if (input.PostContentType != post.PostContentType) input.IsPostContentTypeManual = true;
            ObjectMapper.Map(input, post);
            if (post.CampaignId.IsNotNullOrEmpty())
            {
                // ReSharper disable once PossibleInvalidOperationException
                var campaign = await _campaignRepository.GetAsync(post.CampaignId.Value);
                post.PartnerId = campaign.PartnerId;
            }
            else
            {
                post.CampaignId = null;
                post.PartnerId = null;
                post.IsCampaignManual = false;
            }

            post = await _postRepository.UpdateAsync(post);
            return ObjectMapper.Map<Post, PostDto>(post);
        }
    }
}