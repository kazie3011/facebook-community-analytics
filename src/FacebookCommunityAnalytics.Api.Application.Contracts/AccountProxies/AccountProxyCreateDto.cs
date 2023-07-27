using System;
using System.ComponentModel.DataAnnotations;

namespace FacebookCommunityAnalytics.Api.AccountProxies
{
    public class AccountProxyCreateDto
    {
        public string Description { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? ProxyId { get; set; }
    }
}