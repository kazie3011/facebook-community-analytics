using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using Volo.Abp.Domain.Services;
using System.Linq;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Integrations.Tiktok;
using FacebookCommunityAnalytics.Api.TrendingDetails;
using FluentDateTime;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public interface ITiktokDomainService : IDomainService
    {
        Task<List<TiktokExportRow>> GetExportRows(GetTiktoksInputExtend input);
        Task UpdateTiktokState(UpdateTiktokStateApiRequest input);
        Task<List<TiktokWeeklyTotalFollowers>> GetWeeklyTotalFollowersReport(GetTiktokWeeklyTotalFollowersRequest request);
        Task<List<TiktokWeeklyTotalViews>> GetWeeklyTotalViewsReport(DateTime? timeFrom = null, DateTime? timeTo = null);
        Task<List<TikTokMCNReport>> GetTiktokMCNWeeklyReports(DateTime? timeFrom = null, DateTime? timeTo = null);
        Task<List<TikTokMonthlyTotalFollowers>> GetMonthlyTotalFollowersReport(GetTiktokMonthlyTotalFollowersRequest request);
        Task<List<TikTokMonthlyTotalViews>> GetMonthlyTotalViewsReport(DateTime? timeFrom = null, DateTime? timeTo = null);
        Task<List<TikTokMCNReport>> GetTiktokMCNMonthlyReports(DateTime? timeFrom = null, DateTime? timeTo = null, TikTokMCNType? tikTokMcnType = null);
        Task<List<TikTokMCN>> GetListTikTokMCN(TikTokMCNType tikTokMcnType);
        IEnumerable<(int week, DateTime weekStart, DateTime weekEnd)> GetReportWeeks(DateTime timeFrom, DateTime timeTo, int maximumCount);
        IEnumerable<(DateTime monthStart, DateTime monthEnd)> GetReportMonths(DateTime timeFrom, DateTime timeTo, int count = 8);
        Task<List<TrendingDetailDto>> GetTrendingDetails(DateTime fromDate, DateTime toDate, int count);
    }

    public class TiktokDomainService : BaseDomainService, ITiktokDomainService
    {
        private readonly ITiktokRepository _tiktokRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IRepository<GroupStatsHistory> _groupStatsHistoryRepository;
        private readonly IRepository<TiktokStat> _tiktokStatRepository;
        private readonly IRepository<TikTokMCN> _tikTokMcnsRepository;
        private readonly IRepository<TrendingDetail> _trendingDetailsRepository;

        private readonly Dictionary<GroupCategoryType, int> _categoryOrder = new()
        {
            { GroupCategoryType.Travel, 1 },
            { GroupCategoryType.FnB, 2 },
            { GroupCategoryType.Daily, 3 },
            { GroupCategoryType.Beauty, 4 },
            { GroupCategoryType.Fashion, 5 },
            { GroupCategoryType.Education, 6 },
            { GroupCategoryType.Architecture, 7 },
            { GroupCategoryType.TravelAndSport, 8 },
            { GroupCategoryType.Sport, 9 },
            { GroupCategoryType.Pets, 10 },
            { GroupCategoryType.General, 11 },
            { GroupCategoryType.Dalat, 12 },
            { GroupCategoryType.Entertainment, 13 },
            { GroupCategoryType.Talent, 14 },
            { GroupCategoryType.HappyDay, 15 },
            { GroupCategoryType.Unknown, 16 },
            { GroupCategoryType.FilterNoSelect, 17 },
        };

        public TiktokDomainService(
            ITiktokRepository tiktokRepository,
            IGroupRepository groupRepository,
            IRepository<GroupStatsHistory> groupStatsHistoryRepository,
            IRepository<TiktokStat> tiktokStatRepository,
            IRepository<TikTokMCN> tikTokMcnsRepository,
            IRepository<TrendingDetail> trendingDetailsRepository)
        {
            _tiktokRepository = tiktokRepository;
            _groupRepository = groupRepository;
            _groupStatsHistoryRepository = groupStatsHistoryRepository;
            _tiktokStatRepository = tiktokStatRepository;
            _tikTokMcnsRepository = tikTokMcnsRepository;
            _trendingDetailsRepository = trendingDetailsRepository;
        }

        private async Task<List<Group>> GetMCNGDLChannels()
        {
            var mcnVietNamIds = (await _tikTokMcnsRepository.GetQueryableAsync()).Where(x => x.MCNType == TikTokMCNType.MCNGdl).Select(x => x.Id).ToList();
            var channels = (await _groupRepository.GetQueryableAsync()).Where(x => x.GroupSourceType == GroupSourceType.Tiktok && x.McnId.HasValue && mcnVietNamIds.Contains(x.McnId.Value)).ToList();
            return channels;
        }

        public async Task<List<TiktokExportRow>> GetExportRows(GetTiktoksInputExtend input)
        {
            var groupIds = new List<Guid>();
            if (input.TikTokMcnType == TikTokMCNType.MCNGdl)
            {
                groupIds = (await GetMCNGDLChannels()).Select(x => x.Id).ToList();
            }

            var posts = await _tiktokRepository.GetListWithNavigationPropertiesAsync
            (
                input.Search,
                input.CreatedDateTimeMin,
                input.CreatedDateTimeMax,
                input.SendEmail,
                null,
                groupIds,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );

            posts = posts.Where(properties => properties.Tiktok.IsNew).ToList();
            posts = posts.OrderBy((x) => GetKeyIndex(x.Group.GroupCategoryType)).ToList();
            var rows = ObjectMapper.Map<List<TiktokWithNavigationProperties>, List<TiktokExportRow>>(posts);
            return rows;
        }

        public async Task UpdateTiktokState(UpdateTiktokStateApiRequest input)
        {
            var groupIds = new List<Guid>();
            if (input.TikTokMcnType == TikTokMCNType.MCNGdl)
            {
                groupIds = (await GetMCNGDLChannels()).Select(x => x.Id).ToList();
            }

            var posts = await _tiktokRepository.GetListWithNavigationPropertiesAsync
            (
                input.Search,
                input.CreatedDateTimeMin,
                input.CreatedDateTimeMax,
                input.SendEmail,
                null,
                groupIds,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );
            var postIds = posts.Select(x => x.Tiktok.Id).ToList();

            var postEntities = _tiktokRepository.Where(x => postIds.Contains(x.Id)).ToList();
            foreach (var postEntity in postEntities)
            {
                postEntity.IsNew = input.IsNew;
            }

            await _tiktokRepository.UpdateManyAsync(postEntities);
        }

        public async Task<List<TiktokWeeklyTotalFollowers>> GetWeeklyTotalFollowersReport(GetTiktokWeeklyTotalFollowersRequest request)
        {
            var results = new List<TiktokWeeklyTotalFollowers>();

            var (startTime, endTime) = GetWeeklyReportDate(request.TimeFrom, request.TimeTo);

            var tiktokPosts = await _tiktokRepository.GetListExtendAsync(createdDateTimeMin: startTime, createdDateTimeMax: endTime);
            var groups = await _groupRepository.GetListAsync(groupSourceType: GroupSourceType.Tiktok, isActive: true);
            var groupIds = groups.Select(x => x.Id);
            var groupStatsHistories = await _groupStatsHistoryRepository.GetListAsync
            (
                x => x.GroupId != null
                     && groupIds.Contains(x.GroupId.Value)
                     && x.CreatedAt >= startTime
                     && x.CreatedAt <= endTime
            );
            var reportWeeks = GetReportWeeks(startTime, endTime, 8);

            foreach (var group in groups)
            {
                var stats = groupStatsHistories.Where(x => x.GroupId == group.Id).ToList();
                var groupTiktokPosts = tiktokPosts.Where(_ => _.GroupId == group.Id).ToList();

                foreach (var (week, weekStart, weekEnd) in reportWeeks)
                {
                    var weeklyTiktokPosts = groupTiktokPosts.Where(_ => _.CreatedDateTime >= weekStart && _.CreatedDateTime <= weekEnd).ToList();

                    var followers = 0;
                    if (stats.Any())
                    {
                        var weekStatsHistories = stats.Where(x => weekStart <= x.CreatedAt && x.CreatedAt <= weekEnd).ToList();
                        followers = weekStatsHistories.Any() ? weekStatsHistories.OrderBy(x => x.CreatedAt).Last().GroupMembers : 0;
                    }

                    var viewsCount = weeklyTiktokPosts.Sum(_ => _.ViewCount);
                    var videosCount = weeklyTiktokPosts.Count;
                    results.Add
                    (
                        new TiktokWeeklyTotalFollowers
                        {
                            MonthName = weekStart.ToString("MMMM", CultureInfo.InvariantCulture),
                            ChannelName = group.Name,
                            WeekName = $"Week {week} - {weekStart.ToString("MMM yyyy", CultureInfo.InvariantCulture)}",
                            Followers = followers,
                            Views = viewsCount,
                            Videos = videosCount,
                            WeekStart = weekStart,
                            WeekEnd = weekEnd,
                            IsCurrentWeek = weekStart <= DateTime.UtcNow && DateTime.UtcNow <= weekEnd,
                            TiktokCategoryType = group.GroupCategoryType
                        }
                    );
                }
            }

            if (results.IsNotNullOrEmpty())
            {
                results = results.OrderBy(_ => _.WeekStart)
                    .ThenBy((x) => GetKeyIndex(x.TiktokCategoryType))
                    .ThenByDescending(_ => _.Followers)
                    .ToList();
                //results = results.OrderBy(_ => _.WeekStart).ThenBy(_ => _.TiktokCategoryType).ThenByDescending(_ => _.Followers).ToList();
            }

            if (request.OrderByWeekName.IsNotNullOrEmpty())
            {
                var groupBy = results.GroupBy(_ => _.ChannelName).ToList();
                switch (request.TiktokOrderByType)
                {
                    case TiktokOrderByType.Followers:
                        results = (from e in groupBy.OrderBy(_ => _.FirstOrDefault(m => m.WeekName == request.OrderByWeekName)?.Followers)
                            from e2 in e
                            select e2).ToList();
                        break;
                    case TiktokOrderByType.Views:
                        results = (from e in groupBy.OrderBy(_ => _.FirstOrDefault(m => m.WeekName == request.OrderByWeekName)?.Views)
                            from e2 in e
                            select e2).ToList();
                        break;
                    case TiktokOrderByType.Videos:
                        results = (from e in groupBy.OrderBy(_ => _.FirstOrDefault(m => m.WeekName == request.OrderByWeekName)?.Videos)
                            from e2 in e
                            select e2).ToList();
                        break;
                    case TiktokOrderByType.DescendingFollowers:
                        results = (from e in groupBy.OrderByDescending(_ => _.FirstOrDefault(m => m.WeekName == request.OrderByWeekName)?.Followers)
                            from e2 in e
                            select e2).ToList();
                        break;
                    case TiktokOrderByType.DescendingViews:
                        results = (from e in groupBy.OrderByDescending(_ => _.FirstOrDefault(m => m.WeekName == request.OrderByWeekName)?.Views)
                            from e2 in e
                            select e2).ToList();
                        break;
                    case TiktokOrderByType.DescendingVideos:
                        results = (from e in groupBy.OrderByDescending(_ => _.FirstOrDefault(m => m.WeekName == request.OrderByWeekName)?.Videos)
                            from e2 in e
                            select e2).ToList();
                        break;
                    case TiktokOrderByType.NoSelect:
                        break;
                }
            }

            return results;
        }

        public async Task<List<TiktokWeeklyTotalViews>> GetWeeklyTotalViewsReport(DateTime? timeFrom = null, DateTime? timeTo = null)
        {
            var results = new List<TiktokWeeklyTotalViews>();
            var stats = await _tiktokStatRepository.GetListAsync(x => x.Hashtag == "gdlfamily");

            var (startTime, endTime) = GetWeeklyReportDate(timeFrom, timeTo);

            var reportWeeks = GetReportWeeks(startTime, endTime, 8);
            foreach (var (week, weekStart, weekEnd) in reportWeeks)
            {
                long totalViews = 0;
                var weekStats = stats.Where(x => weekStart <= x.Date && x.Date <= weekEnd).ToList();
                if (weekStats.Any())
                {
                    totalViews = weekStats.OrderBy(x => x.Date).Last().Count;
                }

                var item = new TiktokWeeklyTotalViews
                {
                    MonthName = weekStart.ToString("MMMM", CultureInfo.InvariantCulture),
                    TotalViews = totalViews,
                    WeekName = $"Week {week} - {weekStart.ToString("MMM yyyy", CultureInfo.InvariantCulture)}",
                    WeekStart = weekStart,
                    WeekEnd = weekEnd,
                    IsCurrentWeek = weekStart <= DateTime.UtcNow && DateTime.UtcNow <= weekEnd
                };
                results.Add(item);
            }

            return results;
        }

        private KeyValuePair<DateTime, DateTime> GetWeeklyReportDate(DateTime? timeFrom, DateTime? timeTo)
        {
            DateTime startTime;
            DateTime endTime;
            if (timeFrom.HasValue && timeTo.HasValue)
            {
                startTime = timeFrom.Value.Date.GetFirstDateOfWeek();
                endTime = timeTo.Value.Date.Add(new TimeSpan(23, 59, 59));
            }
            else
            {
                endTime = DateTime.UtcNow.Date.AddSeconds(-1);
                startTime = endTime.Date.AddDays(-21);
            }

            return new KeyValuePair<DateTime, DateTime>(startTime, endTime);
        }

        public IEnumerable<(int week, DateTime weekStart, DateTime weekEnd)> GetReportWeeks(DateTime timeFrom, DateTime timeTo, int maximumCount)
        {
            var minWeekStart = timeFrom.GetFirstDateOfWeek();
            var results = new List<(int week, DateTime weekStart, DateTime weekEnd)>();

            while (true)
            {
                var week = DateTimeHelper.GetWeekOfYear(timeTo);
                var weekStart = timeTo.GetFirstDateOfWeek().Date;

                if (weekStart.Date < minWeekStart.Date)
                    break;

                var weekEnd = weekStart.GetLastDateOfWeek();

                results.Add(new ValueTuple<int, DateTime, DateTime>(week, weekStart, weekEnd.Date.Add(new TimeSpan(23, 59, 59))));

                timeTo = weekStart.Previous(DayOfWeek.Monday);
            }

            return results.OrderBy(x => x.weekStart).Skip(Math.Max(0, results.Count - maximumCount));
        }


        public async Task<List<TikTokMonthlyTotalFollowers>> GetMonthlyTotalFollowersReport(GetTiktokMonthlyTotalFollowersRequest request)
        {
            var results = new List<TikTokMonthlyTotalFollowers>();

            var (startTime, endTime) = GetMonthlyReportDate(request.TimeFrom, request.TimeTo);
            var tiktokPosts = await _tiktokRepository.GetListExtendAsync(createdDateTimeMin: startTime, createdDateTimeMax: endTime);
            var groups = await _groupRepository.GetListAsync(groupSourceType: GroupSourceType.Tiktok, isActive: true);
            var groupIds = groups.Select(x => x.Id);
            var groupStatsHistories = await _groupStatsHistoryRepository.GetListAsync
            (
                x => x.GroupId != null
                     && groupIds.Contains(x.GroupId.Value)
                     && x.CreatedAt >= startTime
                     && x.CreatedAt <= endTime
            );

            var reportMonths = GetReportMonths(startTime, endTime);

            foreach (var group in groups)
            {
                var stats = groupStatsHistories.Where(x => x.GroupId == group.Id).ToList();
                var groupTiktokPosts = tiktokPosts.Where(_ => _.GroupId == group.Id).ToList();
                foreach (var (monthStart, monthEnd) in reportMonths)
                {
                    var weeklyTiktokPosts = groupTiktokPosts.Where(_ => _.CreatedDateTime >= monthStart && _.CreatedDateTime <= monthEnd).ToList();

                    var followers = 0;
                    if (stats.Any())
                    {
                        var weekStatsHistories = stats.Where(x => monthStart <= x.CreatedAt && x.CreatedAt <= monthEnd).ToList();
                        followers = weekStatsHistories.Any() ? weekStatsHistories.OrderBy(x => x.CreatedAt).Last().GroupMembers : 0;
                    }

                    var viewsCount = weeklyTiktokPosts.Sum(_ => _.ViewCount);
                    var videosCount = weeklyTiktokPosts.Count;
                    var averageCount = videosCount > 0 ? viewsCount / videosCount : 0;
                    results.Add
                    (
                        new TikTokMonthlyTotalFollowers
                        {
                            MonthName = monthStart.ToString("MMMM", CultureInfo.InvariantCulture),
                            ChannelName = group.Name,
                            TimeTitle = L["TiktokReports.TimeTitle", monthStart.Month.ToString("00"), monthStart.Year],
                            Followers = followers,
                            Views = viewsCount,
                            Videos = videosCount,
                            Average = averageCount,
                            IsCurrentMonth = monthStart <= DateTime.UtcNow && DateTime.UtcNow <= monthEnd,
                            TiktokCategoryType = group.GroupCategoryType,
                            Ticks = monthStart.Ticks
                        }
                    );
                }
            }

            if (results.IsNotNullOrEmpty())
            {
                results = results.OrderBy(_ => _.Ticks)
                    .ThenBy((x) => GetKeyIndex(x.TiktokCategoryType))
                    .ThenByDescending(_ => _.Followers)
                    .ToList();
                //results = results.OrderBy(_ => _.Ticks).ThenBy(_ => _.TiktokCategoryType).ThenByDescending(_ => _.Followers).ToList();
            }

            if (request.OrderByTimeTitle.IsNotNullOrEmpty())
            {
                var groupBy = results.GroupBy(_ => _.ChannelName).ToList();
                switch (request.TiktokOrderByType)
                {
                    case TiktokOrderByType.Followers:
                        results = (from e in groupBy.OrderBy(_ => _.FirstOrDefault(m => m.TimeTitle == request.OrderByTimeTitle)?.Followers)
                            from e2 in e
                            select e2).ToList();
                        break;
                    case TiktokOrderByType.Views:
                        results = (from e in groupBy.OrderBy(_ => _.FirstOrDefault(m => m.TimeTitle == request.OrderByTimeTitle)?.Views)
                            from e2 in e
                            select e2).ToList();
                        break;
                    case TiktokOrderByType.Videos:
                        results = (from e in groupBy.OrderBy(_ => _.FirstOrDefault(m => m.TimeTitle == request.OrderByTimeTitle)?.Videos)
                            from e2 in e
                            select e2).ToList();
                        break;
                    case TiktokOrderByType.Average:
                        results = (from e in groupBy.OrderBy(_ => _.FirstOrDefault(m => m.TimeTitle == request.OrderByTimeTitle)?.Average)
                            from e2 in e
                            select e2).ToList();
                        break;
                    case TiktokOrderByType.DescendingFollowers:
                        results = (from e in groupBy.OrderByDescending(_ => _.FirstOrDefault(m => m.TimeTitle == request.OrderByTimeTitle)?.Followers)
                            from e2 in e
                            select e2).ToList();
                        break;
                    case TiktokOrderByType.DescendingViews:
                        results = (from e in groupBy.OrderByDescending(_ => _.FirstOrDefault(m => m.TimeTitle == request.OrderByTimeTitle)?.Views)
                            from e2 in e
                            select e2).ToList();
                        break;
                    case TiktokOrderByType.DescendingVideos:
                        results = (from e in groupBy.OrderByDescending(_ => _.FirstOrDefault(m => m.TimeTitle == request.OrderByTimeTitle)?.Videos)
                            from e2 in e
                            select e2).ToList();
                        break;
                    case TiktokOrderByType.DescendingAverage:
                        results = (from e in groupBy.OrderByDescending(_ => _.FirstOrDefault(m => m.TimeTitle == request.OrderByTimeTitle)?.Average)
                            from e2 in e
                            select e2).ToList();
                        break;
                    case TiktokOrderByType.NoSelect:
                        break;
                }
            }

            return results;
        }

        public async Task<List<TikTokMonthlyTotalViews>> GetMonthlyTotalViewsReport(DateTime? timeFrom = null, DateTime? timeTo = null)
        {
            var results = new List<TikTokMonthlyTotalViews>();
            var stats = await _tiktokStatRepository.GetListAsync(x => x.Hashtag == "gdlfamily");

            var (startTime, endTime) = GetMonthlyReportDate(timeFrom, timeTo);

            var reportMonths = GetReportMonths(startTime, endTime);

            foreach (var (monthStart, monthEnd) in reportMonths)
            {
                long totalViews = 0;
                var monthStats = stats.Where(x => monthStart <= x.Date && x.Date <= monthEnd).ToList();
                if (monthStats.Any())
                {
                    totalViews = monthStats.OrderBy(x => x.Date).Last().Count;
                }

                var item = new TikTokMonthlyTotalViews
                {
                    MonthName = monthStart.ToString("MMMM", CultureInfo.InvariantCulture),
                    TotalViews = totalViews,
                    TimeTitle = L["TiktokReports.TimeTitle", monthStart.Month.ToString("00"), monthEnd.Year],
                    IsCurrentMonth = monthStart <= DateTime.UtcNow && DateTime.UtcNow <= monthEnd
                };
                results.Add(item);
            }

            return results;
        }

        public async Task<List<TikTokMCNReport>> GetTiktokMCNWeeklyReports(DateTime? timeFrom = null, DateTime? timeTo = null)
        {
            var results = new List<TikTokMCNReport>();
            var stats = await _tiktokStatRepository.GetListAsync();
            var tiktokMcns = await _tikTokMcnsRepository.GetListAsync();
            var groups = await _groupRepository.GetListAsync(groupSourceType: GroupSourceType.Tiktok, isActive: true);
            var (startTime, endTime) = GetWeeklyReportDate(timeFrom, timeTo);

            var reportWeeks = GetReportWeeks(startTime, endTime, 8).ToList();

            foreach (var (week, weekStart, weekEnd) in reportWeeks)
            {
                var weekStats = stats.Where(x => weekStart <= x.Date && x.Date <= weekEnd).ToList();
                foreach (var tiktokMcn in tiktokMcns)
                {
                    var mcnStats = weekStats.Where(_ => _.Hashtag.Equals(tiktokMcn.HashTag));
                    var last = mcnStats.OrderBy(x => x.Date).LastOrDefault();
                    var item = new TikTokMCNReport()
                    {
                        Title = $"{L["TikTok.Dashboard.Week"].Value} {week} - {weekStart.ToString("MMM yyyy", CultureInfo.InvariantCulture)}",
                        HashTag = tiktokMcn.HashTag,
                        TotalFollowers = last?.Count ?? 0,
                        Name = tiktokMcn.Name
                    };

                    var group = groups.FirstOrDefault(x => x.McnId == tiktokMcn.Id);
                    item.TotalChannel = groups.Count(x => x.McnId == tiktokMcn.Id);
                    item.Url = group?.Url;

                    results.Add(item);
                }
            }

            return results;
        }

        public async Task<List<TikTokMCNReport>> GetTiktokMCNMonthlyReports(DateTime? timeFrom = null, DateTime? timeTo = null, TikTokMCNType? tikTokMcnType = null)
        {
            var stats = await _tiktokStatRepository.GetListAsync();
            var tiktokMcns = await _tikTokMcnsRepository.GetListAsync(x => tikTokMcnType == null || x.MCNType == tikTokMcnType);
            var groups = await _groupRepository.GetListAsync();
            var results = new List<TikTokMCNReport>();

            var (startTime, endTime) = GetMonthlyReportDate(timeFrom, timeTo);

            var reportMonths = GetReportMonths(startTime, endTime);
            foreach (var (monthStart, monthEnd) in reportMonths)
            {
                var startsByDate = stats.Where(x => monthStart <= x.Date && x.Date <= monthEnd).ToList();
                foreach (var g in startsByDate.GroupBy(x => x.Hashtag))
                {
                    var last = g.OrderBy(x => x.Date).LastOrDefault();
                    var mcn = tiktokMcns.FirstOrDefault(x => x.HashTag == g.Key);
                    if (last != null)
                    {
                        var item = new TikTokMCNReport()
                        {
                            Title = L["TikTok.Dashboard.Month", last.Date.Month.ToString("00"), last.Date.Year],
                            HashTag = g.Key,
                            TotalFollowers = last.Count,
                            Name = mcn?.Name,
                        };
                        if (mcn != null)
                        {
                            item.TotalChannel = groups.Count(x => x.McnId == mcn.Id);
                        }

                        results.Add(item);
                    }
                }
            }

            return results;
        }

        private KeyValuePair<DateTime, DateTime> GetMonthlyReportDate(DateTime? timeFrom, DateTime? timeTo)
        {
            DateTime startTime;
            DateTime endTime;
            if (timeFrom.HasValue && timeTo.HasValue)
            {
                startTime = timeFrom.Value;
                endTime = timeTo.Value;
                if (startTime == endTime)
                {
                    startTime = startTime.Date.AddMonths(-1);
                    endTime = endTime.Date.AddSeconds(-1);
                }
            }
            else
            {
                endTime = DateTime.UtcNow.Date.AddSeconds(-1);
                startTime = endTime.Date.AddMonths(-1);
            }

            return new KeyValuePair<DateTime, DateTime>(startTime, endTime);
        }

        public IEnumerable<(DateTime monthStart, DateTime monthEnd)> GetReportMonths(DateTime timeFrom, DateTime timeTo, int count = 8)
        {
            var maxMonthStart = new DateTime(timeTo.Year, timeTo.Month, 1);
            var results = new List<(DateTime monthStart, DateTime monthEnd)>();

            while (true)
            {
                var monthStart = new DateTime(timeFrom.Year, timeFrom.Month, 1);

                if (monthStart > maxMonthStart)
                    break;

                var monthEnd = monthStart.AddMonths(1).AddSeconds(-1);

                results.Add(new ValueTuple<DateTime, DateTime>(monthStart, monthEnd));
                timeFrom = monthEnd.AddDays(1).Date;
            }

            return results.OrderBy(x => x.monthStart).Skip(Math.Max(0, results.Count - count));
        }

        public async Task<List<TrendingDetailDto>> GetTrendingDetails(DateTime fromDate, DateTime toDate, int count)
        {
            var trendingDetails = await _trendingDetailsRepository.GetListAsync(_ => _.CreatedDateTime >= fromDate && _.CreatedDateTime <= toDate);
            return ObjectMapper.Map<List<TrendingDetail>, List<TrendingDetailDto>>(trendingDetails.OrderByDescending(_ => _.View).Take(count).ToList());
        }

        private int GetKeyIndex(GroupCategoryType type)
        {
            var key = _categoryOrder[type];
            return key == -1 ? int.MaxValue : key;
        }

        public async Task<List<TikTokMCN>> GetListTikTokMCN(TikTokMCNType tikTokMcnType)
        {
            return await _tikTokMcnsRepository.GetListAsync(_ => _.MCNType == tikTokMcnType);
        }
    }
}