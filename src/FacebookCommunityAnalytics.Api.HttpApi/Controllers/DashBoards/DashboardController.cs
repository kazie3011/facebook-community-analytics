using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.DashBoards;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.DashBoards
{
    [RemoteService]
    [Area("app")]
    [ControllerName("DashBoard")]
    [Route("api/app/dashboard")]
    public class DashboardController : AbpController, IDashboardAppService
    {
        private readonly IDashboardAppService _dashboardAppService;

        public DashboardController(IDashboardAppService dashboardAppService)
        {
            _dashboardAppService = dashboardAppService;
        }

        [HttpGet]
        [Route("get-sale-chart")]
        public Task<SaleChartDto> GetSaleChart(SaleChartApiRequest request)
        {
            return _dashboardAppService.GetSaleChart(request);
        }

        [HttpGet]
        [Route("get-sale-users")]
        public Task<List<DashboardUserDto>> GetSalePerson()
        {
            return _dashboardAppService.GetSalePerson();
        }
    }
}