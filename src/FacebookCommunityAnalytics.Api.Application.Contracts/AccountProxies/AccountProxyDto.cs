using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.AccountProxies
{
    public class AccountProxyDto : FullAuditedEntityDto<Guid>
    {
        public string Description { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? ProxyId { get; set; }
        public bool IsCrawling { get; set; }
        public DateTime CrawledAt { get; set; }
    }
}