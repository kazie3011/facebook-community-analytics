using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace FacebookCommunityAnalytics.Api.Partners
{
    public class MongoPartnerRepository : MongoDbRepository<ApiMongoDbContext, Partner, Guid>, IPartnerRepository
    {
        public MongoPartnerRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<Partner>> GetListAsync(
            string filterText = null,
            string name = null,
            string description = null,
            string url = null,
            string code = null,
            PartnerType? partnerType = null,
            bool? isActive = null,
            Guid? partnerUserId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                name,
                description,
                url,
                code,
                partnerType,
                isActive,
                partnerUserId
            );
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PartnerConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Partner>>()
                .PageBy<Partner, IMongoQueryable<Partner>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string name = null,
            string description = null,
            string url = null,
            string code = null,
            PartnerType? partnerType = null,
            bool? isActive = null,
            Guid? partnerUserId = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                name,
                description,
                url,
                code,
                partnerType,
                isActive,
                partnerUserId
            );
            return await query.As<IMongoQueryable<Partner>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Partner> ApplyFilter(
            IQueryable<Partner> query,
            string filterText,
            string name = null,
            string description = null,
            string url = null,
            string code = null,
            PartnerType? partnerType = null,
            bool? isActive = null,
            Guid? partnerUserId = null)
        {
            return query
                .WhereIf
                (
                    !string.IsNullOrWhiteSpace(filterText),
                    e => e.Name.ToLower().Contains(filterText.ToLower())
                         || e.Description.ToLower().Contains(filterText.ToLower())
                         || e.Url.ToLower().Contains(filterText.ToLower())
                         || e.Code.ToLower().Contains(filterText.ToLower())
                )
                .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                .WhereIf(!string.IsNullOrWhiteSpace(url), e => e.Url.Contains(url))
                .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code))
                .WhereIf(isActive.HasValue, e => e.IsActive == isActive)
                .WhereIf(partnerUserId.HasValue, e=> e.PartnerUserIds != null && e.PartnerUserIds.Contains(partnerUserId.Value))
                .WhereIf(partnerType.HasValue, e => e.PartnerType == partnerType);
        }
    }
}