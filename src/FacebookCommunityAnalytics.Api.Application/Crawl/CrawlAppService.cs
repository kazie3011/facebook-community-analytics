using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AccountProxies;
using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.TrendingDetails;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml.ConditionalFormatting;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Crawl
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.Crawl.Default)]
    public class CrawlAppService : ApiAppService, ICrawlAppService
    {
        private readonly ICrawlDomainService _crawlDomainService;

        public CrawlAppService(ICrawlDomainService crawlDomainService)
        {
            _crawlDomainService = crawlDomainService;
        }
        public async Task PostCrawlResult(SaveCrawlResultApiRequest result)
        {
            await _crawlDomainService.SaveCrawlResult(result);
        }

        public async Task<SaveAutoCrawlResultApiResponse> PostAutoCrawlResult(SaveAutoCrawlResultApiRequest apiRequest)
        {
            return await _crawlDomainService.SaveAutoCrawlResult(apiRequest);
        }

        public async Task<int> InitUncrawledPosts(DateTime fromDate, DateTime toDate, bool initNewPosts = false)
        {
            return await _crawlDomainService.InitUncrawledPosts(fromDate, toDate, initNewPosts);
        }

        public async Task<int> InitCampaignPosts(List<string> campaignCodes)
        {
            return await _crawlDomainService.InitCampaignPosts(campaignCodes);
        }

        public virtual async Task<UncrawledPostsApiResponse> GetUncrawledPosts(UncrawledPostsApiRequest apiRequest)
        {
            return await _crawlDomainService.GetUncrawledPosts(apiRequest);
        }

        public virtual async Task<UncrawledCampaignPostsApiResponse> GetUncrawledCampaignPosts(UncrawledCampaignPostsApiRequest apiRequest)
        {
            return await _crawlDomainService.GetUncrawledCampaignPosts(apiRequest);
        }

        public async Task<GetUncrawledGroupUserApiResponse> GetUncrawledGroupUsers(GetUncrawledGroupUserApiRequest apiRequest)
        {
            return await _crawlDomainService.GetUncrawledGroupUsers(apiRequest);
        }

        public async Task<GetUncrawledGroupApiResponse> GetUncrawledGroups(GetUncrawledGroupApiRequest apiRequest)
        {
            return await _crawlDomainService.GetUncrawledGroups(apiRequest);
        }

        public async Task<UncrawledPostsApiResponse> GetUnavailablePosts(UncrawledPostsApiRequest apiRequest)
        {
            return await _crawlDomainService.GetUnavailablePosts(apiRequest);
        }
        
        public async Task<List<AccountProxyWithNavigationPropertiesDto>> GetAccountProxies(GetCrawlAccountProxiesRequest request)
        {
            var entities = await _crawlDomainService.GetAccountProxies(request);
            var dto = ObjectMapper.Map<List<AccountProxyWithNavigationProperties>, List<AccountProxyWithNavigationPropertiesDto>>(entities);

            return dto;
        }
        
        public async Task RebindAccountProxies()
        {
            await _crawlDomainService.RebindAccountProxies();
        }
        
        public async Task ResetAccountsCrawlStatus(UnlockCrawlAccountRequest unlockCrawlAccountRequest)
        {
            await _crawlDomainService.UnlockCrawlAccounts(unlockCrawlAccountRequest);
        }

        public async Task<List<AccountDto>> GetAccounts(GetAccountsRequest request)
        {
            return await _crawlDomainService.GetAccounts(request);
        }
    }
}