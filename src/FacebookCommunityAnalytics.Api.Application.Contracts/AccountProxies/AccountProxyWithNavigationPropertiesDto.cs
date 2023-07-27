using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.Proxies;

using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.AccountProxies
{
    public class AccountProxyWithNavigationPropertiesDto
    {
        public AccountProxyDto AccountProxy { get; set; }

        public AccountDto Account { get; set; }
        public ProxyDto Proxy { get; set; }

    }
}