using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class RetryNotAvailablePartnerPostsJob : BackgroundJobBase
    {
        private readonly ICrawlDomainService _crawlDomainService;

        public RetryNotAvailablePartnerPostsJob(ICrawlDomainService crawlDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _crawlDomainService = crawlDomainService;
        }

        protected override async Task DoExecute()
        {
            var (fromDate, toDate) = (DateTime.UtcNow.AddMonths(-1).Date, DateTime.UtcNow);
            await _crawlDomainService.InitUncrawledPosts(fromDate, toDate, true);
        }
    }
}