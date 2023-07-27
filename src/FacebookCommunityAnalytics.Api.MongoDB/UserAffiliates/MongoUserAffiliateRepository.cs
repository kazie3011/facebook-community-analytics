using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.MongoDB;

namespace FacebookCommunityAnalytics.Api.UserAffiliates
{
    public class MongoUserAffiliateRepository : MongoDbRepositoryBase<ApiMongoDbContext, UserAffiliate, Guid>, IUserAffiliateRepository
    {
        public MongoUserAffiliateRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            MarketplaceType? marketplaceType = null,
            AffiliateProviderType? affiliateProviderType = null,
            string url = null,
            string affiliateUrl = null,
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            Guid? groupId = null,
            Guid? partnerId = null,
            Guid? campaignId = null,
            Guid? appUserId = null,
            IEnumerable<Guid> appUserIds = null,
            bool? hasConversion = null,
            IEnumerable<string> shortLinks = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                marketplaceType,
                affiliateProviderType,
                url,
                affiliateUrl,
                createdAtMin,
                createdAtMax,
                groupId,
                partnerId,
                campaignId,
                appUserId,
                appUserIds,
                hasConversion,
                shortLinks
            );
            return await query.As<IMongoQueryable<UserAffiliate>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<UserAffiliateWithNavigationProperties> GetUserAffiliateWithNavigationProperties(Guid id, CancellationToken cancellationToken = default)
        {
            var userAffiliate = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));
            
            var appUser = await (await GetDbContextAsync(cancellationToken)).Users.AsQueryable().FirstOrDefaultAsync(e => e.Id == userAffiliate.AppUserId, cancellationToken: cancellationToken);
            var campaign = await (await GetDbContextAsync(cancellationToken)).Campaigns.AsQueryable().FirstOrDefaultAsync(e => e.Id == userAffiliate.CampaignId, cancellationToken: cancellationToken);
            var partner = await (await GetDbContextAsync(cancellationToken)).Partners.AsQueryable().FirstOrDefaultAsync(e => e.Id == userAffiliate.PartnerId, cancellationToken: cancellationToken);

            return new UserAffiliateWithNavigationProperties()
            {
                UserAffiliate = userAffiliate,
                AppUser = appUser,
                Campaign = campaign,
                Partner = partner,
            };
        }

        public async Task<List<UserAffiliateWithNavigationProperties>> GetUserAffiliateWithNavigationProperties(
            string filterText = null,
            MarketplaceType? marketplaceType = null,
            AffiliateProviderType? affiliateProviderType = null,
            string url = null,
            string affiliateUrl = null,
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            Guid? groupId = null,
            Guid? partnerId = null,
            Guid? campaignId = null,
            Guid? appUserId = null,
            IEnumerable<Guid> appUserIds = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            bool? hasConversion = null,
            IEnumerable<string> shortLinks = null,
            CancellationToken cancellationToken = default)
        {
            if (sorting.IsNotNullOrEmpty())
            {
                var sortExtend = sorting.Split('.');
                if (sorting.Split('.').Length > 2) { sorting = $"{sortExtend[sortExtend.Length - 2]}.{sortExtend.Last()}"; }
                else { sorting = sortExtend.Last(); }
            }

            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText: filterText,
                marketplaceType: marketplaceType,
                affiliateProviderType: affiliateProviderType,
                url: url,
                affiliateUrl: affiliateUrl,
                createdAtMin: createdAtMin,
                createdAtMax: createdAtMax,
                groupId: groupId,
                partnerId: partnerId,
                campaignId: campaignId,
                appUserId: appUserId,
                appUserIds: appUserIds,
                hasConversion: hasConversion,
                shortLinks: shortLinks
            );
            var userAffiliates = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserAffiliateConsts.GetDefaultSorting(false) : sorting) //Handle sorting before call repo
                .As<IMongoQueryable<UserAffiliate>>()
                .PageBy<UserAffiliate, IMongoQueryable<UserAffiliate>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
            var dbContext = await GetDbContextAsync(cancellationToken);

            return userAffiliates.Select
                (
                    s => new UserAffiliateWithNavigationProperties
                    {
                        UserAffiliate = s,
                        AppUser = dbContext.Users.AsQueryable().FirstOrDefault(e => e.Id == s.AppUserId && !e.IsDeleted),
                        UserInfo = dbContext.UserInfos.AsQueryable().FirstOrDefault(e => e.AppUserId == s.AppUserId && !e.IsDeleted),
                        Group = dbContext.Groups.AsQueryable().FirstOrDefault(e => e.Id == s.GroupId && !e.IsDeleted),
                        Partner = dbContext.Partners.AsQueryable().FirstOrDefault(e => e.Id == s.PartnerId && !e.IsDeleted),
                        Campaign = dbContext.Campaigns.AsQueryable().FirstOrDefault(e => e.Id == s.CampaignId && !e.IsDeleted),
                    }
                )
                .ToList();
        }

        public async Task<List<UserAffiliate>> GetListAsync(
            string filterText = null,
            MarketplaceType? marketplaceType = null,
            string url = null,
            string affiliateUrl = null,
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            Guid? groupId = null,
            Guid? partnerId = null,
            Guid? campaignId = null,
            Guid? appUserId = null,
            IEnumerable<Guid> appUserIds = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            bool? hasConversion = null,
            IEnumerable<string> shortLinks = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                marketplaceType,
                url: url,
                affiliateUrl: affiliateUrl,
                createdAtMin: createdAtMin,
                createdAtMax: createdAtMax,
                groupId: groupId,
                partnerId: partnerId,
                campaignId: campaignId,
                appUserId: appUserId,
                appUserIds: appUserIds,
                hasConversion: hasConversion,
                shortLinks: shortLinks
            );
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserAffiliateConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<UserAffiliate>>()
                .PageBy<UserAffiliate, IMongoQueryable<UserAffiliate>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<UserAffiliate> ApplyFilter(
            IQueryable<UserAffiliate> query,
            string filterText,
            MarketplaceType? marketplaceType = null,
            AffiliateProviderType? affiliateProviderType = null,
            string url = null,
            string affiliateUrl = null,
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            Guid? groupId = null,
            Guid? partnerId = null,
            Guid? campaignId = null,
            Guid? appUserId = null,
            IEnumerable<Guid> appUserIds = null,
            bool? hasConversion = null,
            IEnumerable<string> shortLinks = null,
            CancellationToken cancellationToken = default)
        {
            return query
                .WhereIf
                (
                    !string.IsNullOrWhiteSpace(filterText),
                    e => e.Url.ToLower().Contains(filterText.ToLower())
                         || e.AffiliateUrl.ToLower().Contains(filterText.ToLower())
                )
                .WhereIf(marketplaceType.HasValue, e => e.MarketplaceType == marketplaceType)
                .WhereIf(affiliateProviderType.HasValue, e => e.AffiliateProviderType == affiliateProviderType)
                .WhereIf(!url.IsNullOrWhiteSpace(), e => e.Url.ToLower() == url.ToLower())
                .WhereIf(!affiliateUrl.IsNullOrWhiteSpace(), e => e.AffiliateUrl.ToLower() == affiliateUrl.ToLower())
                .WhereIf(createdAtMax.HasValue, e => e.CreatedAt <= createdAtMax)
                .WhereIf(createdAtMin.HasValue, e => e.CreatedAt >= createdAtMin)
                .WhereIf(groupId != null, e => e.GroupId == groupId)
                .WhereIf(partnerId != null, e => e.PartnerId == partnerId)
                .WhereIf(campaignId != null, e => e.CampaignId == campaignId)
                .WhereIf(appUserId != null, e => e.AppUserId == appUserId)
                .WhereIf(appUserIds != null, e => appUserIds.Contains((Guid) e.AppUserId))
                .WhereIf(shortLinks != null, e => shortLinks.Contains(e.AffiliateUrl))
                .WhereIf
                (
                    hasConversion != null,
                    e => e.AffConversionModel != null
                         && ((hasConversion.Value && e.AffConversionModel.ConversionCount > 0) || (!hasConversion.Value && e.AffConversionModel.ConversionCount <= 0))
                );
        }

        public async Task<List<UserAffiliate>> Get(List<string> shortUrls)
        {
            var query = await GetMongoQueryableAsync();
            var affiliates = query.WhereIf
                (
                    shortUrls.IsNotNullOrEmpty(),
                    _ => !_.IsDeleted
                         && shortUrls.Contains(_.AffiliateUrl)
                )
                .ToList();

            return affiliates;
        }
    }
}