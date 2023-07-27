using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Integrations.Tiki.TikiAffiliates;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Affiliates
{
    [AllowAnonymous]
    [RemoteService(IsEnabled = false)]
    public class AffiliateRoutingAppService : ApiAppService, IAffiliateRoutingAppService
    {
        private readonly ITikiAffiliateDomainService _tikiAffiliateDomainService;

        public AffiliateRoutingAppService(ITikiAffiliateDomainService tikiAffiliateDomainService)
        {
            _tikiAffiliateDomainService = tikiAffiliateDomainService;
        }

        public async Task<string> GetMappingUrl(string key)
        {
            var tikiKeyLength = GlobalConfiguration.TikiConfiguration.ShortkeyLength;
            var shortLink = $"{GlobalConsts.BaseAffiliateDomain}/{key.Trim()}";
            
            if (key.Length == tikiKeyLength)
            {
                return await _tikiAffiliateDomainService.MapAffiliateUrl(shortLink);
            }

            return shortLink;
        }
    }
}