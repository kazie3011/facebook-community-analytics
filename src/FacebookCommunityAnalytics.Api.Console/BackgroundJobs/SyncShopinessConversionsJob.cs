using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AffiliateConversions;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class SyncShopinessConversionsJob : BackgroundJobBase
    {
        private readonly IAffiliateConversionDomainService _conversionDomainService;

        public SyncShopinessConversionsJob(IAffiliateConversionDomainService conversionDomainService)
        {
            _conversionDomainService = conversionDomainService;
        }

        protected override async Task DoExecute()
        {
            try
            {
                var now = DateTime.UtcNow;
                var fromDate = now.AddDays(-3);
                
                System.Console.WriteLine($"------------------------------AFFILIATE SYNC CONVERSION--------------------------");
                System.Console.WriteLine($"From Time: {fromDate}");
                System.Console.WriteLine($"To Time: {now}");
                System.Console.WriteLine($"Owner ship Type: {AffiliateOwnershipType.GDL}");

                await _conversionDomainService.SyncAffConversions(fromDate, now, AffiliateOwnershipType.GDL);
                
                await Task.Delay(3000);
            
                await _conversionDomainService.SyncAffConversions(fromDate, now, AffiliateOwnershipType.HappyDay);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw;
            }
        }
    }
}