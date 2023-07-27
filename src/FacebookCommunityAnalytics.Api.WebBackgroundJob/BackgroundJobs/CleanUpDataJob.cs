using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Dev;
using FacebookCommunityAnalytics.Api.Services;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class CleanUpDataJob : BackgroundJobBase
    {
        private readonly IClearDataDomainService _clearDataDomainService;
        public CleanUpDataJob(IClearDataDomainService clearDataDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _clearDataDomainService = clearDataDomainService;
        }
        
        protected override async Task DoExecute()
        {
            await _clearDataDomainService.CleanUpData(3);
        }
    
    }
}