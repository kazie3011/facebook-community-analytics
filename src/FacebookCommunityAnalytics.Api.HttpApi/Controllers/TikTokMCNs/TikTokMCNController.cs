using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.TikTokMCNs;
using FacebookCommunityAnalytics.Api.Tiktoks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.TikTokMCNs
{
    [RemoteService]
    [Area("app")]
    [ControllerName("TikTokMCN")]
    [Route("api/app/tiktokmcns")]
    public class TikTokMCNController : AbpController, ITikTokMCNAppService, ITikTokMCNGdlAppService
    {
        private readonly ITikTokMCNAppService _tikTokMcnAppService;
        private readonly ITikTokMCNGdlAppService _tikTokMcnGdlAppService;

        public TikTokMCNController(ITikTokMCNAppService tikTokMcnAppService, ITikTokMCNGdlAppService tikTokMcnGdlAppService)
        {
            _tikTokMcnAppService = tikTokMcnAppService;
            _tikTokMcnGdlAppService = tikTokMcnGdlAppService;
        }

        [HttpGet]
        [Route("get-list-async")]
        public virtual Task<PagedResultDto<TikTokMCNDto>> GetListAsync(GetTikTokMCNsInput input)
        {
            return _tikTokMcnAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<TikTokMCNDto> GetAsync(Guid id)
        {
            return _tikTokMcnAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<TikTokMCNDto> CreateAsync(CreateUpdateTikTokMCNDto input)
        {
            return _tikTokMcnAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<TikTokMCNDto> UpdateAsync(Guid id, CreateUpdateTikTokMCNDto input)
        {
            return _tikTokMcnAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _tikTokMcnAppService.DeleteAsync(id);
        }

        [HttpGet("get-list-hashtags")]
        public Task<List<string>> GetHashtags()
        {
            return _tikTokMcnAppService.GetHashtags();
        }

        [HttpGet("get-weekly-gdl-in-out-chart")]
        public Task<MultipleDataSourceChart<int>> GetWeeklyChannelInOutChart()
        {
            return _tikTokMcnGdlAppService.GetWeeklyChannelInOutChart();
        }
        
        [HttpGet("get-monthly-gdl-in-out-chart")]
        public Task<MultipleDataSourceChart<int>> GetMonthlyChannelInOutChart()
        {
            return _tikTokMcnGdlAppService.GetMonthlyChannelInOutChart();
        }
        
        [HttpGet("get-gdl-view-creator-chart")]
        public Task<MultipleDataSourceChart<long>> GetViewAndCreatorChart()
        {
            return _tikTokMcnGdlAppService.GetViewAndCreatorChart();
        }
        
        [HttpGet("get-gdl-contract-status-chart")]
        public Task<PieChartDataSource<double>> GetChannelContractStatusChart()
        {
            return _tikTokMcnGdlAppService.GetChannelContractStatusChart();
        }
        [HttpGet("get-gdl-categories-chart")]
        public Task<PieChartDataSource<double>> GetChannelCategoriesChart(List<GroupCategoryType> categoryTypes)
        {
            return _tikTokMcnGdlAppService.GetChannelCategoriesChart(categoryTypes);
        }

        [HttpGet("get-top-mcn-gdl-tiktok-videos")]
        public Task<List<TiktokDto>> GetTopMCNGDLVideos(GetTikTokVideosRequest request)
        {
            return _tikTokMcnGdlAppService.GetTopMCNGDLVideos(request);
        }
        
        [HttpGet("get-top-mcn-vn-channel")]
        public Task<List<MCNVietNamChannelDto>> GetTopMCNVietNamChannel(DateTime fromDate, DateTime toDate, int count)
        {
            return _tikTokMcnGdlAppService.GetTopMCNVietNamChannel(fromDate, toDate, count);
        }
    }
}