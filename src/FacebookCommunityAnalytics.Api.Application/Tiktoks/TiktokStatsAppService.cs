using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Medias;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.TikTokMCNs;
using FacebookCommunityAnalytics.Api.TrendingDetails;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.Tiktok.Default)]
    public class TiktokStatsAppService : ApiAppService, ITiktokStatsAppService
    {
        private readonly ITiktokRepository              _tiktokRepository;
        private readonly ITiktokDomainService           _tiktokDomainService;
        private readonly IRepository<TikTokMCN>         _tikTokMcnsRepository;
        private readonly IGroupDomainService            _groupDomainService;
        private readonly IRepository<GroupStatsHistory> _groupStatsHistoryRepository;
        private readonly IRepository<TrendingDetail>    _trendingDetailsRepository;

        public TiktokStatsAppService(
            ITiktokRepository              tiktokRepository,
            ITiktokDomainService           tiktokDomainService,
            IRepository<TikTokMCN>         tikTokMcnsRepository,
            IGroupDomainService            groupDomainService,
            IRepository<GroupStatsHistory> groupStatsHistoryRepository,
            IRepository<TrendingDetail>    trendingDetailsRepository)
        {
            _tiktokRepository            = tiktokRepository;
            _tiktokDomainService         = tiktokDomainService;
            _tikTokMcnsRepository        = tikTokMcnsRepository;
            _groupDomainService          = groupDomainService;
            _groupStatsHistoryRepository = groupStatsHistoryRepository;
            _trendingDetailsRepository   = trendingDetailsRepository;
        }

        public async Task<PagedResultDto<TiktokWithNavigationPropertiesDto>> GetListAsync(GetTiktoksInputExtend input)
        {
            var groupIds = new List<Guid>();
            if (input.TikTokMcnType == TikTokMCNType.MCNGdl)
            {
                groupIds = (await _groupDomainService.GetMCNGDLChannels()).Select(x => x.Id).ToList();
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
            var count = await _tiktokRepository.GetCountAsync
                            (
                             input.Search,
                             input.CreatedDateTimeMin,
                             input.CreatedDateTimeMax,
                             input.SendEmail,
                             null,
                             groupIds
                            );
            var items = ObjectMapper.Map<List<TiktokWithNavigationProperties>, List<TiktokWithNavigationPropertiesDto>>(posts);
            var index = input.SkipCount;
            foreach (var item in items)
            {
                index++;
                item.Tiktok.Index = index;
            }

            return new PagedResultDto<TiktokWithNavigationPropertiesDto>()
            {
                Items      = items,
                TotalCount = count
            };
        }

        public async Task<List<TiktokExportRow>> GetExportRows(GetTiktoksInputExtend input)
        {
            var index      = 1;
            var tiktokList = await _tiktokDomainService.GetExportRows(input);
            foreach (var item in tiktokList)
            {
                item.Index = index;
                index++;
            }

            return tiktokList;
        }

        public Task<List<TiktokWeeklyTotalFollowers>> GetWeeklyTotalFollowersReport(GetTiktokWeeklyTotalFollowersRequest request)
        {
            return _tiktokDomainService.GetWeeklyTotalFollowersReport(request);
        }

        public Task<List<TiktokWeeklyTotalViews>> GetWeeklyTotalViewsReport(DateTime? timeFrom = null, DateTime? timeTo = null)
        {
            return _tiktokDomainService.GetWeeklyTotalViewsReport(timeFrom, timeTo);
        }

        public Task<List<TikTokMonthlyTotalFollowers>> GetMonthlyTotalFollowersReport(GetTiktokMonthlyTotalFollowersRequest request)
        {
            return _tiktokDomainService.GetMonthlyTotalFollowersReport(request);
        }

        public Task<List<TikTokMonthlyTotalViews>> GetMonthlyTotalViewsReport(DateTime? timeFrom = null, DateTime? timeTo = null)
        {
            return _tiktokDomainService.GetMonthlyTotalViewsReport(timeFrom, timeTo);
        }

        public Task<List<TikTokMCNReport>> GetTiktokMCNMonthlyReport(DateTime? timeFrom = null, DateTime? timeTo = null, TikTokMCNType? tikTokMcnType = null)
        {
            return _tiktokDomainService.GetTiktokMCNMonthlyReports(timeFrom, timeTo, tikTokMcnType);
        }

        public Task<List<TikTokMCNReport>> GetTiktokMCNWeeklyReports(DateTime? timeFrom = null, DateTime? timeTo = null)
        {
            return _tiktokDomainService.GetTiktokMCNWeeklyReports(timeFrom, timeTo);
        }

        public async Task<List<TikTokMCNDto>> GetListTikTokMCN(TikTokMCNType tikTokMcnType)
        {
            return ObjectMapper.Map<List<TikTokMCN>, List<TikTokMCNDto>>(await _tiktokDomainService.GetListTikTokMCN(tikTokMcnType));
        }

        //MCN GDL
        public async Task<FileResultDto> GetVideoImage(Guid videoId)
        {
            var videoTiktok = await _tiktokRepository.GetAsync(videoId);
            if (videoTiktok != null && videoTiktok.ThumbnailImage.IsNotNullOrEmpty())
            {
                var extension       = Path.GetExtension(videoTiktok.ThumbnailImage);
                var contentType     = $"image/{extension.Replace(".", string.Empty)}";
                var rootPathChannel = GlobalConfiguration.MediaConfiguration.TiktokRootPathVideo;
                var fullPathImage   = rootPathChannel.EndsWith("\\") ? $"{rootPathChannel}{videoTiktok.ThumbnailImage}" : $"{rootPathChannel}\\{videoTiktok.ThumbnailImage}";
                if (File.Exists(fullPathImage))
                {
                    var fileData = await System.IO.File.ReadAllBytesAsync(fullPathImage);
                    return new FileResultDto()
                    {
                        FileData    = fileData,
                        ContentType = contentType
                    };
                }
            }

            return null;
        }

        //MCN GDL
        public async Task<TiktokDto> GetTopVideoOfDay(DateTime startDate, DateTime endDate)
        {
            var channels   = await _groupDomainService.GetMCNGDLChannels();
            var channelIds = channels.Select(x => x.Id);
            var tiktokVideoTop = _tiktokRepository.Where
                                                   (x => x.GroupId != null && channelIds.Contains(x.GroupId.Value) && x.CreatedDateTime >= startDate && x.CreatedDateTime <= endDate)
                                                  .OrderByDescending(o => o.ViewCount)
                                                  .FirstOrDefault();
            return ObjectMapper.Map<Tiktok, TiktokDto>(tiktokVideoTop);
        }

        //MCN GDL
        public async Task<TopChannelDto> GetTopChannel(DateTime startDate, DateTime endDate)
        {
            var startDateMin    = DateTime.UtcNow.Date.AddDays(-7);
            var endDateMax      = DateTime.UtcNow.Date.AddTicks(-1);
            var topStatChannels = await GetTopStatChannels(startDateMin, endDateMax);
            return topStatChannels.OrderByDescending(x => x.IncrementFollower).FirstOrDefault();
        }

        //MCN GDL
        public async Task<List<TopChannelDto>> GetTopStatChannelsOfWeek()
        {
            var startDateMin    = DateTime.UtcNow.Date.AddDays(-7);
            var endDateMax      = DateTime.UtcNow.Date.AddTicks(-1);
            var topStatChannels = await GetTopStatChannels(startDateMin, endDateMax);
            var index           = 0;
            return topStatChannels.OrderByDescending(x => x.IncrementFollower)
                                  .Take(10)
                                  .Select
                                       (
                                        _ =>
                                        {
                                            index++;
                                            _.Index = index;
                                            return _;
                                        }
                                       )
                                  .ToList();
        }

        //MCN GDL
        public async Task<List<TopChannelDto>> GetTopStatChannelOfMonth()
        {
            var startDateMin    = DateTime.UtcNow.GetFirstDateOfMonth();
            var endDateMax      = DateTime.UtcNow.GetLastDateOfMonth();
            var topStatChannels = await GetTopStatChannels(startDateMin, endDateMax);
            topStatChannels = topStatChannels.OrderByDescending(x => x.IncrementFollower).Take(10).ToList();
            var index = 1;
            foreach (var item in topStatChannels)
            {
                item.Index = index;
                index++;
            }

            return topStatChannels;
        }

        public async Task<List<GroupDto>> GetGDLChannels()
        {
            var gdlChannels = await _groupDomainService.GetMCNGDLChannels();
            return ObjectMapper.Map<List<Group>, List<GroupDto>>(gdlChannels);
        }

        public async Task<List<TrendingDetailDto>> GetTrendingDetails(DateTime fromDate, DateTime toDate, int count)
        {
            return await _tiktokDomainService.GetTrendingDetails(fromDate, toDate, count);
        }

        public async Task<List<TrendingDetailDto>> GetTopTrendingContentsByDateRange(DateTime? from, DateTime? to, int contentNumber, bool calculateGrowth)
        {
            if (!from.HasValue || !to.HasValue) return new List<TrendingDetailDto>();
            var trendings          = await _trendingDetailsRepository.GetListAsync(_ => _.CreatedDateTime >= from.Value.AddDays(-7) && _.CreatedDateTime <= to.Value);
            var trendingDetails    = trendings.Where(x => x.CreatedDateTime >= from && x.CreatedDateTime <= to).DistinctBy(_ => _.Description).OrderByDescending(x => x.View).Take(contentNumber).ToList();
            var trendingDetailDtos = ObjectMapper.Map<List<TrendingDetail>, List<TrendingDetailDto>>(trendingDetails).ToList();
            trendingDetailDtos = trendingDetailDtos.Select
                                                    (
                                                     _ =>
                                                     {
                                                         _.Rank = trendingDetailDtos.IndexOf(_) + 1;
                                                         return _;
                                                     }
                                                    )
                                                   .ToList();
            if (calculateGrowth)
            {
                var previousTime           = from.Value;
                var lastDayTrendingDetails = new List<TrendingDetail>();
                while (lastDayTrendingDetails.IsNullOrEmpty() && previousTime > from.Value.AddDays(-7))
                {
                    previousTime = previousTime.AddDays(-1);
                    lastDayTrendingDetails = trendings.Where(x => x.CreatedDateTime >= previousTime && x.CreatedDateTime <= previousTime.Add(new TimeSpan(23, 59, 59)))
                                                      .OrderByDescending(x => x.View)
                                                      .ToList();
                }

                CalculateRank(trendingDetailDtos, lastDayTrendingDetails);
            }

            return trendingDetailDtos;
        }

        private void CalculateRank(List<TrendingDetailDto> trendingDetails, List<TrendingDetail> lastDayTrendingDetails)
        {
            foreach (var item in trendingDetails)
            {
                var preRankIndex = lastDayTrendingDetails.FindIndex(x => x.Description == item.Description);

                // Item not in lastDayTrendingDetails => -1
                item.Increase = preRankIndex == -1 ? 0 : preRankIndex - item.Rank + 1;
            }
        }

        private async Task<List<TopChannelDto>> GetTopStatChannels(DateTime startDateMin, DateTime endDateMax)
        {
            var topStatChannels = new List<TopChannelDto>();
            var channels        = await _groupDomainService.GetMCNGDLChannels();
            foreach (var channel in channels)
            {
                var groupStarts = (await _groupStatsHistoryRepository.GetListAsync(x => x.GroupId == channel.Id && x.CreatedAt.HasValue && x.CreatedAt >= startDateMin && x.CreatedAt <= endDateMax))
                                 .OrderBy(x => x.CreatedAt)
                                 .ToList();
                if (groupStarts.IsNullOrEmpty())
                {
                    continue;
                }

                var groupStartMin = groupStarts.FirstOrDefault();
                if (groupStartMin == null)
                {
                    continue;
                }

                var groupStartMax = groupStarts.LastOrDefault();
                if (groupStartMax == null)
                {
                    continue;
                }

                topStatChannels.Add
                    (
                     new TopChannelDto()
                     {
                         Group              = ObjectMapper.Map<Group, GroupDto>(channel),
                         StartTotalFollower = groupStartMin.GroupMembers,
                         EndTotalFollower   = groupStartMax.GroupMembers,
                         IncrementFollower  = groupStartMax.GroupMembers - groupStartMin.GroupMembers,
                         GrowthPercent      = ((double)(groupStartMax.GroupMembers - groupStartMin.GroupMembers) / groupStartMin.GroupMembers) * 100
                     }
                    );
            }

            return topStatChannels;
        }
    }
}