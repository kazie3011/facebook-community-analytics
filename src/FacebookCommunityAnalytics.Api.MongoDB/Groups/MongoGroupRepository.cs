using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace FacebookCommunityAnalytics.Api.Groups
{
    public class MongoGroupRepository : MongoDbRepository<ApiMongoDbContext, Group, Guid>, IGroupRepository
    {
        public MongoGroupRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<Group>> GetListAsync(
            string filterText = null,
            string title = null,
            string fid = null,
            string name = null,
            string altName = null,
            string url = null,
            bool? isActive = null,
            GroupSourceType? groupSourceType = null,
            GroupOwnershipType? groupOwnershipType = null,
            GroupCategoryType? groupCategoryType = null,
            TikTokContractStatus? contractStatus = null,
            List<Guid> MCNHashIds = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                title,
                fid,
                name,
                altName,
                url,
                isActive,
                groupSourceType,
                groupOwnershipType,
                groupCategoryType,
                contractStatus,
                MCNHashIds
            );
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? GroupConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Group>>()
                .PageBy<Group, IMongoQueryable<Group>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string title = null,
            string fid = null,
            string name = null,
            string altName = null,
            string url = null,
            bool? isActive = null,
            GroupSourceType? groupSourceType = null,
            GroupOwnershipType? groupOwnershipType = null,
            GroupCategoryType? groupCategoryType = null,
            TikTokContractStatus? contractStatus = null,
            List<Guid> MCNHashIds = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                title,
                fid,
                name,
                altName,
                url,
                isActive,
                groupSourceType,
                groupOwnershipType,
                groupCategoryType,
                contractStatus,
                MCNHashIds
            );
            return await query.As<IMongoQueryable<Group>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Group> ApplyFilter(
            IQueryable<Group> query,
            string filterText,
            string title = null,
            string fid = null,
            string name = null,
            string altName = null,
            string url = null,
            bool? isActive = null,
            GroupSourceType? groupSourceType = null,
            GroupOwnershipType? groupOwnershipType = null,
            GroupCategoryType? groupCategoryType = null,
            TikTokContractStatus? contractStatus = null,
            List<Guid> MCNHashIds = null
        )
        {
            return query
                .WhereIf
                (
                    !string.IsNullOrWhiteSpace(filterText),
                    e => e.Title.ToLower().Contains(filterText.ToLower())
                         || e.Fid.ToLower().Contains(filterText.ToLower())
                         || e.Name.ToLower().Contains(filterText.ToLower())
                         || e.AltName.ToLower()
                             .Contains(filterText.ToLower())
                         || e.Url.ToLower().Contains(filterText.ToLower())
                )
                .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Title.Contains(title))
                .WhereIf(!string.IsNullOrWhiteSpace(fid), e => e.Fid.Contains(fid))
                .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                .WhereIf(!string.IsNullOrWhiteSpace(altName), e => e.AltName.Contains(altName))
                .WhereIf(!string.IsNullOrWhiteSpace(url), e => e.Url.Contains(url))
                .WhereIf(isActive.HasValue, e => e.IsActive == isActive)
                .WhereIf(groupSourceType.HasValue, e => e.GroupSourceType == groupSourceType)
                .WhereIf(groupOwnershipType.HasValue, e => e.GroupOwnershipType == groupOwnershipType)
                .WhereIf(groupCategoryType.HasValue, e => e.GroupCategoryType == groupCategoryType)
                .WhereIf(contractStatus.HasValue, e => e.ContractStatus == contractStatus)
                .WhereIf(MCNHashIds != null, x => x.McnId.HasValue && MCNHashIds.Contains(x.McnId.Value));
        }
    }
}