using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace FacebookCommunityAnalytics.Api.AffiliateStats
{
    public class MongoAffiliateStatRepository : MongoDbRepository<ApiMongoDbContext, AffiliateStat, Guid>, IAffiliateStatRepository
    {
        public MongoAffiliateStatRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<AffiliateStat>> GetListAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                affiliateOwnershipType,
                clickMin,
                clickMax,
                conversionMin,
                conversionMax,
                amountMin,
                amountMax,
                commissionMin,
                commissionMax,
                commisionBonusMin,
                commisionBonusMax,
                clientOffsetInMinutes,
                createdAtMin,
                createdAtMax
            );
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? AffiliateStatConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<AffiliateStat>>()
                .PageBy<AffiliateStat, IMongoQueryable<AffiliateStat>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                affiliateOwnershipType,
                clickMin,
                clickMax,
                conversionMin,
                conversionMax,
                amountMin,
                amountMax,
                commissionMin,
                commissionMax,
                commisionBonusMin,
                commisionBonusMax,
                clientOffsetInMinutes,
                createdAtMin,
                createdAtMax
            );
            return await query.As<IMongoQueryable<AffiliateStat>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<AffiliateStat> ApplyFilter(
            IQueryable<AffiliateStat> query,
            string filterText,
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
            DateTime? createdAtMax = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
                .WhereIf(affiliateOwnershipType.HasValue, e => e.AffiliateOwnershipType == affiliateOwnershipType)
                .WhereIf(clickMin.HasValue, e => e.Click >= clickMin.Value)
                .WhereIf(clickMax.HasValue, e => e.Click <= clickMax.Value)
                .WhereIf(conversionMin.HasValue, e => e.Conversion >= conversionMin.Value)
                .WhereIf(conversionMax.HasValue, e => e.Conversion <= conversionMax.Value)
                .WhereIf(amountMin.HasValue, e => e.Amount >= amountMin.Value)
                .WhereIf(amountMax.HasValue, e => e.Amount <= amountMax.Value)
                .WhereIf(commissionMin.HasValue, e => e.Commission >= commissionMin.Value)
                .WhereIf(commissionMax.HasValue, e => e.Commission <= commissionMax.Value)
                .WhereIf(commisionBonusMin.HasValue, e => e.CommissionBonus >= commisionBonusMin.Value)
                .WhereIf(commisionBonusMax.HasValue, e => e.CommissionBonus <= commisionBonusMax.Value)
                .WhereIf(createdAtMin.HasValue, e => e.CreatedAt >= createdAtMin)
                .WhereIf(createdAtMax.HasValue, e => e.CreatedAt < createdAtMax);
        }
    }
}