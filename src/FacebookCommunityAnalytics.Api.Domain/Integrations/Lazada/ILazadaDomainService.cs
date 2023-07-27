using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Integrations.Lazada.Models;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api.Integrations.Lazada
{
    public interface ILazadaDomainService : IDomainService
    {
        Task<BitlyAffiliateResponse> GenerateAffiliate(string longLink, string code);
    }

    public class LazadaDomainService : BaseDomainService, ILazadaDomainService
    {
        private readonly LazadaConfiguration _lazadaConfiguration;
        private readonly ILazadaApiConsumer _apiConsumer;

        public LazadaDomainService(GlobalConfiguration globalConfiguration, ILazadaApiConsumer apiConsumer)
        {
            _lazadaConfiguration = globalConfiguration.LazadaConfiguration;
            _apiConsumer = apiConsumer;
        }

        public async Task<BitlyAffiliateResponse> GenerateAffiliate(string longLink, string code)
        {
            var link = longLink.StartsWith(_lazadaConfiguration.BaseLink) ?
                       longLink : $"{_lazadaConfiguration.BaseLink}?url={longLink}&sub_id1={code}";

            var resp = await _apiConsumer.GenerateBitlyAffiliate(link);

            return resp;
        }
    }
}
