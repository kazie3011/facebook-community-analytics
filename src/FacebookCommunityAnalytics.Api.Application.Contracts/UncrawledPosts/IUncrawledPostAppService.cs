using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.UncrawledPosts
{
    public interface IUncrawledPostsAppService : IApplicationService
    {
        Task<PagedResultDto<UncrawledPostDto>> GetListAsync(GetUncrawledPostsInput input);

        Task<UncrawledPostDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<UncrawledPostDto> CreateAsync(UncrawledPostCreateDto input);

        Task<UncrawledPostDto> UpdateAsync(Guid id, UncrawledPostUpdateDto input);
        Task HardDeleteAsync(Guid id);
    }
}