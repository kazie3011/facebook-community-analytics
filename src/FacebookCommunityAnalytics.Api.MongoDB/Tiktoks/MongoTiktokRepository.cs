using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.MongoDB;
using FacebookCommunityAnalytics.Api.Posts;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.MongoDB;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public partial class MongoTiktokRepository : MongoDbRepositoryBase<ApiMongoDbContext, Tiktok, Guid>, ITiktokRepository
    {
        public MongoTiktokRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<TiktokWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var tiktok = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var group = await (await GetDbContextAsync(cancellationToken)).Groups.AsQueryable().FirstOrDefaultAsync(e => e.Fid == tiktok.ChannelId, cancellationToken: cancellationToken);
            var appUser = await (await GetDbContextAsync(cancellationToken)).Users.AsQueryable().FirstOrDefaultAsync(e => e.Id == tiktok.AppUserId, cancellationToken: cancellationToken);
            var campaign = await (await GetDbContextAsync(cancellationToken)).Campaigns.AsQueryable().FirstOrDefaultAsync(e => e.Id == tiktok.CampaignId, cancellationToken: cancellationToken);
            var partner = await (await GetDbContextAsync(cancellationToken)).Partners.AsQueryable().FirstOrDefaultAsync(e => e.Id == tiktok.PartnerId, cancellationToken: cancellationToken);

            return new TiktokWithNavigationProperties
            {
                Tiktok = tiktok,
                Group = group,
                AppUser = appUser,
                Campaign = campaign,
                Partner = partner
            };
        }

        public async Task<List<TiktokWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            PostCopyrightType? postCopyrightType = null,
            string url = null,
            string shortUrl = null,
            int? likeCountMin = null,
            int? likeCountMax = null,
            int? commentCountMin = null,
            int? commentCountMax = null,
            int? shareCountMin = null,
            int? shareCountMax = null,
            int? totalCountMin = null,
            int? totalCountMax = null,
            string hashtag = null,
            string fid = null,
            bool? isNotAvailable = null,
            PostStatus? status = null,
            string note = null,
            DateTime? createdDateTimeMin = null,
            DateTime? createdDateTimeMax = null,
            DateTime? lastCrawledDateTimeMin = null,
            DateTime? lastCrawledDateTimeMax = null,
            DateTime? submissionDateTimeMin = null,
            DateTime? submissionDateTimeMax = null,
            Guid? categoryId = null,
            string channelId = null,
            Guid? appUserId = null,
            Guid? campaignId = null,
            Guid? partnerId = null,
            bool? isNew = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                postCopyrightType,
                url,
                shortUrl,
                likeCountMin,
                likeCountMax,
                commentCountMin,
                commentCountMax,
                shareCountMin,
                shareCountMax,
                totalCountMin,
                totalCountMax,
                hashtag,
                fid,
                isNotAvailable,
                status,
                note,
                createdDateTimeMin,
                createdDateTimeMax,
                lastCrawledDateTimeMin,
                lastCrawledDateTimeMax,
                submissionDateTimeMin,
                submissionDateTimeMax,
                categoryId,
                channelId,
                appUserId,
                campaignId,
                partnerId,
                isNew
            );
            var posts = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? TiktokConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<Tiktok>>()
                .PageBy<Tiktok, IMongoQueryable<Tiktok>>(skipCount, maxResultCount)
                .ToListAsync
                (
                    GetCancellationToken(cancellationToken)
                );

            var dbContext = await GetDbContextAsync(cancellationToken);
            return posts.Select
                (
                    s => new TiktokWithNavigationProperties
                    {
                        Tiktok = s,
                        Group = dbContext.Groups.AsQueryable().FirstOrDefault(e => e.Fid == s.ChannelId && !e.IsDeleted),
                        AppUser = dbContext.Users.AsQueryable().FirstOrDefault(e => e.Id == s.AppUserId && !e.IsDeleted),
                        AppUserInfo = dbContext.UserInfos.AsQueryable().FirstOrDefault(e => e.AppUserId == s.AppUserId && !e.IsDeleted),
                        Campaign = dbContext.Campaigns.AsQueryable().FirstOrDefault(e => e.Id == s.CampaignId && !e.IsDeleted),
                        Partner = dbContext.Partners.AsQueryable().FirstOrDefault(e => e.Id == s.PartnerId && !e.IsDeleted)
                    }
                )
                .ToList();
        }

        //Todoo Long: GetListAsync Can't use
        public async Task<List<Tiktok>> GetListAsync(
            string filterText = null,
            PostCopyrightType? postCopyrightType = null,
            string url = null,
            string shortUrl = null,
            int? likeCountMin = null,
            int? likeCountMax = null,
            int? commentCountMin = null,
            int? commentCountMax = null,
            int? shareCountMin = null,
            int? shareCountMax = null,
            int? totalCountMin = null,
            int? totalCountMax = null,
            string hashtag = null,
            string fid = null,
            bool? isNotAvailable = null,
            PostStatus? status = null,
            string note = null,
            DateTime? createdDateTimeMin = null,
            DateTime? createdDateTimeMax = null,
            DateTime? lastCrawledDateTimeMin = null,
            DateTime? lastCrawledDateTimeMax = null,
            DateTime? submissionDateTimeMin = null,
            DateTime? submissionDateTimeMax = null,
            // Guid? categoryId = null,
            // Guid? groupId = null,
            // Guid? appUserId = null,
            // Guid? campaignId = null,
            // Guid? partnerId = null,
            bool? isNew = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                postCopyrightType,
                url,
                shortUrl,
                likeCountMin,
                likeCountMax,
                commentCountMin,
                commentCountMax,
                shareCountMin,
                shareCountMax,
                totalCountMin,
                totalCountMax,
                hashtag,
                fid,
                isNotAvailable,
                status,
                note,
                createdDateTimeMin,
                createdDateTimeMax,
                lastCrawledDateTimeMin,
                lastCrawledDateTimeMax,
                submissionDateTimeMin,
                submissionDateTimeMax,
                isNew: isNew
            );
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? TiktokConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Tiktok>>()
                .PageBy<Tiktok, IMongoQueryable<Tiktok>>(skipCount, maxResultCount)
                .ToListAsync
                (
                    GetCancellationToken(cancellationToken)
                );
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            PostCopyrightType? postCopyrightType = null,
            string url = null,
            string shortUrl = null,
            int? likeCountMin = null,
            int? likeCountMax = null,
            int? commentCountMin = null,
            int? commentCountMax = null,
            int? shareCountMin = null,
            int? shareCountMax = null,
            int? totalCountMin = null,
            int? totalCountMax = null,
            string hashtag = null,
            string fid = null,
            bool? isNotAvailable = null,
            PostStatus? status = null,
            string note = null,
            DateTime? createdDateTimeMin = null,
            DateTime? createdDateTimeMax = null,
            DateTime? lastCrawledDateTimeMin = null,
            DateTime? lastCrawledDateTimeMax = null,
            DateTime? submissionDateTimeMin = null,
            DateTime? submissionDateTimeMax = null,
            Guid? categoryId = null,
            string channelId = null,
            Guid? appUserId = null,
            Guid? campaignId = null,
            Guid? partnerId = null,
            bool? isNew = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                postCopyrightType,
                url,
                shortUrl,
                likeCountMin,
                likeCountMax,
                commentCountMin,
                commentCountMax,
                shareCountMin,
                shareCountMax,
                totalCountMin,
                totalCountMax,
                hashtag,
                fid,
                isNotAvailable,
                status,
                note,
                createdDateTimeMin,
                createdDateTimeMax,
                lastCrawledDateTimeMin,
                lastCrawledDateTimeMax,
                submissionDateTimeMin,
                submissionDateTimeMax,
                categoryId,
                channelId,
                appUserId,
                campaignId,
                partnerId,
                isNew
            );
            return await query.As<IMongoQueryable<Tiktok>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<List<TiktokWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string tiktokId = null,
            DateTime? createdDateTimeMin = null,
            DateTime? createdDateTimeMax = null,
            bool? sendEmail = null,
            List<string> channelIds = null,
            List<Guid> groupIds = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = (await GetMongoQueryableAsync(cancellationToken));

            query = ApplyFilterNav(query, tiktokId, createdDateTimeMin, createdDateTimeMax,sendEmail, channelIds, groupIds)
                .OrderBy(string.IsNullOrWhiteSpace(sorting) ? TiktokConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<Tiktok>>()
                .PageBy<Tiktok, IMongoQueryable<Tiktok>>(skipCount, maxResultCount);

            var posts = await query.ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return posts.Select
                (
                    s => new TiktokWithNavigationProperties
                    {
                        Tiktok = s,
                        Group = dbContext.Groups.AsQueryable().FirstOrDefault(e => e.Fid == s.ChannelId && !e.IsDeleted),
                        AppUser = dbContext.Users.AsQueryable().FirstOrDefault(e => e.Id == s.AppUserId && !e.IsDeleted),
                        AppUserInfo = dbContext.UserInfos.AsQueryable().FirstOrDefault(e => e.AppUserId == s.AppUserId && !e.IsDeleted),
                        Campaign = dbContext.Campaigns.AsQueryable().FirstOrDefault(e => e.Id == s.CampaignId && !e.IsDeleted),
                        Partner = dbContext.Partners.AsQueryable().FirstOrDefault(e => e.Id == s.PartnerId && !e.IsDeleted)
                    }
                )
                .ToList();
        }

        public async Task<long> GetCountAsync(
            string tiktokId = null,
            DateTime? createdDateTimeMin = null,
            DateTime? createdDateTimeMax = null,
            bool? sendEmail = null,
            List<string> channelIds = null,
            List<Guid> groupIds = null,
            CancellationToken cancellationToken = default)
        {
            var query = (await GetMongoQueryableAsync(cancellationToken));

            
            query = ApplyFilterNav(query, tiktokId, createdDateTimeMin, createdDateTimeMax,sendEmail, channelIds, groupIds)
                .As<IMongoQueryable<Tiktok>>();

            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        private IQueryable<Tiktok> ApplyFilterNav(IQueryable<Tiktok> query,
            string filterText = null,
            DateTime? createdDateTimeMin = null,
            DateTime? createdDateTimeMax = null,
            bool? sendEmail = null,
            List<string> channelIds = null,
            List<Guid> groupIds = null)
        {
            return query.WhereIf(filterText.IsNotNullOrWhiteSpace(), tiktok => tiktok.Url.Contains(filterText) 
                                                                               || tiktok.VideoId.Contains(filterText) 
                                                                               || tiktok.ShortUrl.Contains(filterText))
                .WhereIf(createdDateTimeMin.HasValue, tiktok => tiktok.CreatedDateTime >= createdDateTimeMin.Value.ToUniversalTime())
                .WhereIf(createdDateTimeMax.HasValue, tiktok => tiktok.CreatedDateTime < createdDateTimeMax.Value.ToUniversalTime())
                .WhereIf(channelIds.IsNotNullOrEmpty(), tiktok => channelIds.Contains(tiktok.ChannelId))
                .WhereIf(groupIds.IsNotNullOrEmpty(), tiktok => groupIds.Contains(tiktok.GroupId.Value))
                .WhereIf(sendEmail == true, tiktok => tiktok.IsNew);
        }


        protected virtual IQueryable<Tiktok> ApplyFilter(
            IQueryable<Tiktok> query,
            string filterText,
            PostCopyrightType? postCopyrightType = null,
            string url = null,
            string shortUrl = null,
            int? likeCountMin = null,
            int? likeCountMax = null,
            int? commentCountMin = null,
            int? commentCountMax = null,
            int? shareCountMin = null,
            int? shareCountMax = null,
            int? totalCountMin = null,
            int? totalCountMax = null,
            string hashtag = null,
            string fid = null,
            bool? isNotAvailable = null,
            PostStatus? status = null,
            string note = null,
            DateTime? createdDateTimeMin = null,
            DateTime? createdDateTimeMax = null,
            DateTime? lastCrawledDateTimeMin = null,
            DateTime? lastCrawledDateTimeMax = null,
            DateTime? submissionDateTimeMin = null,
            DateTime? submissionDateTimeMax = null,
            Guid? categoryId = null,
            string channelId = null,
            Guid? appUserId = null,
            Guid? campaignId = null,
            Guid? partnerId = null,
            bool? isNew = null)
        {
            return query
                .WhereIf
                (
                    !string.IsNullOrWhiteSpace(filterText),
                    e => e.Url.Contains(filterText) || e.ShortUrl.Contains(filterText) || e.Hashtag.Contains(filterText) || e.VideoId.Contains(filterText)
                )
                .WhereIf(!string.IsNullOrWhiteSpace(url), e => e.Url.Contains(url))
                .WhereIf(!string.IsNullOrWhiteSpace(shortUrl), e => e.ShortUrl.Contains(shortUrl))
                .WhereIf(likeCountMin.HasValue, e => e.LikeCount >= likeCountMin.Value)
                .WhereIf(likeCountMax.HasValue, e => e.LikeCount <= likeCountMax.Value)
                .WhereIf(commentCountMin.HasValue, e => e.CommentCount >= commentCountMin.Value)
                .WhereIf(commentCountMax.HasValue, e => e.CommentCount <= commentCountMax.Value)
                .WhereIf(shareCountMin.HasValue, e => e.ShareCount >= shareCountMin.Value)
                .WhereIf(shareCountMax.HasValue, e => e.ShareCount <= shareCountMax.Value)
                .WhereIf(totalCountMin.HasValue, e => e.TotalCount >= totalCountMin.Value)
                .WhereIf(totalCountMax.HasValue, e => e.TotalCount <= totalCountMax.Value)
                .WhereIf(!string.IsNullOrWhiteSpace(hashtag), e => e.Hashtag.Contains(hashtag))
                .WhereIf(!string.IsNullOrWhiteSpace(fid), e => e.VideoId.Contains(fid))
                .WhereIf(createdDateTimeMin.HasValue, e => e.CreatedDateTime >= createdDateTimeMin.Value)
                .WhereIf(createdDateTimeMax.HasValue, e => e.CreatedDateTime <= createdDateTimeMax.Value)
                .WhereIf(lastCrawledDateTimeMin.HasValue, e => e.LastCrawledDateTime >= lastCrawledDateTimeMin.Value)
                .WhereIf(lastCrawledDateTimeMax.HasValue, e => e.LastCrawledDateTime <= lastCrawledDateTimeMax.Value)
                .WhereIf(channelId.IsNullOrWhiteSpace(), e => e.ChannelId == channelId)
                .WhereIf(appUserId != null && appUserId != Guid.Empty, e => e.AppUserId == appUserId)
                .WhereIf(campaignId != null && campaignId != Guid.Empty, e => e.CampaignId == campaignId)
                .WhereIf(partnerId != null && partnerId != Guid.Empty, e => e.PartnerId == partnerId)
                .WhereIf(isNew.HasValue, x => x.IsNew == isNew);
        }
    }
}