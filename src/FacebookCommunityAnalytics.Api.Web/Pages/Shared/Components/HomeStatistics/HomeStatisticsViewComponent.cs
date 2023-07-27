using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.PartnerModule;
using FacebookCommunityAnalytics.Api.Statistics;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

namespace FacebookCommunityAnalytics.Api.Web.Pages.Shared.Components.HomeStatistics
{
    [Widget(RefreshUrl = "HomeWidgets/Statistics")]
    public class HomeStatisticsViewComponent : AbpViewComponent
    {
        private readonly IPartnerModuleAppService _partnerModuleAppService;

        public HomeStatisticsViewComponent(IPartnerModuleAppService partnerModuleAppService)
        {
            _partnerModuleAppService = partnerModuleAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync(GetGrowthCampaignChartsInput input)
        {
            if (input == null || !input.FromDateTime.HasValue ||  !input.ToDateTime.HasValue ) return View(new GrowthCampaignChartDto());
            var result = await _partnerModuleAppService.GetGrowthCampaignChartsAsync(input);
            return View(result);

        }
    }
}