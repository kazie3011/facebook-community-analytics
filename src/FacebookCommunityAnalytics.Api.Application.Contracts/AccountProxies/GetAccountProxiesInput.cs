using Volo.Abp.Application.Dtos;
using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.AccountProxies
{
    public class GetAccountProxiesInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Description { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? ProxyId { get; set; }

        public GetAccountProxiesInput()
        {
        }
    }

    public class GetCrawlAccountProxiesRequest
    {
        public AccountType AccountType { get; set; }
    }

    public class UnlockCrawlAccountRequest
    {
        public List<Guid> AccountProxyIds { get; set; }
        public AccountType AccountType { get; set; }
    }

    public class GetAccountsRequest
    {
        public AccountStatus AccountStatus { get; set; }
    }
}