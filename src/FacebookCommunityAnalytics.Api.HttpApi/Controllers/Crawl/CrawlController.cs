using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Crawl;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AccountProxies;
using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.TrendingDetails;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.Crawl
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Crawl")]
    [Route("api/app/crawl")]
    public class CrawlController : AbpController, ICrawlAppService
    {
        private readonly ICrawlAppService _crawlAppService;

        public CrawlController(ICrawlAppService crawlAppService)
        {
            _crawlAppService = crawlAppService;
        }

        [HttpGet("unavailable")]
        public Task<UncrawledPostsApiResponse> GetUnavailablePosts([FromQuery] UncrawledPostsApiRequest apiRequest)
        {
            return _crawlAppService.GetUnavailablePosts(apiRequest);
        }

        [HttpGet("uncrawled")]
        public virtual Task<UncrawledPostsApiResponse> GetUncrawledPosts([FromQuery] UncrawledPostsApiRequest apiRequest)
        {
            return _crawlAppService.GetUncrawledPosts(apiRequest);
        }
        
        [HttpGet("uncrawled-campaign-posts")]
        public virtual Task<UncrawledCampaignPostsApiResponse> GetUncrawledCampaignPosts([FromQuery] UncrawledCampaignPostsApiRequest apiRequest)
        {
            return _crawlAppService.GetUncrawledCampaignPosts(apiRequest);
        }

        [HttpGet("uncrawled-groups")]
        public Task<GetUncrawledGroupApiResponse> GetUncrawledGroups([FromQuery] GetUncrawledGroupApiRequest apiRequest)
        {
            return _crawlAppService.GetUncrawledGroups(apiRequest);
        }

        [HttpGet("uncrawled-group-users")]
        public Task<GetUncrawledGroupUserApiResponse> GetUncrawledGroupUsers([FromQuery] GetUncrawledGroupUserApiRequest apiRequest)
        {
            return _crawlAppService.GetUncrawledGroupUsers(apiRequest);
        }

        [HttpPost("crawl-result")]
        public Task PostCrawlResult(SaveCrawlResultApiRequest result)
        {
            return _crawlAppService.PostCrawlResult(result);
        }

        [HttpPost("auto-crawl-result")]
        public Task<SaveAutoCrawlResultApiResponse> PostAutoCrawlResult(SaveAutoCrawlResultApiRequest apiRequest)
        {
            return _crawlAppService.PostAutoCrawlResult(apiRequest);
        }

        [HttpGet("uncrawled-init")]
        public Task<int> InitUncrawledPosts(DateTime fromDate, DateTime toDate, bool initNewPosts = false)
        {
            return _crawlAppService.InitUncrawledPosts(fromDate, toDate, initNewPosts);
        }

        [HttpPost("init-campaign-posts")]
        public Task<int> InitCampaignPosts(List<string> campaignCodes)
        {
            return _crawlAppService.InitCampaignPosts(campaignCodes);
        }

        [HttpGet("rebind-accounts")]
        public Task RebindAccountProxies()
        {
            return _crawlAppService.RebindAccountProxies();
        }

        [HttpGet("account-proxies")]
        public Task<List<AccountProxyWithNavigationPropertiesDto>> GetAccountProxies(GetCrawlAccountProxiesRequest request)
        {
            return _crawlAppService.GetAccountProxies(request);
        }

        [HttpGet("accounts")]
        public Task<List<AccountDto>> GetAccounts(GetAccountsRequest request)
        {
            return _crawlAppService.GetAccounts(request);
        }

        [HttpPost("reset-accounts-crawl-status")]
        public Task ResetAccountsCrawlStatus(UnlockCrawlAccountRequest unlockCrawlAccountRequest)
        {
            return _crawlAppService.ResetAccountsCrawlStatus(unlockCrawlAccountRequest);
        }
    }
}