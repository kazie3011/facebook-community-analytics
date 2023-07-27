using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.Integrations.Tiktok
{
    public class TiktokStat : AuditedEntity<Guid>
    {
        public string Hashtag { get; set; }
        public long Count { get; set; }
        public DateTime Date { get; set; }
    }
    public class TiktokVideoStat : Entity<Guid>
    {
        public string Hashtag { get; set; }
        public string ChannelId { get; set; }
        public string VideoId { get; set; }
        
        public int Like { get; set; }
        public int Comment { get; set; }
        public int Share { get; set; }
        public long ViewCount { get; set; }
        public DateTime Date { get; set; }
    }
}