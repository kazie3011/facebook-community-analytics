using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AffiliateConversions;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class SyncShopinessStatisticAffJob : BackgroundJobBase
    {
        private readonly IAffiliateConversionDomainService _affiliateConversionDomainService;

        public SyncShopinessStatisticAffJob(IAffiliateConversionDomainService affiliateConversionDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _affiliateConversionDomainService = affiliateConversionDomainService;
        }

        protected override async Task DoExecute()
        {
            var now = DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour);

            await _affiliateConversionDomainService.SyncAffStats(3, AffiliateOwnershipType.GDL, now.AddHours(-2), now);

            await Task.Delay(3000);
            
            await _affiliateConversionDomainService.SyncAffStats(3, AffiliateOwnershipType.HappyDay, now.AddHours(-2), now);
        }
    }
}