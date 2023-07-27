using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Statistics;

namespace FacebookCommunityAnalytics.Api.UserAffiliates
{
    public class UserProfileStats
    {

    }
    
    public class UserProfileChartResponse
    {
        public UserProfileChartResponse()
        {
            CountPostsByTypeChartData = new List<DataChartItemDto<int, PostContentType>>();
            SaleChartData = new List<DataChartItemDto<decimal, string>>();
            TikTokChartData = new List<DataChartItemDto<int, string>>();
        }
        public List<DataChartItemDto<int,PostContentType>> CountPostsByTypeChartData { get; set; }
        public List<DataChartItemDto<decimal,string>> SaleChartData { get; set; } 
        public List<DataChartItemDto<int,string>> TikTokChartData { get; set; } 
    }

    public class UserProfileChartRequest
    {
        public Guid TeamId { get; set; }
        public Guid UserId { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
    }
}