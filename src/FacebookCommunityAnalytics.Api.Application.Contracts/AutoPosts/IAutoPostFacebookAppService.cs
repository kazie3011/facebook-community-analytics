using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.AutoPosts
{
    public interface IAutoPostFacebookAppService : IApplicationService
    {
        public Task<IList<AutoPostFacebookNotDoneDto>> GetUnAutoPostFacebooksAsync();
        Task<PagedResultDto<AutoPostFacebookDto>> GetListAsync(PagedResultRequestDto input);

        Task<AutoPostFacebookDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<AutoPostFacebookDto> CreateAsync(CreateUpdateAutoPostFacebookDto input);

        Task<AutoPostFacebookDto> UpdateAsync(Guid id, CreateUpdateAutoPostFacebookDto input);
        
        Task UpdateLikeCommentAsync(Guid autoPostFacebookId,int numberLike, int numberComment);
    }
}