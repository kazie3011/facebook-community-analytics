using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AffiliateStats;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class SyncGeneralAffStatJob : BackgroundJobBase
    {
        private readonly IAffiliateStatsDomainService _affiliateStatsDomainService;

        public SyncGeneralAffStatJob(IAffiliateStatsDomainService affiliateStatsDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
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