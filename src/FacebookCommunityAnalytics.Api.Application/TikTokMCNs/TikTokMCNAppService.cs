using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Tiktoks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.TikTokMCNs
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.Tiktok.Default)]
    public class TikTokMCNAppService :
        BaseCrudApiAppService<TikTokMCN, TikTokMCNDto, Guid, GetTikTokMCNsInput, CreateUpdateTikTokMCNDto>, ITikTokMCNAppService
    {
        private readonly ITikTokMCNRepository _tikTokMcnRepository;

        public TikTokMCNAppService(IRepository<TikTokMCN, Guid> repository, ITikTokMCNRepository tikTokMcnRepository) : base(repository)
        {
            _tikTokMcnRepository = tikTokMcnRepository;
        }

        public override async Task<PagedResultDto<TikTokMCNDto>> GetListAsync(GetTikTokMCNsInput input)
        {
            var count = await _tikTokMcnRepository.GetCountAsync(input.FilterText);
            var items = await _tikTokMcnRepository.GetListAsync(input.FilterText);
            return new PagedResultDto<TikTokMCNDto>()
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<TikTokMCN>, List<TikTokMCNDto>>(items)
            };
        }

        public async Task<List<string>> GetHashtags()
        {
            var mcns = await _tikTokMcnRepository.GetListAsync();
            return mcns.Select(x => x.HashTag).ToList();
        }
    }
}