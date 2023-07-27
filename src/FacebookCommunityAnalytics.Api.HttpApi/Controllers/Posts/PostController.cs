using FacebookCommunityAnalytics.Api.Shared;
using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Groups;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.Posts;

namespace FacebookCommunityAnalytics.Api.Controllers.Posts
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Post")]
    [Route("api/app/posts")]
    public class PostController : AbpController, IPostsAppService
    {
        private readonly IPostsAppService _postsAppService;

        public PostController(IPostsAppService postsAppService)
        {
            _postsAppService = postsAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<PostWithNavigationPropertiesDto>> GetListAsync(GetPostsInput input)
        {
            return _postsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<PostWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _postsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<PostDto> GetAsync(Guid id)
        {
            return _postsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("category-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetCategoryLookupAsync(LookupRequestDto input)
        {
            return _postsAppService.GetCategoryLookupAsync(input);
        }

        [HttpGet]
        [Route("group-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetGroupLookupAsync(GroupLookupRequestDto input)
        {
            return _postsAppService.GetGroupLookupAsync(input);
        }

        [HttpGet]
        [Route("app-user-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input)
        {
            return _postsAppService.GetAppUserLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<PostDto> CreateAsync(PostCreateDto input)
        {
            return _postsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<PostDto> UpdateAsync(Guid id, PostUpdateDto input)
        {
            return _postsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _postsAppService.DeleteAsync(id);
        }
    }
}