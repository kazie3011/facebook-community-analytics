using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.Proxies;

namespace FacebookCommunityAnalytics.Api.AccountProxies
{
    public class AccountProxyWithNavigationProperties
    {
        public AccountProxy AccountProxy { get; set; }

        public Account Account { get; set; }
        public Proxy Proxy { get; set; }
        
    }
}