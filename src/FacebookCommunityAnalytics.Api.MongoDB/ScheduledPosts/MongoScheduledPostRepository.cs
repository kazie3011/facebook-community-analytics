using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.MongoDB;
using FacebookCommunityAnalytics.Api.Proxies;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace FacebookCommunityAnalytics.Api.ScheduledPosts
{
    public class MongoScheduledPostRepository : MongoDbRepository<ApiMongoDbContext, ScheduledPost, Guid>, IScheduledPostRepository
    {
        public MongoScheduledPostRepository(IMongoDbContextProvider<ApiMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<ScheduledPost>> GetListAsync(
            string filterText = null,
            string content = null,
            bool? isAutoPost = null,
            DateTime? scheduledPostDateTimeMin = null,
            DateTime? scheduledPostDateTimeMax = null,
            DateTime? postedAtMin = null,
            DateTime? postedAtMax = null,
            string groupId = null,
            PostContentType? postContentType = null,
            PostCopyrightType? postCopyrightType = null,
            Guid? categoryId = null,
            bool? isPosted = null,
            Guid? appUserId = null,
            IEnumerable<Guid> appUserIds = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, content, isAutoPost, scheduledPostDateTimeMin, scheduledPostDateTimeMax, postedAtMin, postedAtMax, groupId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ScheduledPostConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<ScheduledPost>>()
                .PageBy<ScheduledPost, IMongoQueryable<ScheduledPost>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string content = null,
            bool? isAutoPost = null,
            DateTime? scheduledPostDateTimeMin = null,
            DateTime? scheduledPostDateTimeMax = null,
            DateTime? postedAtMin = null,
            DateTime? postedAtMax = null,
            string groupId = null,
            PostContentType? postContentType = null,
            PostCopyrightType? postCopyrightType = null,
            Guid? categoryId = null,
            bool? isPosted = null,
            Guid? appUserId = null,
            IEnumerable<Guid> appUserIds = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, content, isAutoPost, scheduledPostDateTimeMin, scheduledPostDateTimeMax, postedAtMin, postedAtMax, groupId);
            return await query.As<IMongoQueryable<ScheduledPost>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<ScheduledPost> ApplyFilter(
            IQueryable<ScheduledPost> query,
            string filterText = null,
            string content = null,
            bool? isAutoPost = null,
            DateTime? scheduledPostDateTimeMin = null,
            DateTime? scheduledPostDateTimeMax = null,
            DateTime? postedAtMin = null,
            DateTime? postedAtMax = null,
            string groupId = null,
            PostContentType? postContentType = null,
            PostCopyrightType? postCopyrightType = null,
            Guid? categoryId = null,
            bool? isPosted = null,
            Guid? appUserId = null,
            IEnumerable<Guid> appUserIds = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Content.Contains(filterText) || e.GroupIds.Contains(filterText))
                .WhereIf(!string.IsNullOrWhiteSpace(content), e => e.Content.Contains(content))
                .WhereIf(scheduledPostDateTimeMin.HasValue, e => e.ScheduledPostDateTime >= scheduledPostDateTimeMin.Value)
                .WhereIf(scheduledPostDateTimeMax.HasValue, e => e.ScheduledPostDateTime <= scheduledPostDateTimeMax.Value)
                .WhereIf(postedAtMin.HasValue, e => e.PostedAt >= postedAtMin.Value)
                .WhereIf(postedAtMax.HasValue, e => e.PostedAt <= postedAtMax.Value)
                .WhereIf(!string.IsNullOrEmpty(groupId), e => e.GroupIds.Contains(groupId))
                .WhereIf(isAutoPost.HasValue, e => e.IsAutoPost == isAutoPost)
                .WhereIf(postContentType.HasValue, e => e.PostContentType == postContentType)
                .WhereIf(postCopyrightType.HasValue, e => e.PostCopyrightType == postCopyrightType)
                .WhereIf(categoryId != null && categoryId != Guid.Empty, e => e.CategoryId == categoryId)
                .WhereIf(isPosted.HasValue, e => e.IsPosted == isPosted)
                .WhereIf(appUserId != null && appUserId != Guid.Empty, e => e.AppUserId == appUserId)
                .WhereIf(appUserIds != null, e => appUserIds.Contains((Guid)e.AppUserId));
        }

        public async Task<List<SchedulePostWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string content = null,
            bool? isAutoPost = null,
            DateTime? scheduledPostDateTimeMin = null,
            DateTime? scheduledPostDateTimeMax = null,
            DateTime? postedAtMin = null,
            DateTime? postedAtMax = null,
            string groupId = null,
            PostContentType? postContentType = null,
            PostCopyrightType? postCopyrightType = null,
            Guid? categoryId = null,
            bool? isPosted = null,
            Guid? appUserId = null,
            IEnumerable<Guid> appUserIds = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, content, isAutoPost, scheduledPostDateTimeMin, scheduledPostDateTimeMax, postedAtMin, postedAtMax, groupId, postContentType, postCopyrightType, categoryId, isPosted, appUserId, appUserIds);
            var scheduledPosts = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ScheduledPostConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<ScheduledPost>>()
                .PageBy<ScheduledPost, IMongoQueryable<ScheduledPost>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
            var dbContext = await GetDbContextAsync(cancellationToken);
            return scheduledPosts.Select(s => new SchedulePostWithNavigationProperties
            {
                ScheduledPost = s,
                Category = dbContext.Categories.AsQueryable().FirstOrDefault(e => e.Id == s.CategoryId && !e.IsDeleted),
                AppUser = dbContext.Users.AsQueryable().FirstOrDefault(e => e.Id == s.AppUserId && !e.IsDeleted),
                AppUserInfo = dbContext.UserInfos.AsQueryable().FirstOrDefault(e => e.AppUserId == s.AppUserId && !e.IsDeleted)
            }).ToList();
        }
    }
}