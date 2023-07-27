using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.UserAffiliates
{
    public interface IUserAffiliateRepository : IRepository<UserAffiliate, Guid>
    {
        Task<List<UserAffiliateWithNavigationProperties>> GetUserAffiliateWithNavigationProperties(
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
            CancellationToken cancellationToken = default);

        Task<List<UserAffiliate>> GetListAsync(
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
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default
        );

        Task<UserAffiliateWithNavigationProperties> GetUserAffiliateWithNavigationProperties(Guid id,CancellationToken cancellationToken = default);
        Task<List<UserAffiliate>> Get(List<string> shortUrls);
    }
}