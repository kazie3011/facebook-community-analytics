using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AffiliateStats;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class SyncGeneralAffStatJob : BackgroundJobBase
    {
        private readonly IAffiliateStatsDomainService _affiliateStatsDomainService;

        public SyncGeneralAffStatJob(IAffiliateStatsDomainService affiliateStatsDomainService)
        {
            _affiliateStatsDomainService = affiliateStatsDomainService;
        }

        protected override async Task DoExecute()
        {
            await _affiliateStatsDomainService.SyncGeneralAffStats(new SyncGeneralAffStatApiRequest
            {
                FromDateTime = DateTime.UtcNow.AddDays(-3).Date,
                ToDateTime = DateTime.UtcNow.AddDays(-1).Date,
            });
        }
    }
}