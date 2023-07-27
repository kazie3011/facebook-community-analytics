using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services.Emails;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class SendPostsEmailJob : BackgroundJobBase
    {
        private readonly IPostEmailDomainService _postEmailDomainService;

        public SendPostsEmailJob(IPostEmailDomainService postEmailDomainService)
        {
            _postEmailDomainService = postEmailDomainService;
        }

        protected override async Task DoExecute()
        {
            await _postEmailDomainService.SendUncrawlsPosts();
        }
    }
}