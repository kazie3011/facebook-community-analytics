using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public partial class MongoPostRepository : MongoDbRepositoryBase<ApiMongoDbContext, Post, Guid>, IPostRepository
    {
        public MongoPostRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<PostWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var post = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var category = await (await GetDbContextAsync(cancellationToken)).Categories.AsQueryable().FirstOrDefaultAsync(e => e.Id == post.CategoryId, cancellationToken: cancellationToken);
            var group = await (await GetDbContextAsync(cancellationToken)).Groups.AsQueryable().FirstOrDefaultAsync(e => e.Id == post.GroupId, cancellationToken: cancellationToken);
            var appUser = await (await GetDbContextAsync(cancellationToken)).Users.AsQueryable().FirstOrDefaultAsync(e => e.Id == post.AppUserId, cancellationToken: cancellationToken);
            var campaign = await (await GetDbContextAsync(cancellationToken)).Campaigns.AsQueryable().FirstOrDefaultAsync(e => e.Id == post.CampaignId, cancellationToken: cancellationToken);
            var partner = await (await GetDbContextAsync(cancellationToken)).Partners.AsQueryable().FirstOrDefaultAsync(e => e.Id == post.PartnerId, cancellationToken: cancellationToken);

            return new PostWithNavigationProperties
            {
                Post = post,
                Category = category,
                Group = group,
                AppUser = appUser,
                Campaign = campaign,
                Partner = partner
            };
        }

        public async Task<List<PostWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
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
                status,
                postSourceType,
                note,
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
                partnerId
            );
            var posts = await IAsyncCursorSourceExtensions.ToListAsync
            (
                query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PostConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                    .As<IMongoQueryable<Post>>()
                    .PageBy<Post, IMongoQueryable<Post>>(skipCount, maxResultCount),
                GetCancellationToken(cancellationToken)
            );

            var dbContext = await GetDbContextAsync(cancellationToken);
            return posts.Select
                (
                    s => new PostWithNavigationProperties
                    {
                        Post = s,
                        Category = dbContext.Categories.AsQueryable().FirstOrDefault(e => e.Id == s.CategoryId && !e.IsDeleted),
                        Group = dbContext.Groups.AsQueryable().FirstOrDefault(e => e.Id == s.GroupId && !e.IsDeleted),
                        AppUser = dbContext.Users.AsQueryable().FirstOrDefault(e => e.Id == s.AppUserId && !e.IsDeleted),
                        AppUserInfo = dbContext.UserInfos.AsQueryable().FirstOrDefault(e => e.AppUserId == s.AppUserId && !e.IsDeleted),
                        Campaign = dbContext.Campaigns.AsQueryable().FirstOrDefault(e => e.Id == s.CampaignId && !e.IsDeleted),
                        Partner = dbContext.Partners.AsQueryable().FirstOrDefault(e => e.Id == s.PartnerId && !e.IsDeleted),
                    }
                )
                .ToList();
        }

        public async Task<List<Post>> GetListAsync(
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
            PostStatus? status = null,
            PostSourceType? postSourceType = null,
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
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
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
                status,
                postSourceType,
                note,
                createdDateTimeMin,
                createdDateTimeMax,
                lastCrawledDateTimeMin,
                lastCrawledDateTimeMax,
                submissionDateTimeMin,
                submissionDateTimeMax
            );
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PostConsts.GetDefaultSorting(false) : sorting);
            return await IAsyncCursorSourceExtensions.ToListAsync
            (
                query.As<IMongoQueryable<Post>>()
                    .PageBy<Post, IMongoQueryable<Post>>(skipCount, maxResultCount),
                GetCancellationToken(cancellationToken)
            );
        }

        public async Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter
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
                status,
                postSourceType,
                note,
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
                partnerId
            );
            return await query.As<IMongoQueryable<Post>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }


        protected virtual IQueryable<Post> ApplyFilter(
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
            Guid? partnerId = null)
        {
            return query
                .WhereIf
                (
                    !string.IsNullOrWhiteSpace(filterText),
                    e => e.Url.Contains(filterText) || e.Hashtag.Contains(filterText) || e.Fid.Contains(filterText) || e.Note.Contains(filterText)
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
                .WhereIf(status.HasValue, e => e.Status == status)
                .WhereIf(postSourceType.HasValue, e => e.PostSourceType == postSourceType)
                .WhereIf(!string.IsNullOrWhiteSpace(note), e => e.Note.Contains(note))
                .WhereIf(createdDateTimeMin.HasValue, e => e.CreatedDateTime >= createdDateTimeMin.Value)
                .WhereIf(createdDateTimeMax.HasValue, e => e.CreatedDateTime <= createdDateTimeMax.Value)
                .WhereIf(lastCrawledDateTimeMin.HasValue, e => e.LastCrawledDateTime >= lastCrawledDateTimeMin.Value)
                .WhereIf(lastCrawledDateTimeMax.HasValue, e => e.LastCrawledDateTime <= lastCrawledDateTimeMax.Value)
                .WhereIf(submissionDateTimeMin.HasValue, e => e.SubmissionDateTime >= submissionDateTimeMin.Value)
                .WhereIf(submissionDateTimeMax.HasValue, e => e.SubmissionDateTime <= submissionDateTimeMax.Value)
                .WhereIf(categoryId != null && categoryId != Guid.Empty, e => e.CategoryId == categoryId)
                .WhereIf(groupId != null && groupId != Guid.Empty, e => e.GroupId == groupId)
                .WhereIf(appUserId != null && appUserId != Guid.Empty, e => e.AppUserId == appUserId)
                .WhereIf(campaignId != null && campaignId != Guid.Empty, e => e.CampaignId == campaignId)
                .WhereIf(partnerId != null && partnerId != Guid.Empty, e => e.PartnerId == partnerId);
        }

    }
}