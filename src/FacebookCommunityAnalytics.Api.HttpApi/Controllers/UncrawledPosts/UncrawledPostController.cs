using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.UncrawledPosts;

namespace FacebookCommunityAnalytics.Api.Controllers.UncrawledPosts
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UncrawledPost")]
    [Route("api/app/uncrawled-posts")]
    public class UncrawledPostController : AbpController, IUncrawledPostsAppService
    {
        private readonly IUncrawledPostsAppService _uncrawledPostsAppService;

        public UncrawledPostController(IUncrawledPostsAppService uncrawledPostsAppService)
        {
            _uncrawledPostsAppService = uncrawledPostsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<UncrawledPostDto>> GetListAsync(GetUncrawledPostsInput input)
        {
            return _uncrawledPostsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<UncrawledPostDto> GetAsync(Guid id)
        {
            return _uncrawledPostsAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<UncrawledPostDto> CreateAsync(UncrawledPostCreateDto input)
        {
            return _uncrawledPostsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<UncrawledPostDto> UpdateAsync(Guid id, UncrawledPostUpdateDto input)
        {
            return _uncrawledPostsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _uncrawledPostsAppService.DeleteAsync(id);
        }
        
        [HttpDelete]
        [Route("hard-delete-async/{id}")]
        public Task HardDeleteAsync(Guid id)
        {
            return _uncrawledPostsAppService.HardDeleteAsync(id);
        }
    }
}