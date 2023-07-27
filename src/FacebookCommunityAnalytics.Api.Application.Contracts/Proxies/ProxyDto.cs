using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Proxies
{
    public class ProxyDto : FullAuditedEntityDto<Guid>
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Protocol { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime LastPingDateTime { get; set; }
        public bool IsActive { get; set; }
    }
}