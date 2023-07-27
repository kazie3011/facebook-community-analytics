using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.AffiliateStats
{
    public interface IAffiliateStatRepository : IRepository<AffiliateStat, Guid>
    {
        Task<List<AffiliateStat>> GetListAsync(
            string filterText = null,
            AffiliateOwnershipType? affiliateOwnershipType = null,
            int? clickMin = null,
            int? clickMax = null,
            int? conversionMin = null,
            int? conversionMax = null,
            decimal? amountMin = null,
            decimal? amountMax = null,
            decimal? commissionMin = null,
            decimal? commissionMax = null,
            decimal? commisionBonusMin = null,
            decimal? commisionBonusMax = null,
            int? clientOffsetInMinutes = null,
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            AffiliateOwnershipType? affiliateOwnershipType = null,
            int? clickMin = null,
            int? clickMax = null,
            int? conversionMin = null,
            int? conversionMax = null,
            decimal? amountMin = null,
            decimal? amountMax = null,
            decimal? commissionMin = null,
            decimal? commissionMax = null,
            decimal? commisionBonusMin = null,
            decimal? commisionBonusMax = null,
            int? clientOffsetInMinutes = null, 
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            CancellationToken cancellationToken = default);
    }
}