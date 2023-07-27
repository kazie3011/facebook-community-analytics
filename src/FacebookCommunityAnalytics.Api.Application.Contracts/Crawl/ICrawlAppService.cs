using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AccountProxies;
using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.TrendingDetails;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Crawl
{
    public interface ICrawlAppService : IApplicationService
    {
        Task<UncrawledPostsApiResponse> GetUnavailablePosts(UncrawledPostsApiRequest apiRequest);
        Task<UncrawledPostsApiResponse> GetUncrawledPosts(UncrawledPostsApiRequest apiRequest);
        Task<UncrawledCampaignPostsApiResponse> GetUncrawledCampaignPosts(UncrawledCampaignPostsApiRequest apiRequest);
        Task<GetUncrawledGroupUserApiResponse> GetUncrawledGroupUsers(GetUncrawledGroupUserApiRequest apiRequest);
        Task PostCrawlResult(SaveCrawlResultApiRequest result);
        Task<SaveAutoCrawlResultApiResponse> PostAutoCrawlResult(SaveAutoCrawlResultApiRequest apiRequest);
        Task<GetUncrawledGroupApiResponse> GetUncrawledGroups(GetUncrawledGroupApiRequest apiRequest);
        Task<List<AccountProxyWithNavigationPropertiesDto>> GetAccountProxies(GetCrawlAccountProxiesRequest request);
        Task<int> InitUncrawledPosts(DateTime fromDate, DateTime toDate, bool initNewPosts = false);
        Task<int> InitCampaignPosts(List<string> campaignCodes);
        Task RebindAccountProxies();
        Task ResetAccountsCrawlStatus(UnlockCrawlAccountRequest unlockCrawlAccountRequest);
        Task<List<AccountDto>> GetAccounts(GetAccountsRequest request);
    }
}