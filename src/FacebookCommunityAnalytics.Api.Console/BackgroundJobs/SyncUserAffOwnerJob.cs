using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class SyncUserAffOwnerJob : BackgroundJobBase
    {
        private readonly IUserAffiliateDomainService _userAffiliateDomainService;

        public SyncUserAffOwnerJob(IUserAffiliateDomainService userAffiliateDomainService)
        {
            _userAffiliateDomainService = userAffiliateDomainService;
        }

        protected override async Task DoExecute()
        {
            var now = DateTime.UtcNow;
            await _userAffiliateDomainService.InitUserAffiliates(now.AddDays(-3), now);
        }
    }
}