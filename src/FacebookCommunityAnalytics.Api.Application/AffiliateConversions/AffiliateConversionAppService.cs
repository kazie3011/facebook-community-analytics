using System;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.AffiliateConversions
{
    [Authorize]
    [RemoteService(IsEnabled = true)]
    public class AffiliateConversionAppService : CrudAppService<AffiliateConversion, AffiliateConversionDto, Guid, GetAffiliateConversionInput, AffiliateConversionCreateUpdateDto>, IAffiliateConversionAppService
    {
        public AffiliateConversionAppService(IRepository<AffiliateConversion, Guid> repository) : base(repository)
        {
            
        }
    }
}