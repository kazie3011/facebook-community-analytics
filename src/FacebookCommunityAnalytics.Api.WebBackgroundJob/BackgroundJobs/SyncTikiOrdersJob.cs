using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Integrations.Tiki.TikiAffiliates;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class SyncTikiOrdersJob : BackgroundJobBase
    {
        private readonly ITikiAffiliateDomainService _tikiAffiliateDomainService;

        public SyncTikiOrdersJob(ITikiAffiliateDomainService tikiAffiliateDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
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