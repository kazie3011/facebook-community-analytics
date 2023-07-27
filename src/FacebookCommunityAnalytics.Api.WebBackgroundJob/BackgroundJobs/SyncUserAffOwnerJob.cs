using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class SyncUserAffOwnerJob : BackgroundJobBase
    {
        private readonly IUserAffiliateDomainService _userAffiliateDomainService;

        public SyncUserAffOwnerJob(IUserAffiliateDomainService userAffiliateDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
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