using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AffiliateConversions;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Services;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class SyncShopinessConversionsJob : BackgroundJobBase
    {
        private readonly IAffiliateConversionDomainService _conversionDomainService;
        private readonly IUserAffiliateDomainService _userAffiliateDomainService;

        public SyncShopinessConversionsJob(IAffiliateConversionDomainService conversionDomainService,
            IUserAffiliateDomainService userAffiliateDomainService,
            ISmtpEmailSender emailSender,
            ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _conversionDomainService = conversionDomainService;
            _userAffiliateDomainService = userAffiliateDomainService;
        }

        protected override async Task DoExecute()
        {
            var now = DateTime.UtcNow;
            await _userAffiliateDomainService.InitUserAffiliates(now.AddDays(-3), now);
            await Task.Delay(3000);

            await _conversionDomainService.SyncAffConversions(now.AddDays(-3), now, AffiliateOwnershipType.GDL);
            await Task.Delay(3000);
            
            await _conversionDomainService.SyncAffConversions(now.AddDays(-3), now, AffiliateOwnershipType.HappyDay);
        }
    }
}