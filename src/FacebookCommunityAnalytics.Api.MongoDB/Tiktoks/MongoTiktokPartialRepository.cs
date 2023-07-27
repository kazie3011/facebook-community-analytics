using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Posts;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public partial class MongoTiktokRepository
    {
        public async Task<List<TiktokWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            DateTime? fromDate,
            DateTime? toDate,
            List<Guid> staffIds,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText: string.Empty, createdDateTimeMin: fromDate, createdDateTimeMax: toDate);
            query = query.WhereIf(staffIds.IsNotNullOrEmpty(), x => x.AppUserId.HasValue && staffIds.Contains(x.AppUserId.Value));
            var posts = await query.As<IMongoQueryable<Tiktok>>()
                .ToListAsync(cancellationToken);

            var dbContext = await GetDbContextAsync(cancellationToken);
            var results = from p in posts
                join g in dbContext.Groups.AsQueryable().Where(x=>!x.IsDeleted) on p.ChannelId equals g.Fid into pg
                from x2 in pg.DefaultIfEmpty()
                join u in dbContext.Users.AsQueryable().Where(x=>!x.IsDeleted) on p.AppUserId equals u.Id into pu
                from x3 in pu.DefaultIfEmpty()
                join ui in dbContext.UserInfos.AsQueryable().Where(x=>!x.IsDeleted) on p.AppUserId equals ui.AppUserId into pui
                from x4 in pui.DefaultIfEmpty()
                select new TiktokWithNavigationProperties
                {
                    Tiktok = p,
                    Group = x2,
                    AppUser = x3,
                    AppUserInfo = x4
                };
            return results.ToList();
        }

        public async Task<List<Tiktok>> GetAsync(IEnumerable<string> videoIds)
        {
            var dbContext = await GetDbContextAsync();

            return await dbContext.Tiktoks.AsQueryable()
                .Where(_ => videoIds.Contains(_.VideoId) && !_.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Tiktok>> GetByVideoIdsAsync(IEnumerable<string> videoIds)
        {
            var dbContext = await GetDbContextAsync();

            return await dbContext.Tiktoks.AsQueryable()
                .Where(_ => videoIds.Contains(_.VideoId) && !_.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Tiktok>> GetUncrawledTiktoks(int intervalHours, DateTime fromDateTime, DateTime? toDateTime)
        {
            //var query = (await GetMongoQueryableAsync())
            //.WhereIf(true, _ => !_.IsNotAvailable);

            var dbContext = await GetDbContextAsync();
            return await dbContext.Tiktoks.AsQueryable()
                .Where
                (
                    p => !p.IsDeleted
                         && (p.CreatedDateTime == null
                             || p.CreatedDateTime.Value > fromDateTime && (toDateTime == null || p.CreatedDateTime.Value < toDateTime)
                         )
                )
                .Where
                (
                    p => p.LastCrawledDateTime == null
                         || p.LastCrawledDateTime < DateTime.UtcNow.AddHours(-intervalHours)
                )
                .ToListAsync();
        }

        public async Task<List<Tiktok>> GetChartPosts(
            string channelId = null,
            DateTime? startDateTime = null,
            DateTime? endDateTime = null,
            IEnumerable<Guid?> groupIds = null,
            CancellationToken cancellationToken = default)
        {
            var query = (await GetDbContextAsync(cancellationToken)).Tiktoks.AsQueryable().Where(x=>!x.IsDeleted)
                .WhereIf(startDateTime.HasValue, p => p.CreatedDateTime >= startDateTime)
                .WhereIf(endDateTime.HasValue, p => p.CreatedDateTime <= endDateTime)
                .WhereIf(channelId.IsNotNullOrWhiteSpace(), p => p.ChannelId == channelId)
                .WhereIf(groupIds != null, p => groupIds.Contains(p.Id))
                .OrderByDescending(p => p.TotalCount);

            return await query.As<IMongoQueryable<Tiktok>>()
                .PageBy<Tiktok, IMongoQueryable<Tiktok>>(0, int.MaxValue)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<List<Tiktok>> GetListExtendAsync(
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
            bool? isValid = null,
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
            IEnumerable<Guid> appUserIds = null,
            IEnumerable<string> channelIds = null,
            IEnumerable<Guid> campaignIds = null,
            IEnumerable<Guid> groupIds = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilterExtend
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
                isValid,
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
                isNew,
                appUserIds,
                channelIds,
                campaignIds,
                groupIds
            );
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? TiktokConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Tiktok>>()
                .PageBy<Tiktok, IMongoQueryable<Tiktok>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<List<TiktokWithNavigationProperties>> GetListWithNavigationPropertiesExtendAsync(
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
            string videoId = null,
            bool? isNotAvailable = null,
            bool? isValid = null,
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
            IEnumerable<Guid> appUserIds = null,
            IEnumerable<string> channelIds = null,
            IEnumerable<Guid> campaignIds = null,
            IEnumerable<Guid> groupIds = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilterExtend
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
                videoId,
                isNotAvailable,
                isValid,
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
                isNew,
                appUserIds,
                channelIds,
                campaignIds,
                groupIds
            );
            var posts = await query.OrderBy
                (
                    string.IsNullOrWhiteSpace(sorting)
                        ? TiktokConsts.GetDefaultSorting(false)
                        : sorting.Split('.').Last()
                )
                .As<IMongoQueryable<Tiktok>>()
                .PageBy<Tiktok, IMongoQueryable<Tiktok>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            var results = from p in posts
                join g in dbContext.Groups.AsQueryable().Where(x=>!x.IsDeleted) on p.ChannelId equals g.Fid into pg
                from x2 in pg.DefaultIfEmpty()
                join u in dbContext.Users.AsQueryable().Where(x=>!x.IsDeleted) on p.AppUserId equals u.Id into pu
                from x3 in pu.DefaultIfEmpty()
                join ui in dbContext.UserInfos.AsQueryable().Where(x=>!x.IsDeleted) on p.AppUserId equals ui.AppUserId into pui
                from x4 in pui.DefaultIfEmpty()
                select new TiktokWithNavigationProperties
                {
                    Tiktok = p,
                    Group = x2,
                    AppUser = x3,
                    AppUserInfo = x4
                };
            return results.ToList();
        }

        public async Task<long> GetCountExtendAsync(
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
            bool? isValid = null,
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
            IEnumerable<Guid> appUserIds = null,
            IEnumerable<string> channelIds = null,
            IEnumerable<Guid> campaignIds = null,
            IEnumerable<Guid> groupIds = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilterExtend
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
                isValid,
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
                isNew,
                appUserIds,
                channelIds,
                campaignIds,
                groupIds
            );
            return await query.As<IMongoQueryable<Tiktok>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Tiktok> ApplyFilterExtend(
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
            bool? isValid = null,
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
            IEnumerable<Guid> appUserIds = null,
            IEnumerable<string> channelIds = null,
            IEnumerable<Guid> campaignIds = null,
            IEnumerable<Guid> groupIds = null)
        {
            if (filterText.IsNotNullOrWhiteSpace()) filterText = filterText.Trim();
            
            var offset = TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes;
            
            lastCrawledDateTimeMax = lastCrawledDateTimeMax?.AddMinutes(1).AddMinutes(offset);
            lastCrawledDateTimeMin = lastCrawledDateTimeMin?.AddMinutes(offset);
            
            createdDateTimeMax = createdDateTimeMax?.AddMinutes(1).AddMinutes(offset);
            createdDateTimeMin = createdDateTimeMin?.AddMinutes(offset);

            return query
                .WhereIf
                (
                    !string.IsNullOrWhiteSpace(filterText),
                    e => e.Url.Contains(filterText)
                         || e.ShortUrl.ToLower().Contains(filterText.ToLower())
                         || e.VideoId.ToLower().Contains(filterText.ToLower())
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
                .WhereIf(createdDateTimeMax.HasValue, e => e.CreatedDateTime < createdDateTimeMax.Value)
                .WhereIf(lastCrawledDateTimeMin.HasValue, e => e.LastCrawledDateTime >= lastCrawledDateTimeMin.Value)
                .WhereIf(lastCrawledDateTimeMax.HasValue, e => e.LastCrawledDateTime < lastCrawledDateTimeMax.Value)
                .WhereIf(channelId.IsNotNullOrEmpty(), e => e.ChannelId == channelId)
                .WhereIf(partnerId != null && partnerId != Guid.Empty, e => e.PartnerId == partnerId)
                .WhereIf(appUserId != null && appUserId != Guid.Empty, e => e.AppUserId == appUserId)
                .WhereIf(campaignId != null && campaignId != Guid.Empty, e => e.CampaignId == campaignId)
                .WhereIf(appUserIds != null, e => appUserIds.Contains((Guid)e.AppUserId))
                .WhereIf(channelIds != null, e => channelIds.Contains(e.ChannelId))
                .WhereIf(campaignIds != null, e => campaignIds.Contains((Guid)e.CampaignId))
                .WhereIf(groupIds != null, e => groupIds.Contains((Guid)e.GroupId))
                .WhereIf(isNew.HasValue,x=>x.IsNew == isNew);
        }
    }
}