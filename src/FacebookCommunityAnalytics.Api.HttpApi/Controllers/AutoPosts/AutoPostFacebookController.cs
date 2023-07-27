using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AutoPosts;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Controllers.AutoPosts
{
    [RemoteService]
    [Area("app")]
    [ControllerName("AutoPostFacebook")]
    [Route("api/app/auto-post-facebook")]
    public class AutoPostFacebookController : ApiController, IAutoPostFacebookAppService
    {
        private readonly IAutoPostFacebookAppService _autoPostFacebookAppService;

        public AutoPostFacebookController(IAutoPostFacebookAppService autoPostFacebookAppService)
        {
            _autoPostFacebookAppService = autoPostFacebookAppService;
        }

        [HttpGet("get-posts-not-done")]
        public Task<IList<AutoPostFacebookNotDoneDto>> GetUnAutoPostFacebooksAsync()
        {
            return _autoPostFacebookAppService.GetUnAutoPostFacebooksAsync();
        }

        [HttpGet]
        public Task<PagedResultDto<AutoPostFacebookDto>> GetListAsync(PagedResultRequestDto input)
        {
            return _autoPostFacebookAppService.GetListAsync(input);
        }

        [HttpGet("{id}")]
        public Task<AutoPostFacebookDto> GetAsync(Guid id)
        {
            return _autoPostFacebookAppService.GetAsync(id);
        }

        [HttpDelete("{id}")]
        public Task DeleteAsync(Guid id)
        {
            return _autoPostFacebookAppService.DeleteAsync(id);
        }

        [HttpPost]
        public Task<AutoPostFacebookDto> CreateAsync(CreateUpdateAutoPostFacebookDto input)
        {
            return _autoPostFacebookAppService.CreateAsync(input);
        }

        [HttpPut("{id}")]
        public Task<AutoPostFacebookDto> UpdateAsync(Guid id, CreateUpdateAutoPostFacebookDto input)
        {
            return _autoPostFacebookAppService.UpdateAsync(id, input);
        }

        [HttpPost("update-number-like-comment")]
        public Task UpdateLikeCommentAsync(Guid autoPostFacebookId, int numberLike, int numberComment)
        {
            return _autoPostFacebookAppService.UpdateLikeCommentAsync(autoPostFacebookId, numberLike, numberComment);
        }
    }
}