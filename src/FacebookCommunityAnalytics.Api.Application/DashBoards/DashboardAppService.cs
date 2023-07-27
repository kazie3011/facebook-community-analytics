using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.DashBoards
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.Groups.Default)]
    public class DashboardAppService : ApplicationService, IDashboardAppService
    {
        private readonly IDashboardDomainService _dashboardDomainService;

        public DashboardAppService(IDashboardDomainService dashboardDomainService)
        {
            _dashboardDomainService = dashboardDomainService;
        }

        public async Task<SaleChartDto> GetSaleChart(SaleChartApiRequest request)
        {
            return await _dashboardDomainService.GetSaleChart(request);
        }

        public async Task<List<DashboardUserDto>> GetSalePerson()
        {
            return await _dashboardDomainService.GetSalePerson();
        }
    }
}