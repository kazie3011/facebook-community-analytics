using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace FacebookCommunityAnalytics.Api.AffiliateConversions
{
    public class MongoAffiliateConversionRepository : MongoDbRepository<ApiMongoDbContext, AffiliateConversion, Guid>, IAffiliateConversionRepository
    {
        public MongoAffiliateConversionRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
            
        }


        public async Task<List<AffiliateConversion>> GetListExtendAsync(
            long? dateTimeMin = null,
            long? dateTimeMax = null,
            IEnumerable<string> shortKeys = null,
            CancellationToken cancellationToken = default
        )
        {
            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                dateTimeMin,
                dateTimeMax,
                shortKeys,
                cancellationToken
            );
            return await query.As<IMongoQueryable<AffiliateConversion>>()
                .ToListAsync(GetCancellationToken(cancellationToken));
        }
        protected virtual IQueryable<AffiliateConversion> ApplyFilter(
            IQueryable<AffiliateConversion> query,
            long? dateTimeMin = null,
            long? dateTimeMax = null,
            IEnumerable<string> shortKeys = null,
            CancellationToken cancellationToken = default)
        {
            return query
                .WhereIf(dateTimeMin != 0 && dateTimeMin != null, e => e.ConversionTime >= dateTimeMin)
                .WhereIf(dateTimeMax != 0 && dateTimeMax is not null, e => e.ConversionTime <= dateTimeMax)
                .WhereIf(shortKeys != null, e => shortKeys.Contains(e.ShortKey));
        }
    }
}