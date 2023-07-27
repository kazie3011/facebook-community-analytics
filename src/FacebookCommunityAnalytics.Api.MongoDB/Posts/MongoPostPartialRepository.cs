using FacebookCommunityAnalytics.Api.Core.Enums;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public partial class MongoPostRepository
    {
        public async Task<List<PostWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            DateTime? fromDate,
            DateTime? toDate,
            List<Guid> staffIds,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText: string.Empty, createdDateTimeMin: fromDate, createdDateTimeMax: toDate);
            query = query.WhereIf(staffIds.IsNotNullOrEmpty(), x => x.AppUserId.HasValue && staffIds.Contains(x.AppUserId.Value));
            var posts = await query
                .As<IMongoQueryable<Post>>()
                .ToListAsync(cancellationToken);

            var dbContext = await GetDbContextAsync(cancellationToken);
            var results = from p in posts
                join c in dbContext.Categories.AsQueryable().Where(x=>!x.IsDeleted) on p.CategoryId equals c.Id into pc
                from x1 in pc.DefaultIfEmpty()
                join g in dbContext.Groups.AsQueryable().Where(x=>!x.IsDeleted) on p.GroupId equals g.Id into pg
                from x2 in pg.DefaultIfEmpty()
                join u in dbContext.Users.AsQueryable().Where(x=>!x.IsDeleted) on p.AppUserId equals u.Id into pu
                from x3 in pu.DefaultIfEmpty()
                join ui in dbContext.UserInfos.AsQueryable().Where(x=>!x.IsDeleted) on p.AppUserId equals ui.AppUserId into pui
                from x4 in pui.DefaultIfEmpty()
                select new PostWithNavigationProperties
                {
                    Post = p,
                    Category = x1,
                    Group = x2,
                    AppUser = x3,
                    AppUserInfo = x4
                };
            return results.ToList();
        }

        public async Task<List<Post>> GetAsync(IEnumerable<string> urls)
        {
            var dbContext = await GetDbContextAsync();

            return await dbContext.Posts.AsQueryable()
                .Where(_ => urls.Contains(_.Url) && !_.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Post>> GetUncrawledPosts(int intervalHours, DateTime fromDateTime, DateTime? toDateTime)
        {
            //var query = (await GetMongoQueryableAsync())
            //.WhereIf(true, _ => !_.IsNotAvailable);

            var dbContext = await GetDbContextAsync();
            return await dbContext.Posts.AsQueryable()
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
                .Where
                (
                    p => p.SubmissionDateTime.Value > fromDateTime && (toDateTime == null || p.SubmissionDateTime.Value < toDateTime)
                )
                .ToListAsync();
        }

        public async Task<List<Post>> GetNotAvailablePosts(int intervalHours, DateTime fromDateTime, DateTime? toDateTime)
        {
            var dbContext = await GetDbContextAsync();

            return await dbContext.Posts.AsQueryable()
                .Where(p => p.IsNotAvailable && !p.IsDeleted)
                .Where(p =>  p.SubmissionDateTime.Value >= fromDateTime && (toDateTime == null || p.SubmissionDateTime.Value <= toDateTime))
                .Where(p => p.LastCrawledDateTime != null && p.LastCrawledDateTime < DateTime.UtcNow.AddHours(-intervalHours))
                .ToListAsync();
        }

        public async Task<List<Post>> GetChartPosts(
            Guid? groupId = null,
            DateTime? startDateTime = null,
            DateTime? endDateTime = null,
            PostContentType? postContentType = null,
            IEnumerable<Guid?> groupIds = null,
            CancellationToken cancellationToken = default)
        {
            var query = (await GetDbContextAsync()).Posts.AsQueryable().Where(x=>!x.IsDeleted)
                .WhereIf(startDateTime.HasValue, p => p.CreatedDateTime >= startDateTime)
                .WhereIf(endDateTime.HasValue, p => p.CreatedDateTime <= endDateTime)
                .WhereIf(groupId.HasValue && groupId.Value != Guid.Empty, p => p.GroupId == groupId)
                .WhereIf(postContentType.HasValue, p => p.PostContentType == postContentType)
                .WhereIf(groupIds != null, p => groupIds.Contains(p.Id))
                .OrderByDescending(p => p.TotalCount);

            return await query.As<IMongoQueryable<Post>>()
                .PageBy<Post, IMongoQueryable<Post>>(0, int.MaxValue)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<List<Post>> GetListExtendAsync(
            string filterText = null,
            PostContentType? postContentType = null,
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
            PostSourceType? postSourceType = null,
            string note = null,
            DateTime? createdDateTimeMin = null,
            DateTime? createdDateTimeMax = null,
            DateTime? lastCrawledDateTimeMin = null,
            DateTime? lastCrawledDateTimeMax = null,
            DateTime? submissionDateTimeMin = null,
            DateTime? submissionDateTimeMax = null,
            Guid? categoryId = null,
            Guid? groupId = null,
            Guid? appUserId = null,
            Guid? campaignId = null,
            Guid? partnerId = null,
            IEnumerable<Guid> appUserIds = null,
            IEnumerable<Guid> groupIds = null,
            IEnumerable<Guid> campaignIds = null,
            string sorting = null,
            int maxResultCount = Int32.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilterExtend
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                postContentType,
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
                postSourceType,
                note,
                null,
                createdDateTimeMin,
                createdDateTimeMax,
                lastCrawledDateTimeMin,
                lastCrawledDateTimeMax,
                submissionDateTimeMin,
                submissionDateTimeMax,
                categoryId,
                groupId,
                appUserId,
                campaignId,
                partnerId,
                appUserIds,
                groupIds,
                campaignIds
            );
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PostConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Post>>()
                .PageBy<Post, IMongoQueryable<Post>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<List<PostWithNavigationProperties>> GetListWithNavigationPropertiesExtendAsync(
            string filterText = null,
            PostContentType? postContentType = null,
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
            PostSourceType? postSourceType = null,
            string note = null,
            int? clientOffsetInMinutes = null,
            DateTime? createdDateTimeMin = null,
            DateTime? createdDateTimeMax = null,
            DateTime? lastCrawledDateTimeMin = null,
            DateTime? lastCrawledDateTimeMax = null,
            DateTime? submissionDateTimeMin = null,
            DateTime? submissionDateTimeMax = null,
            Guid? categoryId = null,
            Guid? groupId = null,
            Guid? appUserId = null,
            Guid? campaignId = null,
            Guid? partnerId = null,
            IEnumerable<Guid> appUserIds = null,
            IEnumerable<Guid> groupIds = null,
            IEnumerable<Guid> campaignIds = null,
            IEnumerable<PostSourceType> postSourceTypes = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilterExtend
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                postContentType,
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
                postSourceType,
                note,
                clientOffsetInMinutes,
                createdDateTimeMin,
                createdDateTimeMax,
                lastCrawledDateTimeMin,
                lastCrawledDateTimeMax,
                submissionDateTimeMin,
                submissionDateTimeMax,
                categoryId,
                groupId,
                appUserId,
                campaignId,
                partnerId,
                appUserIds,
                groupIds,
                campaignIds,
                postSourceTypes
            );
            var posts = await query.OrderBy
                (
                    string.IsNullOrWhiteSpace(sorting)
                        ? PostConsts.GetDefaultSorting(false)
                        : sorting.Split('.').Last()
                )
                .As<IMongoQueryable<Post>>()
                .PageBy<Post, IMongoQueryable<Post>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            var results = from p in posts
                join c in dbContext.Categories.AsQueryable().Where(x=>!x.IsDeleted) on p.CategoryId equals c.Id into pc
                from x1 in pc.DefaultIfEmpty()
                join g in dbContext.Groups.AsQueryable().Where(x=>!x.IsDeleted) on p.GroupId equals g.Id into pg
                from x2 in pg.DefaultIfEmpty()
                join cp in dbContext.Campaigns.AsQueryable().Where(x=>!x.IsDeleted) on p.CampaignId equals cp.Id into cpg
                from x3 in cpg.DefaultIfEmpty()
                join u in dbContext.Users.AsQueryable().Where(x=>!x.IsDeleted) on p.AppUserId equals u.Id into pu
                from x4 in pu.DefaultIfEmpty()
                join ui in dbContext.UserInfos.AsQueryable().Where(x=>!x.IsDeleted) on p.AppUserId equals ui.AppUserId into pui
                from x5 in pui.DefaultIfEmpty()
                select new PostWithNavigationProperties
                {
                    Post = p,
                    Category = x1,
                    Group = x2,
                    Campaign = x3,
                    AppUser = x4,
                    AppUserInfo = x5
                };
            return results.ToList();
        }

        public async Task<List<PostWithNavigationProperties>> GetPostsByEvaluationAsync(Guid appUserId, int month, int year, CancellationToken cancellationToken = default)
        {
            var firstDayOfMonth = new DateTime(year, month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1);

            var dbContext = await GetDbContextAsync(cancellationToken);
            
            var posts = await dbContext.Posts.AsQueryable()
                .Where(x => x.AppUserId == appUserId 
                            && x.CreatedDateTime.HasValue 
                            && x.CreatedDateTime >= firstDayOfMonth && x.CreatedDateTime <= lastDayOfMonth).ToListAsync(cancellationToken);
            
             var results = from p in posts
                            join c in dbContext.Categories.AsQueryable().Where(x=>!x.IsDeleted) on p.CategoryId equals c.Id into pc
                            from x1 in pc.DefaultIfEmpty()
                            join g in dbContext.Groups.AsQueryable().Where(x=>!x.IsDeleted) on p.GroupId equals g.Id into pg
                            from x2 in pg.DefaultIfEmpty() join cp in dbContext.Campaigns.AsQueryable().Where(x=>!x.IsDeleted) on p.CampaignId equals cp.Id into cpg
                            from x3 in cpg.DefaultIfEmpty()
                            join u in dbContext.Users.AsQueryable().Where(x=>!x.IsDeleted) on p.AppUserId equals u.Id into pu
                            from x4 in pu.DefaultIfEmpty()
                            join ui in dbContext.UserInfos.AsQueryable().Where(x=>!x.IsDeleted) on p.AppUserId equals ui.AppUserId into pui
                            from x5 in pui.DefaultIfEmpty()
                            select new PostWithNavigationProperties
                            {
                                Post = p,
                                Category = x1,
                                Group = x2,
                                Campaign = x3,
                                AppUser = x4,
                                AppUserInfo = x5
                            };
                        return results.ToList();
        }
        public async Task<long> GetCountExtendAsync(
            string filterText = null,
            PostContentType? postContentType = null,
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
            PostSourceType? postSourceType = null,
            string note = null,
            int? clientOffsetInMinutes = null,
            DateTime? createdDateTimeMin = null,
            DateTime? createdDateTimeMax = null,
            DateTime? lastCrawledDateTimeMin = null,
            DateTime? lastCrawledDateTimeMax = null,
            DateTime? submissionDateTimeMin = null,
            DateTime? submissionDateTimeMax = null,
            Guid? categoryId = null,
            Guid? groupId = null,
            Guid? appUserId = null,
            Guid? campaignId = null,
            Guid? partnerId = null,
            IEnumerable<Guid> appUserIds = null,
            IEnumerable<Guid> groupIds = null,
            IEnumerable<Guid> campaignIds = null,
            IEnumerable<PostSourceType> postSourceTypes = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilterExtend
            (
                (await GetMongoQueryableAsync(cancellationToken)),
                filterText,
                postContentType,
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
                postSourceType,
                note,
                clientOffsetInMinutes,
                createdDateTimeMin,
                createdDateTimeMax,
                lastCrawledDateTimeMin,
                lastCrawledDateTimeMax,
                submissionDateTimeMin,
                submissionDateTimeMax,
                categoryId,
                groupId,
                appUserId,
                campaignId,
                partnerId,
                appUserIds,
                groupIds,
                campaignIds,
                postSourceTypes
            );
            return await query.As<IMongoQueryable<Post>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Post> ApplyFilterExtend(
            IQueryable<Post> query,
            string filterText,
            PostContentType? postContentType = null,
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
            PostSourceType? postSourceType = null,
            string note = null,
            int? clientOffsetInMinutes = null,
            DateTime? createdDateTimeMin = null,
            DateTime? createdDateTimeMax = null,
            DateTime? lastCrawledDateTimeMin = null,
            DateTime? lastCrawledDateTimeMax = null,
            DateTime? submissionDateTimeMin = null,
            DateTime? submissionDateTimeMax = null,
            Guid? categoryId = null,
            Guid? groupId = null,
            Guid? appUserId = null,
            Guid? campaignId = null,
            Guid? partnerId = null,
            IEnumerable<Guid> appUserIds = null,
            IEnumerable<Guid> groupIds = null,
            IEnumerable<Guid> campaignIds = null,
            IEnumerable<PostSourceType> postSourceTypes = null)

        {
            if (filterText.IsNotNullOrWhiteSpace()) filterText = filterText.Trim();

            return query
                .WhereIf
                (
                    !string.IsNullOrWhiteSpace(filterText),
                    e => e.Url.Contains(filterText)
                         || e.Hashtag.ToLower().Contains(filterText.ToLower())
                         || e.Fid.ToLower().Contains(filterText.ToLower())
                         || e.Note.Contains(filterText)
                )
                .WhereIf(postContentType.HasValue, e => e.PostContentType == postContentType)
                .WhereIf(postCopyrightType.HasValue, e => e.PostCopyrightType == postCopyrightType)
                .WhereIf(!string.IsNullOrWhiteSpace(url), e => e.Url.Contains(url))
                .WhereIf(likeCountMin.HasValue, e => e.LikeCount >= likeCountMin.Value)
                .WhereIf(likeCountMax.HasValue, e => e.LikeCount <= likeCountMax.Value)
                .WhereIf(commentCountMin.HasValue, e => e.CommentCount >= commentCountMin.Value)
                .WhereIf(commentCountMax.HasValue, e => e.CommentCount <= commentCountMax.Value)
                .WhereIf(shareCountMin.HasValue, e => e.ShareCount >= shareCountMin.Value)
                .WhereIf(shareCountMax.HasValue, e => e.ShareCount <= shareCountMax.Value)
                .WhereIf(totalCountMin.HasValue, e => e.TotalCount >= totalCountMin.Value)
                .WhereIf(totalCountMax.HasValue, e => e.TotalCount <= totalCountMax.Value)
                .WhereIf(!string.IsNullOrWhiteSpace(hashtag), e => e.Hashtag.Contains(hashtag))
                .WhereIf(!string.IsNullOrWhiteSpace(fid), e => e.Fid.Contains(fid))
                .WhereIf(isNotAvailable.HasValue, e => e.IsNotAvailable == isNotAvailable)
                .WhereIf(isValid.HasValue, e => e.IsValid == isValid)
                .WhereIf(status.HasValue, e => e.Status == status)
                .WhereIf(postSourceType.HasValue, e => e.PostSourceType == postSourceType)
                .WhereIf(!string.IsNullOrWhiteSpace(note), e => e.Note.Contains(note))
                .WhereIf(createdDateTimeMin.HasValue, e => e.CreatedDateTime == null || e.CreatedDateTime >= createdDateTimeMin.Value)
                .WhereIf(createdDateTimeMax.HasValue, e => e.CreatedDateTime == null || e.CreatedDateTime < createdDateTimeMax.Value)
                .WhereIf(lastCrawledDateTimeMin.HasValue, e => e.LastCrawledDateTime == null || e.LastCrawledDateTime >= lastCrawledDateTimeMin.Value)
                .WhereIf(lastCrawledDateTimeMax.HasValue, e => e.LastCrawledDateTime == null || e.LastCrawledDateTime <= lastCrawledDateTimeMax.Value)
                .WhereIf(submissionDateTimeMin.HasValue, e => e.SubmissionDateTime == null || e.SubmissionDateTime >= submissionDateTimeMin.Value)
                .WhereIf(submissionDateTimeMax.HasValue, e => e.SubmissionDateTime == null || e.SubmissionDateTime <= submissionDateTimeMax.Value)
                .WhereIf(categoryId != null && categoryId != Guid.Empty, e => e.CategoryId == categoryId)
                .WhereIf(groupId != null && groupId != Guid.Empty, e => e.GroupId == groupId)
                .WhereIf(appUserId != null && appUserId != Guid.Empty, e => e.AppUserId == appUserId)
                .WhereIf(campaignId != null && campaignId != Guid.Empty, e => e.CampaignId == campaignId)
                .WhereIf(partnerId != null && partnerId != Guid.Empty, e => e.PartnerId == partnerId)
                .WhereIf(appUserIds != null, e => appUserIds.Contains((Guid) e.AppUserId))
                .WhereIf(groupIds != null, e => groupIds.Contains((Guid) e.GroupId))
                .WhereIf(campaignIds != null, e => campaignIds.Contains(e.CampaignId.Value))
                .WhereIf(postSourceTypes != null, e => postSourceTypes.Contains(e.PostSourceType));
        }
    }
}