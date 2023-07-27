using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FluentDateTime;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.TikTokMCNs
{
    [RemoteService(isEnabled: false)]
    [Authorize(ApiPermissions.Tiktok.Default)]
    public class TikTokMCNGdlAppService : ApplicationService, ITikTokMCNGdlAppService
    {
        private readonly ITikTokMCNGdlDomainService _tikTokMcnGdlDomainService;
        public TikTokMCNGdlAppService(ITikTokMCNGdlDomainService tikTokMcnGdlDomainService)
        {
            _tikTokMcnGdlDomainService = tikTokMcnGdlDomainService;
        }

        /// <summary>
        /// Hiển thị channel IN/OUT
        /// Hiển thị data theo từng tháng và cho 12 tháng gần nhất.
        /// </summary>
        /// <returns></returns>
        public Task<MultipleDataSourceChart<int>> GetMonthlyChannelInOutChart()
        {
            DateTime endDateTime = DateTime.UtcNow.LastDayOfMonth();
            DateTime startDateTime = endDateTime.AddMonths(-11).FirstDayOfMonth();

            return _tikTokMcnGdlDomainService.GetMCNGDLChannelMonthlyInOutChart(startDateTime, endDateTime);
        }

        public Task<MultipleDataSourceChart<int>> GetWeeklyChannelInOutChart()
        {
            DateTime endDateTime = DateTime.UtcNow.Date;

            // Hiển thị data cho 12 tuần gần nhất.(11 tuần + tuần hiện tại)
            DateTime startDateTime = endDateTime.AddDays(-77).Date;

            return _tikTokMcnGdlDomainService.GetMCNGDLChannelWeekInOutChart(startDateTime, endDateTime);
        }

        public Task<MultipleDataSourceChart<long>> GetViewAndCreatorChart()
        {
            var endDateTime = DateTime.UtcNow.LastDayOfMonth();
            var startDateTime = endDateTime.AddMonths(-11).FirstDayOfMonth();
            return _tikTokMcnGdlDomainService.GetMCNGDLViewAndCreatorChart(startDateTime, endDateTime);
        }

        public Task<PieChartDataSource<double>> GetChannelContractStatusChart()
        {
            return _tikTokMcnGdlDomainService.GetMCNGDLChannelContractStatusChart();
        }

        public Task<PieChartDataSource<double>> GetChannelCategoriesChart(List<GroupCategoryType> categoryTypes)
        {
            return _tikTokMcnGdlDomainService.GetMCNGDLChannelCategoriesChart(categoryTypes);
        }

        public async Task<List<TiktokDto>> GetTopMCNGDLVideos(GetTikTokVideosRequest request)
        {
            var items = ObjectMapper.Map<List<Tiktok>, List<TiktokDto>>(await _tikTokMcnGdlDomainService.GetTopMCNGDLVideos(request));
            var index = 0;
            foreach (var item in items)
            {
                index++;
                item.Index = index;
            }

            return items;
        }

        public async Task<List<MCNVietNamChannelDto>> GetTopMCNVietNamChannel(DateTime fromDate, DateTime toDate, int count)
        {
            return await _tikTokMcnGdlDomainService.GetTopMCNVietNamChannel(fromDate, toDate, count);
        }
    }
}