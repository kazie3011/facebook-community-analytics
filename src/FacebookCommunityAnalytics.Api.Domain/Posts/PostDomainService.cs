using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Categories;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Exceptions;
using FacebookCommunityAnalytics.Api.Exports;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using static System.Int32;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public interface IPostDomainService : IDomainService
    {
        Task<List<Post>> GetPosts(List<string> urls);
        Task<Post> InitPostCreation(Post post, Guid authorUserId,List<Guid> partnerUserIds);
        Task<List<Post>> CreatePostWithSchedule();
        Task<byte[]> ExportExcelPosts(ExportExcelPostInput input);
        Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input);
        Task<PagedResultDto<LookupDto<Guid?>>> GetGroupLookupAsync(GroupLookupRequestDto input);
        Task<PagedResultDto<LookupDto<Guid?>>> GetCategoryLookupAsync(LookupRequestDto input);
        Task<PagedResultDto<LookupDto<Guid?>>> GetCampaignLookupAsync(LookupRequestDto input);
        Task<PagedResultDto<LookupDto<Guid?>>> GetRunningCampaignLookup(LookupRequestDto input);
        Task<PagedResultDto<LookupDto<Guid?>>> GetPartnerLookupAsync(LookupRequestDto input);
        Task<byte[]> ExportExcelTopReactionPosts(ExportExcelTopReactionPostInput input);
        Task<List<PostDetailExportRow>> GetPostDetailExportRow(GetPostsInputExtend input);
        
        Task<List<PostDetailExportRow>> GetEvaluationPostDetailExportRow(GetStaffEvaluationsInput input);
        Task InitPostHistories();
    }

    public class PostDomainService : BaseDomainService, IPostDomainService
    {
        private readonly IOrganizationDomainService _organizationDomainService;
        private readonly IdentityUserManager _userManager;
        private readonly IGroupDomainService _groupDomainService;
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IScheduledPostDomainService _scheduledPostDomainService;
        private readonly IGroupRepository _groupRepository;
        private readonly IRepository<AppUser, Guid> _appUserRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IPartnerRepository _partnerRepository;
        private readonly IExportDomainService _exportDomainService;
        private readonly IRepository<PostHistory, Guid> _postHistoryRepository;

        public PostDomainService(
            IGroupDomainService groupDomainService,
            IPostRepository postRepository,
            ICategoryRepository categoryRepository,
            IScheduledPostDomainService scheduledPostDomainService,
            IGroupRepository groupRepository,
            IOrganizationDomainService organizationDomainService,
            IdentityUserManager userManager,
            IRepository<AppUser, Guid> appUserRepository,
            ICampaignRepository campaignRepository,
            IPartnerRepository partnerRepository,
            IExportDomainService exportDomainService,
            IRepository<PostHistory, Guid> postHistoryRepository)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
            _scheduledPostDomainService = scheduledPostDomainService;
            _groupRepository = groupRepository;
            _organizationDomainService = organizationDomainService;
            _userManager = userManager;
            _appUserRepository = appUserRepository;
            _campaignRepository = campaignRepository;
            _partnerRepository = partnerRepository;
            _exportDomainService = exportDomainService;
            _postHistoryRepository = postHistoryRepository;
            _groupDomainService = groupDomainService;
        }

        public async Task<List<Post>> GetPosts(List<string> urls)
        {
            return await _postRepository.GetAsync(urls);
        }

        public async Task<Post> InitPostCreation(Post post, Guid authorUserId, List<Guid> partnerUserIds)
        {
            if (post != null)
            {
                if (post.CampaignId == Guid.Empty) post.CampaignId = null;
                if (post.PartnerId == Guid.Empty) post.PartnerId = null;
                if (post.CategoryId == Guid.Empty) post.CategoryId = null;
                if (post.GroupId == Guid.Empty) post.GroupId = null;
                post.TenantId = CurrentTenant.Id;
            }
            else
            {
                throw new ApiException(L[ApiDomainErrorCodes.Posts.PostNullData]);
            }

            var cleanUrl = post.Url;
            if (cleanUrl.IsNullOrEmpty()) throw new ApiException(LD[ApiDomainErrorCodes.Posts.InvalidUrl, cleanUrl]);
            switch (GetPostUrlType(cleanUrl))
            {
                case PostUrlType.Facebook:
                {
                    if (FacebookHelper.IsNotValidGroupPostUrl(cleanUrl))
                    {
                        throw new ApiException(LD[ApiDomainErrorCodes.Posts.InvalidUrl, cleanUrl]);
                    }
                    cleanUrl = FacebookHelper.GetCleanUrl(cleanUrl).ToString();

                    var isFbVideo = FacebookHelper.IsFbVideo(cleanUrl);

                    if (!isFbVideo)
                    {
                        var groupFid = FacebookHelper.GetGroupFid(cleanUrl);
                        var group = await _groupDomainService.GetOrCreateAsync
                        (
                            new GetGroupApiRequest()
                            {
                                GroupFid = groupFid,
                                GroupSourceType = FacebookHelper.GetGroupSourceTypeWithPostUrl(cleanUrl),
                                PartnerUserIds = partnerUserIds,
                                PartnerId = post.PartnerId
                            }
                        );
                        if (group == null) throw new ApiException(LD[ApiDomainErrorCodes.Groups.NotExist, groupFid]);
                        
                        post.Url = cleanUrl;
                        post.GroupId = group.Id;
                        post.PostSourceType = MapPostSourceType(group.GroupSourceType);
                        var isValidUrl = FacebookHelper.IsValidUrl(cleanUrl, post.PostSourceType);
                        if (!isValidUrl) throw new ApiException(LD[ApiDomainErrorCodes.Posts.InvalidUrl, cleanUrl]);
                    }
                    else
                    {
                        post.Url = cleanUrl;
                        post.PostSourceType = PostSourceType.Video;
                        var isValidUrl = FacebookHelper.IsValidUrl(cleanUrl, post.PostSourceType);
                        if (!isValidUrl) throw new ApiException(LD[ApiDomainErrorCodes.Posts.InvalidUrl, cleanUrl]);
                    }
                    break;
                }

                case PostUrlType.Instagram:
                {
                    if (InstagramHelper.IsNotValidUrl(cleanUrl)) throw new ApiException(LD[ApiDomainErrorCodes.Posts.InvalidUrl, cleanUrl]);
                    cleanUrl = InstagramHelper.GetCleanUrl(post.Url).ToString();

                    if (!post.GroupId.HasValue) throw new ApiException(LD[ApiDomainErrorCodes.Groups.InsMustHaveGroup]);
                    var group = await _groupRepository.GetAsync(post.GroupId.Value);
                    if (group == null) { throw new ApiException(LD[ApiDomainErrorCodes.Groups.NotExist, post.GroupId.Value]); }

                    post.Url = cleanUrl;
                    post.GroupId = group.Id;
                    post.PostSourceType = MapPostSourceType(group.GroupSourceType);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var dupPostCount = await _postRepository.GetCountExtendAsync(url: post.Url);
            if (dupPostCount > 0) return null;
            post.TenantId = CurrentTenant.Id;
            post.AppUserId = authorUserId;
            post.IsValid = true;
            post.SubmissionDateTime = DateTime.UtcNow;
            post.CreatedDateTime = null;
            post.LastCrawledDateTime = null;

            post.Fid = post.PostSourceType == PostSourceType.Instagram
                ? string.Empty
                : FacebookHelper.GetGroupPostFid(post.Url);

            if (post.CategoryId.IsNotNullOrEmpty())
            {
                var category = await _categoryRepository.GetAsync(post.CategoryId.GetValueOrDefault());
                if (category == null)
                {
                    throw new ApiException
                    (
                        L[ApiDomainErrorCodes.Categories.NotExist,
                            post.CategoryId.GetValueOrDefault()]
                    );
                }
            }

            return post;
        }

        private PostSourceType MapPostSourceType(GroupSourceType groupSourceType)
        {
            switch (groupSourceType)
            {
                case GroupSourceType.Group:
                    return PostSourceType.Group;
                case GroupSourceType.Page:
                    return PostSourceType.Page;
                case GroupSourceType.Website:
                    return PostSourceType.Website;
                case GroupSourceType.Instagram:
                    return PostSourceType.Instagram;
                default:
                    throw new ArgumentOutOfRangeException(nameof(groupSourceType), groupSourceType, null);
            }
        }

        public async Task<List<Post>> CreatePostWithSchedule()
        {
            List<Post> posts = new();

            var scheduledPosts = await _scheduledPostDomainService.GetListScheduledPostHadPosted();
            foreach (var scheduledPost in scheduledPosts)
            {
                if (!scheduledPost.IsPosted) return posts;

                if (scheduledPost.AppUserId == null)
                    throw new ApiException(LD[ApiDomainErrorCodes.ScheduledPosts.SyncFailError]);

                foreach (var url in scheduledPost.Url.Trim().Split(','))
                {
                    if (!scheduledPost.GroupIds.Trim()
                        .Split(',')
                        .ToList()
                        .Contains(FacebookHelper.GetGroupPostFid(url))) { throw new ApiException(LD[ApiDomainErrorCodes.ScheduledPosts.SyncFailError]); }

                    var post = new Post()
                    {
                        Url = FacebookHelper.GetCleanUrl(url).ToString(),
                        AppUserId = scheduledPost.AppUserId,
                        CategoryId = scheduledPost.CategoryId,
                        PostCopyrightType = scheduledPost.PostCopyrightType,
                        PostContentType = scheduledPost.PostContentType,
                    };
                    post = await InitPostCreation(post, (Guid)post.AppUserId, null);

                    posts.Add(post);
                }
            }

            return posts;
        }

        public async Task<List<PostDetailExportRow>> GetPostDetailExportRow(GetPostsInputExtend input)
        {
            input.MaxResultCount = MaxValue;
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
            return ObjectMapper.Map<List<PostWithNavigationProperties>, List<PostDetailExportRow>>(items);
        }

        public async Task<List<PostDetailExportRow>> GetEvaluationPostDetailExportRow(GetStaffEvaluationsInput input)
        {
            if (!input.AppUserId.HasValue)
            {
                return new List<PostDetailExportRow>();
            }
            var posts = await _postRepository.GetPostsByEvaluationAsync(
                input.AppUserId.Value,
                input.Month,
                input.Year
                );
            
            return ObjectMapper.Map<List<PostWithNavigationProperties>, List<PostDetailExportRow>>(posts);
        }

        public async Task<byte[]> GetPostDetailExcelAsync(GetPostsInputExtend input)
        {
            input.MaxResultCount = MaxValue;
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

            return _exportDomainService.GeneratePostDetailExcelBytes(items, string.Empty);
        }

        public async Task<byte[]> ExportExcelPosts(ExportExcelPostInput input)
        {
            var groups = await _groupRepository.GetListAsync();
            var groupFids = Array.Empty<string>();
            if (input.GroupFids.IsNotNullOrWhiteSpace()) { groupFids = input.GroupFids.Split('|'); }

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

            var items = await _postRepository.GetListWithNavigationPropertiesExtendAsync
            (
                postContentType: input.PostContentType,
                createdDateTimeMax: input.EndDateTime,
                createdDateTimeMin: input.StartDateTime,
                appUserId: input.UserId,
                groupId: input.GroupId,
                appUserIds: appUserIds,
                groupIds: groupIds
            );

            return _exportDomainService.GeneratePostExcelBytes(items.Take(input.RequiredQuantity).ToList(), string.Empty);
        }

        public async Task<byte[]> ExportExcelTopReactionPosts(ExportExcelTopReactionPostInput input)
        {
            var items = await _postRepository.GetListWithNavigationPropertiesExtendAsync
            (
                postContentType: input.PostContentType,
                createdDateTimeMax: input.EndDateTime,
                createdDateTimeMin: input.StartDateTime
            );

            return _exportDomainService.GeneratePostExcelBytes(items.OrderByDescending(_ => _.Post.TotalCount).Take(input.RequiredQuantity).ToList(), string.Empty);
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input)
        {
            var result = _appUserRepository.AsQueryable()
                .WhereIf
                (
                    input.Filter.IsNotNullOrWhiteSpace(),
                    u => u.Name.ToLower().RemoveDiacritics().Contains(input.Filter.ToLower().RemoveDiacritics())
                )
                .OrderBy(_ => _.UserName)
                .ToList();

            return new PagedResultDto<LookupDto<Guid?>>
            {
                TotalCount = result.Count,
                Items = ObjectMapper.Map<List<AppUser>, List<LookupDto<Guid?>>>(result)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetCategoryLookupAsync(LookupRequestDto input)
        {
            var result = _categoryRepository.AsQueryable()
                .WhereIf
                (
                    input.Filter.IsNotNullOrWhiteSpace(),
                    u => u.Name.ToLower().RemoveDiacritics().Contains(input.Filter.ToLower().RemoveDiacritics())
                )
                .ToList();

            return new PagedResultDto<LookupDto<Guid?>>
            {
                TotalCount = result.Count,
                Items = ObjectMapper.Map<List<Category>, List<LookupDto<Guid?>>>(result)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetGroupLookupAsync(GroupLookupRequestDto input)
        {
            var filterText = string.Empty;
            if (input.Filter.IsNotNullOrWhiteSpace()) { filterText = input.Filter.ToLower().RemoveDiacritics().Trim(); }

            var groups = _groupRepository.AsQueryable()
                .WhereIf(filterText.IsNotNullOrWhiteSpace(), g => g.Title != null && g.Title.ToLower().RemoveDiacritics().IndexOf(filterText, 0, StringComparison.CurrentCultureIgnoreCase) >= 0)
                .WhereIf(input.GroupSourceType.HasValue, x => x.GroupSourceType == input.GroupSourceType)
                .WhereIf(input.CreatorId.HasValue, x=>x.CreatorId == input.CreatorId)
                .OrderBy(x => x.Title)
                .ToList();

            var lookupData = groups;
            var totalCount = groups.Count();
            return new PagedResultDto<LookupDto<Guid?>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Group>, List<LookupDto<Guid?>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetCampaignLookupAsync(LookupRequestDto input)
        {
            var filterText = string.Empty;
            if (input.Filter.IsNotNullOrWhiteSpace()) { filterText = input.Filter.ToLower().RemoveDiacritics().Trim(); }

            var result = _campaignRepository.AsQueryable().WhereIf
                (
                    filterText.IsNotNullOrWhiteSpace(),
                    c => c.Name.ToLower().RemoveDiacritics().Trim().Contains(filterText)
                )
                .WhereIf(input.CreatorId.HasValue, x=>x.CreatorId == input.CreatorId)
                .ToList();

            return new PagedResultDto<LookupDto<Guid?>>
            {
                TotalCount = result.Count,
                Items = ObjectMapper.Map<List<Campaign>, List<LookupDto<Guid?>>>(result)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetRunningCampaignLookup(LookupRequestDto input)
        {
            var filterText = string.Empty;
            if (input.Filter.IsNotNullOrWhiteSpace()) { filterText = input.Filter.ToLower().RemoveDiacritics().Trim(); }

            var result = _campaignRepository.AsQueryable()
                .Where(_ => _.Status == CampaignStatus.Started || _.Status == CampaignStatus.Hold)
                .WhereIf(filterText.IsNotNullOrWhiteSpace(), c => c.Name.ToLower().RemoveDiacritics().Trim().Contains(filterText))
                .WhereIf(input.CreatorId.HasValue, x=>x.CreatorId == input.CreatorId)
                .ToList();

            return new PagedResultDto<LookupDto<Guid?>>
            {
                TotalCount = result.Count,
                Items = ObjectMapper.Map<List<Campaign>, List<LookupDto<Guid?>>>(result)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetPartnerLookupAsync(LookupRequestDto input)
        {
            var result = _partnerRepository.AsQueryable().WhereIf
                (
                    input.Filter.IsNotNullOrWhiteSpace(),
                    c => c.Name.ToLower().RemoveDiacritics().Contains(input.Filter.ToLower().RemoveDiacritics())
                )
                .ToList();

            return new PagedResultDto<LookupDto<Guid?>>
            {
                TotalCount = result.Count,
                Items = ObjectMapper.Map<List<Partner>, List<LookupDto<Guid?>>>(result)
            };
        }

        public async Task InitPostHistories()
        {
            var postHistoriesExist = await _postHistoryRepository.GetListAsync(x => x.CreatedDateTime == DateTime.UtcNow.Date);
            var postIds = postHistoriesExist.Select(x => x.PostId).ToList();
            
            var posts = await _postRepository.GetListAsync(_ => 
                _.CampaignId != null 
                && !_.IsNotAvailable
                && (postIds.Count == 0 || !postIds.Contains(_.Id))
                );
            var postHistories = posts.Select
                (
                    post => new PostHistory()
                    {
                        PostId = post.Id,
                        Url = post.Url,
                        LikeCount = post.LikeCount,
                        ViewCount = post.ViewCount,
                        CommentCount = post.CommentCount,
                        ShareCount = post.ShareCount,
                        TotalCount = post.TotalCount,
                        CreatedDateTime = DateTime.UtcNow.Date
                    }
                )
                .ToList();

            if (postHistories.IsNotNullOrEmpty())
            {
                foreach (var batch in postHistories.Partition(100))
                {
                    await _postHistoryRepository.InsertManyAsync(batch);
                }
            }
        }
        private PostUrlType GetPostUrlType(string url)
        {
            if (url.Contains("facebook.com")) return PostUrlType.Facebook;
            if (url.Contains("instagram.com")) return PostUrlType.Instagram;
            return PostUrlType.Unknown;
        }
    }
}