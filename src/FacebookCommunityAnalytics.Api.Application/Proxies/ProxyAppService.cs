using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Proxies;

namespace FacebookCommunityAnalytics.Api.Proxies
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.Proxies.Default)]
    public class ProxiesAppService : ApplicationService, IProxiesAppService
    {
        private readonly IProxyRepository _proxyRepository;

        public ProxiesAppService(IProxyRepository proxyRepository)
        {
            _proxyRepository = proxyRepository;
        }

        public virtual async Task<PagedResultDto<ProxyDto>> GetListAsync(GetProxiesInput input)
        {
            var totalCount = await _proxyRepository.GetCountAsync(input.FilterText, input.Ip, input.PortMin, input.PortMax, input.Protocol, input.Username, input.Password, input.LastPingDateTimeMin, input.LastPingDateTimeMax, input.IsActive);
            var items = await _proxyRepository.GetListAsync(input.FilterText, input.Ip, input.PortMin, input.PortMax, input.Protocol, input.Username, input.Password, input.LastPingDateTimeMin, input.LastPingDateTimeMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ProxyDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Proxy>, List<ProxyDto>>(items)
            };
        }

        public virtual async Task<ProxyDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Proxy, ProxyDto>(await _proxyRepository.GetAsync(id));
        }

        [Authorize(ApiPermissions.Proxies.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _proxyRepository.DeleteAsync(id);
        }

        [Authorize(ApiPermissions.Proxies.Create)]
        public virtual async Task<ProxyDto> CreateAsync(ProxyCreateDto input)
        {

            var proxy = ObjectMapper.Map<ProxyCreateDto, Proxy>(input);
            proxy.TenantId = CurrentTenant.Id;
            proxy = await _proxyRepository.InsertAsync(proxy, autoSave: true);
            return ObjectMapper.Map<Proxy, ProxyDto>(proxy);
        }

        [Authorize(ApiPermissions.Proxies.Edit)]
        public virtual async Task<ProxyDto> UpdateAsync(Guid id, ProxyUpdateDto input)
        {

            var proxy = await _proxyRepository.GetAsync(id);
            ObjectMapper.Map(input, proxy);
            proxy = await _proxyRepository.UpdateAsync(proxy);
            return ObjectMapper.Map<Proxy, ProxyDto>(proxy);
        }
    }
}