using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class InitCampaignPostsJob : BackgroundJobBase
    {
        private readonly ICrawlDomainService _crawlDomainService;

        public InitCampaignPostsJob(ICrawlDomainService crawlDomainService)
        {
            _crawlDomainService = crawlDomainService;
        }

        protected override async Task DoExecute()
        {
            await _crawlDomainService.InitCampaignPosts();
        }
    }
}