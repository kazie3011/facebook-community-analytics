using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Groups;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Posts;

namespace FacebookCommunityAnalytics.Api.Posts
{
    [RemoteService(IsEnabled = false)]
    [Authorize]
    public class PostsAppService : ApiAppService, IPostsAppService
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostDomainService _postDomainService;
        
        public PostsAppService(IPostRepository postRepository, IPostDomainService postDomainService)
        {
            _postRepository = postRepository;
            _postDomainService = postDomainService;
        }

        [Authorize(ApiPermissions.Posts.Default)]
        public virtual async Task<PagedResultDto<PostWithNavigationPropertiesDto>> GetListAsync(GetPostsInput input)
        {
            var totalCount = await _postRepository.GetCountAsync(input.FilterText, input.PostContentType, input.PostCopyrightType, input.Url, input.ShortUrl, input.LikeCountMin, input.LikeCountMax, input.CommentCountMin, input.CommentCountMax, input.ShareCountMin, input.ShareCountMax, input.TotalCountMin, input.TotalCountMax, input.Hashtag, input.Fid, input.IsNotAvailable, input.Status, input.PostSourceType, input.Note, input.CreatedDateTimeMin, input.CreatedDateTimeMax, input.LastCrawledDateTimeMin, input.LastCrawledDateTimeMax, input.SubmissionDateTimeMin, input.SubmissionDateTimeMax, input.CategoryId, input.GroupId, input.AppUserId, input.CampaignId, input.PartnerId);
            var items = await _postRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.PostContentType, input.PostCopyrightType, input.Url, input.ShortUrl, input.LikeCountMin, input.LikeCountMax, input.CommentCountMin, input.CommentCountMax, input.ShareCountMin, input.ShareCountMax, input.TotalCountMin, input.TotalCountMax, input.Hashtag, input.Fid, input.IsNotAvailable, input.Status, input.PostSourceType, input.Note, input.CreatedDateTimeMin, input.CreatedDateTimeMax, input.LastCrawledDateTimeMin, input.LastCrawledDateTimeMax, input.SubmissionDateTimeMin, input.SubmissionDateTimeMax, input.CategoryId, input.GroupId, input.AppUserId, input.CampaignId, input.PartnerId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<PostWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<PostWithNavigationProperties>, List<PostWithNavigationPropertiesDto>>(items)
            };
        }

        [Authorize(ApiPermissions.Posts.Default)]
        public virtual async Task<PostWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<PostWithNavigationProperties, PostWithNavigationPropertiesDto>
                (await _postRepository.GetWithNavigationPropertiesAsync(id));
        }

        [Authorize(ApiPermissions.Posts.Default)]
        public virtual async Task<PostDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Post, PostDto>(await _postRepository.GetAsync(id));
        }

        public async Task<PagedResultDto<LookupDto<Guid?>>> GetCategoryLookupAsync(LookupRequestDto input)
        {
            return await _postDomainService.GetCategoryLookupAsync(input);
        }
        public async Task<PagedResultDto<LookupDto<Guid?>>> GetGroupLookupAsync(GroupLookupRequestDto input)
        {
            return await _postDomainService.GetGroupLookupAsync(input);
        }

        public async Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input)
        {
            return await _postDomainService.GetAppUserLookupAsync(input);
        }

        [Authorize(ApiPermissions.Posts.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _postRepository.DeleteAsync(id);
        }

        [Authorize(ApiPermissions.Posts.Create)]
        public virtual async Task<PostDto> CreateAsync(PostCreateDto input)
        {
            var post = ObjectMapper.Map<PostCreateDto, Post>(input);
            post.TenantId = CurrentTenant.Id;
            post = await _postRepository.InsertAsync(post, autoSave: true);
            return ObjectMapper.Map<Post, PostDto>(post);
        }

        [Authorize(ApiPermissions.Posts.Edit)]
        public virtual async Task<PostDto> UpdateAsync(Guid id, PostUpdateDto input)
        {

            var post = await _postRepository.GetAsync(id);
            ObjectMapper.Map(input, post);
            post = await _postRepository.UpdateAsync(post);
            return ObjectMapper.Map<Post, PostDto>(post);
        }
    }
}