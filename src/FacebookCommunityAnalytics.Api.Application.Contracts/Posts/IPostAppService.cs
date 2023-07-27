using FacebookCommunityAnalytics.Api.Shared;
using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Groups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public interface IPostsAppService : IApplicationService
    {
        Task<PagedResultDto<PostWithNavigationPropertiesDto>> GetListAsync(GetPostsInput input);

        Task<PostWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<PostDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid?>>> GetCategoryLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid?>>> GetGroupLookupAsync(GroupLookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<PostDto> CreateAsync(PostCreateDto input);

        Task<PostDto> UpdateAsync(Guid id, PostUpdateDto input);
    }
}