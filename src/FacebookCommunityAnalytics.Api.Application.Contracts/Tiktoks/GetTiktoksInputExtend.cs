using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public class GetTiktoksInputExtend : PagedAndSortedResultRequestDto
    {
        public GetTiktoksInputExtend()
        {
            MaxMaxResultCount = int.MaxValue;
        }
        public string Search { get; set; }
        public DateTime? CreatedDateTimeMin { get; set; }
        public DateTime? CreatedDateTimeMax { get; set; }
        public bool SendEmail { get; set; }
        public Guid? CampaignId { get; set; }
        public string VideoId { get; set; }
        
        public TikTokMCNType? TikTokMcnType { get; set; }
    }

    public class GetTiktokWeeklyTotalFollowersRequest
    {
        public DateTime? TimeFrom { get; set; }
        public DateTime? TimeTo { get; set; }
        public TiktokOrderByType TiktokOrderByType { get; set; }
        public string OrderByWeekName { get; set; }
    }
    
    public class GetTiktokMonthlyTotalFollowersRequest
    {
        public DateTime? TimeFrom { get; set; }
        public DateTime? TimeTo { get; set; }
        public TiktokOrderByType TiktokOrderByType { get; set; }
        public string OrderByTimeTitle { get; set; }
    }
}