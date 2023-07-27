using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace FacebookCommunityAnalytics.Api.Campaigns
{
    public class MongoCampaignRepository : MongoDbRepository<ApiMongoDbContext, Campaign, Guid>, ICampaignRepository
    {
        public MongoCampaignRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<CampaignWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var campaign = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var partner = await (await GetDbContextAsync(cancellationToken)).Partners.AsQueryable().FirstOrDefaultAsync(e => e.Id == campaign.PartnerId, cancellationToken: cancellationToken);
            //var contracts = await (await GetDbContextAsync(cancellationToken)).Contracts.AsQueryable().Where(e => e.CampaignId == campaign.Id).ToListAsync(cancellationToken: cancellationToken);

            return new CampaignWithNavigationProperties
            {
                Campaign = campaign,
                Partner = partner,
               // Contracts = contracts
            };
        }

        public async Task<List<CampaignWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string name = null,
            string code = null,
            string hashtags = null,
            string description = null,
            CampaignType? campaignType = null,
            CampaignStatus? status = null,
            DateTime? startDateTimeMin = null,
            DateTime? startDateTimeMax = null,
            DateTime? endDateTimeMin = null,
            DateTime? endDateTimeMax = null,
            bool? isActive = null,
            Guid? partnerId = null,
            string currentUserEmail = null,
            List<Guid> partnerIds = null,
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
                code,
                hashtags,
                description,
                campaignType,
                status,
                startDateTimeMin,
                startDateTimeMax,
                endDateTimeMin,
                endDateTimeMax,
                isActive,
                partnerId,
                currentUserEmail,
                partnerIds
            );
            var campaigns = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CampaignConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<Campaign>>()
                .PageBy<Campaign, IMongoQueryable<Campaign>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return campaigns.Select
                (
                    campaign => new CampaignWithNavigationProperties
                    {
                        Campaign = campaign,
                        Partner = dbContext.Partners.AsQueryable().FirstOrDefault(partner => partner.Id == campaign.PartnerId && !partner.IsDeleted),
                        // TotalPostFacebook = dbContext.Posts.AsQueryable().Count(post=>post.CampaignId.HasValue && post.CampaignId == campaign.Id && (post.PostSourceType == PostSourceType.Page || post.PostSourceType == PostSourceType.Group)),
                        // TotalPostTiktok = dbContext.Posts.AsQueryable().Count(e=>e.CampaignId.HasValue && e.CampaignId == campaign.Id && e.PostSourceType == PostSourceType.Video)
                        //Contracts = dbContext.Contracts.AsQueryable().Where(e=>e.CampaignId == s.Id).ToList()
                    }
                )
                .ToList();
        }

        public async Task<List<Campaign>> GetListAsync(
            string filterText = null,
            string name = null,
            string code = null,
            string hashtags = null,
            string description = null,
            CampaignType? campaignType = null,
            CampaignStatus? status = null,
            DateTime? startDateTimeMin = null,
            DateTime? startDateTimeMax = null,
            DateTime? endDateTimeMin = null,
            DateTime? endDateTimeMax = null,
            bool? isActive = null,
            Guid? partnerId = null,
            string currentUserEmail = null,
            List<Guid> partnerIds = null,
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
                code,
                hashtags,
                description,
                campaignType,
                status,
                startDateTimeMin,
                startDateTimeMax,
                endDateTimeMin,
                endDateTimeMax,
                isActive,
                partnerId,
                currentUserEmail,
                partnerIds
            );
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CampaignConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Campaign>>()
                .PageBy<Campaign, IMongoQueryable<Campaign>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string name = null,
            string code = null,
            string hashtags = null,
            string description = null,
            CampaignType? campaignType = null,
            CampaignStatus? status = null,
            DateTime? startDateTimeMin = null,
            DateTime? startDateTimeMax = null,
            DateTime? endDateTimeMin = null,
            DateTime? endDateTimeMax = null,
            bool? isActive = null,
            Guid? partnerId = null,
            string currentUserEmail = null,
            List<Guid> partnerIds = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                name,
                code,
                hashtags,
                description,
                campaignType,
                status,
                startDateTimeMin,
                startDateTimeMax,
                endDateTimeMin,
                endDateTimeMax,
                isActive,
                partnerId,
                currentUserEmail,
                partnerIds
            );
            return await query.As<IMongoQueryable<Campaign>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Campaign> ApplyFilter(
            IQueryable<Campaign> query,
            string filterText,
            string name = null,
            string code = null,
            string hashtags = null,
            string description = null,
            CampaignType? campaignType = null,
            CampaignStatus? status = null,
            DateTime? startDateTimeMin = null,
            DateTime? startDateTimeMax = null,
            DateTime? endDateTimeMin = null,
            DateTime? endDateTimeMax = null,
            bool? isActive = null,
            Guid? partnerId = null,
            string currentUserEmail = null,
            List<Guid> partnerIds = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name.ToLower().Contains(filterText.ToLower()) 
                                                                      || e.Code.ToLower().Contains(filterText.ToLower()) 
                                                                      || e.Hashtags.ToLower().Contains(filterText.ToLower()) 
                                                                      || e.Description.ToLower().Contains(filterText.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code))
                .WhereIf(!string.IsNullOrWhiteSpace(hashtags), e => e.Hashtags.Contains(hashtags))
                .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                .WhereIf(campaignType.HasValue, e => e.CampaignType == campaignType)
                .WhereIf(status.HasValue, e => e.Status == status)
                .WhereIf(startDateTimeMin.HasValue, e => e.StartDateTime >= startDateTimeMin.Value)
                .WhereIf(startDateTimeMax.HasValue, e => e.StartDateTime <= startDateTimeMax.Value)
                .WhereIf(endDateTimeMin.HasValue, e => e.EndDateTime >= endDateTimeMin.Value)
                .WhereIf(endDateTimeMax.HasValue, e => e.EndDateTime <= endDateTimeMax.Value)
                .WhereIf(isActive.HasValue, e => e.IsActive == isActive)
                .WhereIf(partnerId != null && partnerId != Guid.Empty, e => e.PartnerId == partnerId)
                .WhereIf(partnerIds.IsNotNullOrEmpty(), x=> x.PartnerId.HasValue && partnerIds.Contains(x.PartnerId.Value))
                .WhereIf(currentUserEmail.IsNotNullOrEmpty(), x => x.Emails.Contains(currentUserEmail));
        }

        public async Task<List<CampaignWithNavigationProperties>> GetListWithNavigationPropertiesExtendAsync(
            string filterText = null,
            CampaignType? campaignType = null,
            CampaignStatus? status = null,
            DateTime? startDateTimeMin = null,
            DateTime? startDateTimeMax = null,
            DateTime? endDateTimeMin = null,
            DateTime? endDateTimeMax = null,
            bool? isActive = null,
            Guid? partnerId = null,
            string currentUserEmail = null,
            List<Guid> partnerIds = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await ApplyExtendFilter(                
                filterText,
                campaignType,
                status,
                startDateTimeMin,
                startDateTimeMax,
                endDateTimeMin,
                endDateTimeMax,
                isActive,
                partnerId,
                currentUserEmail,
                partnerIds,
                cancellationToken
            );
            var campaignWithNav = query.Skip(skipCount).Take(maxResultCount).ToList();
            return campaignWithNav;
        }

        public async Task<long> GetCountExtendAsync(
            string filterText = null,
            CampaignType? campaignType = null,
            CampaignStatus? status = null,
            DateTime? startDateTimeMin = null,
            DateTime? startDateTimeMax = null,
            DateTime? endDateTimeMin = null,
            DateTime? endDateTimeMax = null,
            bool? isActive = null,
            Guid? partnerId = null,
            string currentUserEmail = null,
            List<Guid> partnerIds = null,
            CancellationToken cancellationToken = default)
        {
            var query = await ApplyExtendFilter(                
                filterText,
                campaignType,
                status,
                startDateTimeMin,
                startDateTimeMax,
                endDateTimeMin,
                endDateTimeMax,
                isActive,
                partnerId,
                currentUserEmail,
                partnerIds,
                cancellationToken
            );
            return query.LongCount();
        }
        
        private async Task<IQueryable<CampaignWithNavigationProperties>> ApplyExtendFilter(
            string filterText,
            CampaignType? campaignType = null,
            CampaignStatus? status = null,
            DateTime? startDateTimeMin = null,
            DateTime? startDateTimeMax = null,
            DateTime? endDateTimeMin = null,
            DateTime? endDateTimeMax = null,
            bool? isActive = null,
            Guid? partnerId = null,
            string currentUserEmail = null,
            List<Guid> partnerIds = null,
            CancellationToken cancellationToken = default)
        {
            var campaigns = await (await GetMongoQueryableAsync(cancellationToken)).ToListAsync(cancellationToken);
            var dbContext = await GetDbContextAsync(cancellationToken);

            var query = from ct in campaigns
                join e in dbContext.Partners.AsQueryable().Where(_ => !_.IsDeleted) on ct.PartnerId equals e.Id into t2
                from pn in t2.DefaultIfEmpty()
                select new CampaignWithNavigationProperties()
                {
                    Campaign = ct,
                    Partner = pn
                };

            if (filterText.IsNotNullOrWhiteSpace())
            {
                filterText = filterText.Trim().ToLower();
            }
            return query.AsQueryable()
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => (e.Campaign.Name.IsNotNullOrEmpty() && e.Campaign.Name.Contains(filterText)) 
                                                                      || (e.Campaign.Code.IsNotNullOrEmpty() && e.Campaign.Code.Contains(filterText)) 
                                                                      || (e.Campaign.Hashtags.IsNotNullOrEmpty() && e.Campaign.Hashtags.Contains(filterText)) 
                                                                      || (e.Campaign.Description.IsNotNullOrEmpty() && e.Campaign.Description.Contains(filterText)))
                .WhereIf(campaignType.HasValue, e => e.Campaign.CampaignType == campaignType)
                .WhereIf(status.HasValue, e => e.Campaign.Status == status)
                .WhereIf(startDateTimeMin.HasValue, e => e.Campaign.StartDateTime != null && e.Campaign.StartDateTime >= startDateTimeMin.Value)
                .WhereIf(startDateTimeMax.HasValue, e => e.Campaign.StartDateTime != null && e.Campaign.StartDateTime <= startDateTimeMax.Value)
                .WhereIf(endDateTimeMin.HasValue, e => e.Campaign.EndDateTime != null &&e.Campaign.EndDateTime >= endDateTimeMin.Value)
                .WhereIf(endDateTimeMax.HasValue, e => e.Campaign.EndDateTime != null &&e.Campaign.EndDateTime <= endDateTimeMax.Value)
                .WhereIf(isActive.HasValue, e => e.Campaign.IsActive == isActive)
                .WhereIf(partnerId != null && partnerId != Guid.Empty, e => e.Campaign.PartnerId.HasValue && e.Campaign.PartnerId == partnerId)
                .WhereIf(partnerIds.IsNotNullOrEmpty(), e=> e.Campaign.PartnerId.HasValue && partnerIds.Contains(e.Campaign.PartnerId.Value))
                .WhereIf(currentUserEmail.IsNotNullOrEmpty(), e => e.Campaign.Emails.Contains(currentUserEmail));
        }
    }
}