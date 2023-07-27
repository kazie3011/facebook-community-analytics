using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class CleanUpDataJob : BackgroundJobBase
    {
        private readonly IClearDataDomainService _clearDataDomainService;
        public CleanUpDataJob(IClearDataDomainService clearDataDomainService)
        {
            _clearDataDomainService = clearDataDomainService;
        }
        
        protected override async Task DoExecute()
        {
            await _clearDataDomainService.CleanUpData();

            
        }
    
    }
}