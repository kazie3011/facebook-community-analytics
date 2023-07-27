using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class RetryNotAvailablePostsJob : BackgroundJobBase
    {
        private readonly ICrawlDomainService _crawlDomainService;

        public RetryNotAvailablePostsJob(ICrawlDomainService crawlDomainService)
        {
            _crawlDomainService = crawlDomainService;
        }

        protected override async Task DoExecute()
        {
            var (fromDate, toDate) = GetPayrollDateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
            await _crawlDomainService.InitUncrawledPosts(fromDate, toDate, true);
        }
    }
}