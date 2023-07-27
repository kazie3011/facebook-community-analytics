using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.AffiliateConversions
{
    public interface IAffiliateConversionRepository : IRepository<AffiliateConversion, Guid>
    {
        Task<List<AffiliateConversion>> GetListExtendAsync(
            long? dateTimeMin = null,
            long? dateTimeMax = null,
            IEnumerable<string> shortKeys = null,
            CancellationToken cancellationToken = default
        );
    }
}