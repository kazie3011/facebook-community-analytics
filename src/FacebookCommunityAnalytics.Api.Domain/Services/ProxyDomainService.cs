using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Proxies;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api.Services
{
    public interface IProxyDomainService : IDomainService
    {
        Task<List<Proxy>> GetAlives(bool deactiveDeadProxies = true);
    }
    public class ProxyDomainService : DomainService, IProxyDomainService
    {
        private readonly IProxyRepository _proxyRepository;

        public ProxyDomainService(IProxyRepository proxyRepository)
        {
            _proxyRepository = proxyRepository;
        }

        public async Task<List<Proxy>> GetAlives(bool deactiveDeadProxies = true)
        {
            var proxies = await _proxyRepository.GetListAsync(isActive:true);

            var alives = new List<Proxy>();
            foreach (var proxy in proxies)
            {
                var pingSucceeded = await Ping(proxy.Ip);
                proxy.LastPingDateTime = DateTime.UtcNow;

                if (pingSucceeded)
                {
                    proxy.IsActive = true;
                    alives.Add(proxy);
                }
                else
                {
                    if (deactiveDeadProxies)
                    {
                        proxy.IsActive = false;
                    }
                }

                await _proxyRepository.UpdateAsync(proxy);
            }

            return alives;
        }
        
        private async Task<bool> Ping(string ip)
        {
            var pingSender = new Ping();
            var isValidIp = IPAddress.TryParse(ip.Trim(), out var ipAddress);

            if (isValidIp)
            {
                var rely = await pingSender.SendPingAsync(ipAddress);
                return rely.Status == IPStatus.Success;
            }

            return false;
        }
    }
}