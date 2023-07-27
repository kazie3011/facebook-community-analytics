using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.TrendingDetails
{
    public class TrendingDetailDto : AuditedEntity<Guid>
    {
        public int View { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public int Rank { get; set; }
        public int Increase { get; set; }
    }

    public class TrendingDetailApiRequest
    {
        public List<TrendingDetailDto> TrendingDetails { get; set; }
    }
}