using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class RetryNotAvailablePostsJob : BackgroundJobBase
    {
        private readonly ICrawlDomainService _crawlDomainService;

        public RetryNotAvailablePostsJob(ICrawlDomainService crawlDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _crawlDomainService = crawlDomainService;
        }

        protected override async Task DoExecute()
        {
            var (fromDate, toDate) = GetPayrollDateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
            await _crawlDomainService.InitUncrawledPosts(fromDate, toDate, true);
        }
    }
}