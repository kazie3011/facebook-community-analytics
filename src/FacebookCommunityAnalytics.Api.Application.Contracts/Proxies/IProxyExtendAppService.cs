using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Proxies
{
    public interface IProxyExtendAppService : IApplicationService
    {
        Task ImportProxiesAsync(ProxyImportInput proxyImportInput);
        Task<List<ProxyDto>> GetAlives(bool deactiveDeadProxies = true);
        Task Ping(bool deactiveDeadProxies = true);
    }
}
