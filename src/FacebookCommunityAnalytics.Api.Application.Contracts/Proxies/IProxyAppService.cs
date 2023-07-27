using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Proxies
{
    public interface IProxiesAppService : IApplicationService
    {
        Task<PagedResultDto<ProxyDto>> GetListAsync(GetProxiesInput input);

        Task<ProxyDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<ProxyDto> CreateAsync(ProxyCreateDto input);

        Task<ProxyDto> UpdateAsync(Guid id, ProxyUpdateDto input);

    }
}