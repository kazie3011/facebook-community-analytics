using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.AccountProxies
{
    public interface IAccountProxiesExtendAppService : IAccountProxiesAppService
    {
        Task RebindAccountProxies();
        Task<AccountProxyWithNavigationPropertiesDto> GetForTool();
    }
}