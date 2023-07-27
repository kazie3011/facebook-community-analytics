using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services.Emails;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class SendTiktokEmailsJob : BackgroundJobBase
    {
        private readonly ITiktokEmailDomainService _tiktokEmailDomainService;

        public SendTiktokEmailsJob(ITiktokEmailDomainService tiktokEmailDomainService)
        {
            _tiktokEmailDomainService = tiktokEmailDomainService;
        }

        protected override async Task DoExecute()
        {
            await _tiktokEmailDomainService.SendTiktokDaily();
        }
    }
}