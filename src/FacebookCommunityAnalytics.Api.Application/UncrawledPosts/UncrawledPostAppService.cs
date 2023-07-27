using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.UncrawledPosts;

namespace FacebookCommunityAnalytics.Api.UncrawledPosts
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.UncrawledPosts.Default)]
    public class UncrawledPostsAppService : ApplicationService, IUncrawledPostsAppService
    {
        private readonly IUncrawledPostRepository _uncrawledPostRepository;

        public UncrawledPostsAppService(IUncrawledPostRepository uncrawledPostRepository)
        {
            _uncrawledPostRepository = uncrawledPostRepository;
        }

        public virtual async Task<PagedResultDto<UncrawledPostDto>> GetListAsync(GetUncrawledPostsInput input)
        {
            var totalCount = await _uncrawledPostRepository.GetCountAsync(input.FilterText, input.Url, input.PostSourceType, input.UpdatedAtMin, input.UpdatedAtMax);
            var items = await _uncrawledPostRepository.GetListAsync(input.FilterText, input.Url, input.PostSourceType, input.UpdatedAtMin, input.UpdatedAtMax, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<UncrawledPostDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<UncrawledPost>, List<UncrawledPostDto>>(items)
            };
        }

        public virtual async Task<UncrawledPostDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<UncrawledPost, UncrawledPostDto>(await _uncrawledPostRepository.GetAsync(id));
        }

        [Authorize(ApiPermissions.UncrawledPosts.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _uncrawledPostRepository.DeleteAsync(id);
        }
        
        [Authorize(ApiPermissions.UncrawledPosts.Delete)]
        public virtual async Task HardDeleteAsync(Guid id)
        {
            var uncrawlPost = await _uncrawledPostRepository.GetAsync(id);
            await _uncrawledPostRepository.HardDeleteAsync(uncrawlPost);
        }

        [Authorize(ApiPermissions.UncrawledPosts.Create)]
        public virtual async Task<UncrawledPostDto> CreateAsync(UncrawledPostCreateDto input)
        {

            var uncrawledPost = ObjectMapper.Map<UncrawledPostCreateDto, UncrawledPost>(input);
            uncrawledPost.TenantId = CurrentTenant.Id;
            uncrawledPost = await _uncrawledPostRepository.InsertAsync(uncrawledPost, autoSave: true);
            return ObjectMapper.Map<UncrawledPost, UncrawledPostDto>(uncrawledPost);
        }

        [Authorize(ApiPermissions.UncrawledPosts.Edit)]
        public virtual async Task<UncrawledPostDto> UpdateAsync(Guid id, UncrawledPostUpdateDto input)
        {

            var uncrawledPost = await _uncrawledPostRepository.GetAsync(id);
            ObjectMapper.Map(input, uncrawledPost);
            uncrawledPost = await _uncrawledPostRepository.UpdateAsync(uncrawledPost);
            return ObjectMapper.Map<UncrawledPost, UncrawledPostDto>(uncrawledPost);
        }
    }
}