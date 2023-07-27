using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Statistics;

namespace FacebookCommunityAnalytics.Api.DashBoards
{
    public class SaleChartDto
    {
        public SaleChartDto()
        {
            ChartLabels = new List<string>();
            TotalBarCharts = new List<DataChartItemDto<decimal?, string>>();
            PartialPaymentBarCharts = new List<DataChartItemDto<decimal?, string>>();
            RemainingPaymentBarCharts = new List<DataChartItemDto<decimal?, string>>();
            Contracts = new List<ContractWithNavigationPropertiesDto>();
        }
        public List<string> ChartLabels { get; set; }
        public List<DataChartItemDto<decimal?,string>> TotalBarCharts { get; set; } 
        public List<DataChartItemDto<decimal?,string>> PartialPaymentBarCharts { get; set; } 
        public List<DataChartItemDto<decimal?,string>> RemainingPaymentBarCharts { get; set; } 
        public List<ContractWithNavigationPropertiesDto> Contracts { get; set; }
    }

    public class SaleChartApiRequest
    {
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public Guid? SalePersonId { get; set; }
    }
}