using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Affiliates
{
    public interface IAffiliateRoutingAppService : IApplicationService
    {
        Task<string> GetMappingUrl(string key);
    }
}