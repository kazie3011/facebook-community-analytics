using System;

namespace FacebookCommunityAnalytics.Api.Integrations.Tiki
{
    public interface ITikiAffiliateAppService :
        IBaseApiCrudAppService<TikiAffiliateDto,
            Guid,
            GetTikiAffiliateInput,
            TikiAffiliateCreateUpdateDto>
    {
    }
}