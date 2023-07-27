using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Integrations.Tiki.TikiAffiliates;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class SyncTikiOrdersJob : BackgroundJobBase
    {
        private readonly ITikiAffiliateDomainService _tikiAffiliateDomainService;

        public SyncTikiOrdersJob(ITikiAffiliateDomainService tikiAffiliateDomainService)
        {
            _tikiAffiliateDomainService = tikiAffiliateDomainService;
        }

        protected override async Task DoExecute()
        {
            await _tikiAffiliateDomainService.GetTikiOrders();
            await _tikiAffiliateDomainService.SyncTikiAffiliate();
        }
    }
}