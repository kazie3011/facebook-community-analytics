using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public class TrendingDetail : AuditedEntity<Guid>
    {
        public int View { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}