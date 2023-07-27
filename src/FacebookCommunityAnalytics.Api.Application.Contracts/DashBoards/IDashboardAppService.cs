using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.DashBoards
{
    public interface IDashboardAppService : IApplicationService
    {
        Task<SaleChartDto> GetSaleChart(SaleChartApiRequest request);
        Task<List<DashboardUserDto>> GetSalePerson();
    }
}