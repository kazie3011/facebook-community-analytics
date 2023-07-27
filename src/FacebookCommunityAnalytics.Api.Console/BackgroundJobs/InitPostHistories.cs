using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Posts;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class InitPostHistories : BackgroundJobBase
    {
        private readonly IPostDomainService _postDomainService;

        public InitPostHistories(IPostDomainService postDomainService)
        {
            _postDomainService = postDomainService;
        }

        protected override async Task DoExecute()
        {
            await _postDomainService.InitPostHistories();
        }
    }
}