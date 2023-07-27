using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Proxies
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.Proxies.Default)]
    public class ProxyExtendAppService : ProxiesAppService, IProxyExtendAppService
    {
        private readonly IProxyRepository _proxyRepository;
        private readonly IProxyDomainService _proxyDomainService;

        public ProxyExtendAppService(IProxyRepository proxyRepository, IProxyDomainService proxyDomainService):base(proxyRepository)
        {
            _proxyRepository = proxyRepository;
            _proxyDomainService = proxyDomainService;
        }

        public virtual async Task ImportProxiesAsync(ProxyImportInput proxyImportInput)
        {
            foreach (var item in proxyImportInput.Items)
            {
                var count = await _proxyRepository.GetCountAsync(ip: item.Ip.Trim());
                if (count <= 0)
                {
                    var proxy = ObjectMapper.Map<ProxyImportDto, Proxy>(item);
                    proxy.IsActive = true;
                    proxy.TenantId = CurrentTenant.Id;
                    await _proxyRepository.InsertAsync(proxy);
                }
            }
        }

        public async Task<List<ProxyDto>> GetAlives(bool deactiveDeadProxies = true)
        {
            var alives = await _proxyDomainService.GetAlives(deactiveDeadProxies);

            return ObjectMapper.Map<List<Proxy>, List<ProxyDto>>(alives);
        }

        public async Task Ping(bool deactiveDeadProxies = true)
        {
            await GetAlives(deactiveDeadProxies);
        }
    }
}