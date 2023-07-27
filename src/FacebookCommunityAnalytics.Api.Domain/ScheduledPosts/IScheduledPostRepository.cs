using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Proxies;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.ScheduledPosts
{
    public interface IScheduledPostRepository : IRepository<ScheduledPost, Guid>
    {
        Task<List<ScheduledPost>> GetListAsync(
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
            CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default);

        Task<List<SchedulePostWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            CancellationToken cancellationToken = default);
    }
}