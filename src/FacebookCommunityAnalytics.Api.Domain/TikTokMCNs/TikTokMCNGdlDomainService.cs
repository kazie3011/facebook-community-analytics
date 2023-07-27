using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.Tiktoks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api.TikTokMCNs
{
    public interface ITikTokMCNGdlDomainService : IDomainService
    {
        Task<MultipleDataSourceChart<int>> GetMCNGDLChannelMonthlyInOutChart(DateTime fromDateTime, DateTime toDateTime);
        Task<MultipleDataSourceChart<int>> GetMCNGDLChannelWeekInOutChart(DateTime fromDateTime, DateTime toDateTime);

        Task<MultipleDataSourceChart<long>> GetMCNGDLViewAndCreatorChart(DateTime fromDateTime, DateTime toDateTime);

        Task<PieChartDataSource<double>> GetMCNGDLChannelContractStatusChart();
        
        Task<PieChartDataSource<double>> GetMCNGDLChannelCategoriesChart(List<GroupCategoryType> categoryTypes);
        Task<List<Tiktok>> GetTopMCNGDLVideos(GetTikTokVideosRequest request);
        Task<List<MCNVietNamChannelDto>> GetTopMCNVietNamChannel(DateTime fromDate, DateTime toDate, int count);
    }

    public class TikTokMCNGdlDomainService : BaseDomainService, ITikTokMCNGdlDomainService
    {
        private readonly IRepository<TikTokMCN> _tikTokMcnRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ITiktokRepository _tiktokRepository;
        private readonly ITiktokDomainService _tiktokDomainService;
        private readonly IRepository<MCNVietNamChannel> _mcnVietNamChannelRepository;
        public TikTokMCNGdlDomainService(IRepository<TikTokMCN> tikTokMcnRepository, IGroupRepository groupRepository, ITiktokRepository tiktokRepository, ITiktokDomainService tiktokDomainService,
            IRepository<MCNVietNamChannel> mcnVietNamChannelRepository)
        {
            _tikTokMcnRepository = tikTokMcnRepository;
            _groupRepository = groupRepository;
            _tiktokRepository = tiktokRepository;
            _tiktokDomainService = tiktokDomainService;
            _mcnVietNamChannelRepository = mcnVietNamChannelRepository;
        }

        public async Task<MultipleDataSourceChart<int>> GetMCNGDLChannelMonthlyInOutChart(DateTime fromDateTime, DateTime toDateTime)
        {
            var mcns = await _tikTokMcnRepository.GetListAsync(x => x.MCNType == TikTokMCNType.MCNGdl);
            var mcnIds = mcns.Select(x => x.Id);
            var gdlChannels = await _groupRepository.GetListAsync(x => x.McnId.HasValue && mcnIds.Contains(x.McnId.Value) && x.CreationTime >= fromDateTime);
            var chartStats = new MultipleDataSourceChart<int>() { ChartLabels = fromDateTime.EachMonth(toDateTime).Select(x => x.Date.ToString("MM-yyyy")).ToList() };
            var channelInCharts = new List<DataChartItemDto<int, string>>();
            var channelOutCharts = new List<DataChartItemDto<int, string>>();   
            foreach (var dateTime in fromDateTime.EachMonth(toDateTime))
            {
                var inChannels = gdlChannels.Count(x => x.IsActive && x.CreationTime.EqualMonthYear(dateTime));
                channelInCharts.Add
                (
                    new DataChartItemDto<int, string>()
                    {
                        Display = dateTime.ToString("MM-yyyy"),
                        Type = "TotalChannel",
                        Value = inChannels
                    }
                );
                var outCount = gdlChannels.Count(x => !x.IsActive && x.DeactivatedAt.HasValue && x.DeactivatedAt.Value.EqualMonthYear(dateTime));
                channelOutCharts.Add
                (
                    new DataChartItemDto<int, string>()
                    {
                        Display = dateTime.ToString("MM-yyyy"),
                        Type = "TotalChannel",
                        Value = outCount
                    }
                );
            }

            chartStats.MultipleDataCharts.Add(channelInCharts);
            chartStats.MultipleDataCharts.Add(channelOutCharts);

            return chartStats;
        }

        public async Task<MultipleDataSourceChart<int>> GetMCNGDLChannelWeekInOutChart(DateTime fromDateTime, DateTime toDateTime)
        {
            var mcns = await _tikTokMcnRepository.GetListAsync(x => x.MCNType == TikTokMCNType.MCNGdl);
            var mcnIds = mcns.Select(x => x.Id);        
            //Get 12 weeks
            var weekList = _tiktokDomainService.GetReportWeeks(fromDateTime,toDateTime, 12).ToList();
            var weekNameList = weekList.Select(x => $"W.{x.week} - {x.weekStart.ToString("MMM yy", CultureInfo.InvariantCulture)}").ToList();
            var gdlChannels = await _groupRepository.GetListAsync(x => x.McnId.HasValue && mcnIds.Contains(x.McnId.Value) && x.CreationTime >= fromDateTime);
            var chartStats = new MultipleDataSourceChart<int>() { ChartLabels = weekNameList};
            var channelInCharts = new List<DataChartItemDto<int, string>>();
            var channelOutCharts = new List<DataChartItemDto<int, string>>();
            
            foreach (var item in weekList)
            {
                var inChannels = gdlChannels.Count(x => x.IsActive && x.CreationTime >= item.weekStart & x.CreationTime < item.weekEnd );
                channelInCharts.Add
                (
                    new DataChartItemDto<int, string>()
                    {
                        Display = $"W.{item.week} - {item.weekStart.ToString("MMM yy", CultureInfo.InvariantCulture)}",
                        Type = "TotalChannel",
                        Value = inChannels
                    }
                );
                var outChannels = gdlChannels.Count(x => !x.IsActive && x.DeactivatedAt.HasValue && x.DeactivatedAt >=  item.weekStart & x.CreationTime < item.weekEnd);
                channelOutCharts.Add
                (
                    new DataChartItemDto<int, string>()
                    {
                        Display =  $"W.{item.week} - {item.weekStart.ToString("MMM yy", CultureInfo.InvariantCulture)}",
                        Type = "TotalChannel",
                        Value = outChannels
                    }
                );
            }
            
            chartStats.MultipleDataCharts.Add(channelInCharts);
            chartStats.MultipleDataCharts.Add(channelOutCharts);

            return chartStats;
        }

        public async Task<MultipleDataSourceChart<long>> GetMCNGDLViewAndCreatorChart(DateTime fromDateTime, DateTime toDateTime)
        {
            var chartStats = new MultipleDataSourceChart<long>() { ChartLabels = fromDateTime.EachMonth(toDateTime).Select(x => x.Date.ToString("MM-yyyy")).ToList() };
            var mcns = await _tikTokMcnRepository.GetListAsync(x => x.MCNType == TikTokMCNType.MCNGdl);
            var mcnIds = mcns.Select(x => x.Id);
            var mcnGdlTikTokChannels = await _groupRepository.GetListAsync
            (
                x => x.McnId.HasValue
                     && mcnIds.Contains(x.McnId.Value)
                     && x.GroupSourceType == GroupSourceType.Tiktok
                     && !x.IsDeleted 
                     && x.IsActive
            );
            var dataTotalView = new List<DataChartItemDto<long, string>>();
            var dataCreator = new List<DataChartItemDto<long, string>>();
            foreach (var dateTime in fromDateTime.EachMonth(toDateTime))
            {
                var startDateOfMonth = dateTime.GetFirstDateOfMonth();
                var endDateOfMonth = dateTime.GetLastDateOfMonth();
                var tiktokVideos = await _tiktokRepository.GetListAsync(x => x.CreatedDateTime.HasValue && x.CreatedDateTime >= startDateOfMonth && x.CreatedDateTime <= endDateOfMonth);
                var tiktokChannels = await _groupRepository.GetListAsync(_ => !_.IsDeleted && _.IsActive && _.GroupSourceType == GroupSourceType.Tiktok && _.CreationTime < endDateOfMonth);
                var totalViews = tiktokVideos.Sum(x => x.ViewCount);
                dataTotalView.Add
                (
                    new DataChartItemDto<long, string>()
                    {
                        Display = dateTime.ToString("MM-yyyy"),
                        Type = "TotalView",
                        Value = totalViews
                    }
                );

                dataCreator.Add
                (
                    new DataChartItemDto<long, string>()
                    {
                        Display = dateTime.ToString("MM-yyyy"),
                        Type = "TotalCreator",
                        Value = tiktokChannels.Count
                    }
                );
            }

            chartStats.MultipleDataCharts.Add(dataTotalView);
            chartStats.MultipleDataCharts.Add(dataCreator);

            return chartStats;
        }

        public async Task<PieChartDataSource<double>> GetMCNGDLChannelContractStatusChart()
        {
            var result = new PieChartDataSource<double>();
            var mcns = await _tikTokMcnRepository.GetListAsync(x => x.MCNType == TikTokMCNType.MCNGdl);
            var mcnIds = mcns.Select(x => x.Id);
            var contractStatusList = Enum.GetValues<TikTokContractStatus>().Where(x => x != TikTokContractStatus.NoSet).ToList();
            var channelHasContract = await _groupRepository.GetListAsync
            (
                x => x.McnId.HasValue
                     && mcnIds.Contains(x.McnId.Value)
                     && x.GroupSourceType == GroupSourceType.Tiktok
                     && contractStatusList.Contains(x.ContractStatus)
                     && !x.IsDeleted && x.IsActive
            );
            var totalChannel = channelHasContract.Count * 1.0;
            foreach (var contractStatus in contractStatusList)
            {
                var count = channelHasContract.Count(x => x.ContractStatus == contractStatus);
                if (count == 0)
                {
                    continue;
                }
                result.Items.Add
                (
                    new PieChartItem<double>()
                    {
                        Label = L[contractStatus.ToString()],
                        Value = count,
                        ValuePercent = totalChannel > 0 ?  Math.Round((count * 100) / totalChannel, 1) : 0
                    }
                );
            }

            return result;
        }

        public async Task<PieChartDataSource<double>> GetMCNGDLChannelCategoriesChart(List<GroupCategoryType> categoryTypes)
        {
            var temp = new PieChartDataSource<double>();
            var mcns = await _tikTokMcnRepository.GetListAsync(x => x.MCNType == TikTokMCNType.MCNGdl);
            var mcnIds = mcns.Select(x => x.Id);
            var channelHasContract = await _groupRepository.GetListAsync
            (
                x => x.McnId.HasValue
                     && mcnIds.Contains(x.McnId.Value)
                     && x.GroupSourceType == GroupSourceType.Tiktok
                     && x.ContractStatus != TikTokContractStatus.NoSet
                     && categoryTypes.Contains(x.GroupCategoryType)
                     && !x.IsDeleted && x.IsActive
            );
            var totalChannel = channelHasContract.Count * 1.0;
            foreach (var contractStatus in Enum.GetValues<GroupCategoryType>().Where(x => x != GroupCategoryType.FilterNoSelect && categoryTypes.Contains(x)))
            {
                var count = channelHasContract.Count(x => x.GroupCategoryType == contractStatus);
                temp.Items.Add
                (
                    new PieChartItem<double>()
                    {
                        Label = contractStatus.ToString(),
                        Value = count,
                        ValuePercent = totalChannel > 0 ? Math.Round((count*100)/totalChannel, 1) : 0
                    }
                );
            }

            if (temp.Items.Count <= 10)
            {
                return temp;
            }
            else
            {
                var result = new PieChartDataSource<double>();
                var other = new PieChartItem<double>()
                {
                    Label = "Other"
                };
                foreach (var item in temp.Items.OrderByDescending(_ => _.Value))
                {
                    if (result.Items.Count > 9)
                    {
                        other.Value += item.Value;
                        other.ValuePercent += item.ValuePercent;
                    }
                    else
                    {
                        result.Items.Add(item);
                    }
                }
                result.Items.Add(other);
                return result;
            }
        }

        public async Task<List<Tiktok>> GetTopMCNGDLVideos(GetTikTokVideosRequest request)
        {
            var mcns = await _tikTokMcnRepository.GetListAsync(x => x.MCNType == TikTokMCNType.MCNGdl);
            var mcnIds = mcns.Select(x => x.Id);
            var gdlChannels = await _groupRepository.GetListAsync(x => x.McnId.HasValue && mcnIds.Contains(x.McnId.Value) && !x.IsDeleted && x.IsActive);
            return (await _tiktokRepository.GetListExtendAsync(createdDateTimeMin: request.FromDate, createdDateTimeMax: request.ToDate, groupIds: gdlChannels.Select(_ => _.Id)))
                .OrderByDescending(_ => _.ViewCount)
                .Take(request.Count)
                .ToList();
        }

        public async Task<List<MCNVietNamChannelDto>> GetTopMCNVietNamChannel(DateTime fromDate, DateTime toDate, int count)
        {
            var tiktokChannelStats = await _mcnVietNamChannelRepository.GetListAsync(x => x.CreatedDateTime <= toDate && x.CreatedDateTime >= fromDate);

            return ObjectMapper.Map<List<MCNVietNamChannel>, List<MCNVietNamChannelDto>>(tiktokChannelStats.OrderByDescending(x => x.Followers).Take(count).ToList());
        }
    }
}