using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AffiliateConversions;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Services;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class SyncShopinessStatisticAffJob : BackgroundJobBase
    {
        private readonly IAffiliateConversionDomainService _affiliateConversionDomainService;
        private readonly IUserAffiliateDomainService _userAffiliateDomainService;

        public SyncShopinessStatisticAffJob(IAffiliateConversionDomainService affiliateConversionDomainService, IUserAffiliateDomainService userAffiliateDomainService)
        {
            _affiliateConversionDomainService = affiliateConversionDomainService;
            _userAffiliateDomainService = userAffiliateDomainService;
        }

        protected override async Task DoExecute()
        {
            var now = DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour);
            
            System.Console.WriteLine($"------------------------------AFFILIATE SYNC CLICK--------------------------");
            System.Console.WriteLine($"From Time: {now.AddHours(-2)}");
            System.Console.WriteLine($"To Time: {now}");
            System.Console.WriteLine($"-----------------------Init User Affiliates ----------------------");
            
            await _userAffiliateDomainService.InitUserAffiliates(now.AddDays(-1).Date, now);
            
            System.Console.WriteLine($"------------------------Owner ship Type: {AffiliateOwnershipType.GDL}---------------------------");

            await _affiliateConversionDomainService.SyncAffStats(3, AffiliateOwnershipType.GDL, now.AddHours(-2), now);

            await Task.Delay(3000);
            
            System.Console.WriteLine($"------------------------Owner ship Type: {AffiliateOwnershipType.HappyDay}---------------------------");
            
            await _affiliateConversionDomainService.SyncAffStats(3, AffiliateOwnershipType.HappyDay, now.AddHours(-2), now);
        }
    }
}