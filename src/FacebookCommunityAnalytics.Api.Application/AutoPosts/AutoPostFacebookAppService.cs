using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.AutoPosts
{
    [RemoteService(IsEnabled = false)]
    [Authorize]
    public class AutoPostFacebookAppService : ApplicationService, IAutoPostFacebookAppService
    {
        private readonly IRepository<AutoPostFacebook> _autoPostFacebookRepository;
        private readonly IAutoPostFacebookDomainService _autoPostFacebookDomainService;
        public AutoPostFacebookAppService(IRepository<AutoPostFacebook> autoPostFacebookRepository, IAutoPostFacebookDomainService autoPostFacebookDomainService)
        {
            _autoPostFacebookRepository = autoPostFacebookRepository;
            _autoPostFacebookDomainService = autoPostFacebookDomainService;
        }

        public async Task<IList<AutoPostFacebookNotDoneDto>> GetUnAutoPostFacebooksAsync()
        {
            var data = await _autoPostFacebookDomainService.GetUnAutoPostFacebooksAsync();
            return ObjectMapper.Map<IList<AutoPostFacebookNotDone>, IList<AutoPostFacebookNotDoneDto>>(data);
        }

        public async Task<PagedResultDto<AutoPostFacebookDto>> GetListAsync(PagedResultRequestDto input)
        {
            var query = await _autoPostFacebookRepository.GetQueryableAsync();
            var count = query.Count();
            var data = query.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
            
            return new PagedResultDto<AutoPostFacebookDto>()
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<AutoPostFacebook>, List<AutoPostFacebookDto>>(data)
            };
        }

        public async Task<AutoPostFacebookDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<AutoPostFacebook, AutoPostFacebookDto>(await _autoPostFacebookRepository.GetAsync(x=>x.Id == id));
        }

        public Task DeleteAsync(Guid id)
        {
            return _autoPostFacebookRepository.DeleteAsync(x => x.Id == id);
        }

        public async Task<AutoPostFacebookDto> CreateAsync(CreateUpdateAutoPostFacebookDto input)
        {
            var entity = ObjectMapper.Map<CreateUpdateAutoPostFacebookDto, AutoPostFacebook>(input);
            var created = await _autoPostFacebookRepository.InsertAsync(entity);
            
            return ObjectMapper.Map<AutoPostFacebook, AutoPostFacebookDto>(created);
        }

        public async Task<AutoPostFacebookDto> UpdateAsync(Guid id, CreateUpdateAutoPostFacebookDto input)
        {
            var entity = await _autoPostFacebookRepository.GetAsync(x => x.Id == id);
            ObjectMapper.Map(input, entity);
            await _autoPostFacebookRepository.UpdateAsync(entity);
            return ObjectMapper.Map<AutoPostFacebook, AutoPostFacebookDto>(entity);
        }

        public async Task UpdateLikeCommentAsync(Guid autoPostFacebookId, int numberLike, int numberComment)
        {
            await _autoPostFacebookDomainService.UpdateLikeCommentAsync(autoPostFacebookId, numberLike, numberComment);
        }
    }
}