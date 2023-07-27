using System;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public class TikTokMCN : AuditedEntity<Guid>
    {
        public string Name { get; set; }
        public string HashTag { get; set; }
        public TikTokMCNType MCNType { get; set; }
    }
}