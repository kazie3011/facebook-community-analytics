using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public class MCNVietNamChannel : AuditedEntity<Guid>
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public decimal Followers { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}