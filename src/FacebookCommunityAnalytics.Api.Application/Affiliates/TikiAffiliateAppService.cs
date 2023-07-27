using System;
using FacebookCommunityAnalytics.Api.Integrations.Tiki;
using FacebookCommunityAnalytics.Api.Integrations.Tiki.TikiAffiliates;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Affiliates
{
    [RemoteService(IsEnabled = true)]
    [Authorize]
    public class TikiAffiliateAppService : BaseCrudApiAppService<TikiAffiliate, TikiAffiliateDto, Guid, GetTikiAffiliateInput, TikiAffiliateCreateUpdateDto>, ITikiAffiliateAppService
    {
        public TikiAffiliateAppService(IRepository<TikiAffiliate, Guid> repository) : base(repository)
        {
        }
    }
}