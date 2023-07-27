using System.Collections.Generic;
using System.Threading.Tasks;
using ChartJSCore.Helpers;
using ChartJSCore.Models;
using FacebookCommunityAnalytics.Api.PartnerModule;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

namespace FacebookCommunityAnalytics.Api.Web.Pages.Shared.Components.GrowthChart
{
    [Widget(RefreshUrl = "HomeWidgets/GrowthChart")]
    public class GrowthChartViewComponent : AbpViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
        
       
    }
}