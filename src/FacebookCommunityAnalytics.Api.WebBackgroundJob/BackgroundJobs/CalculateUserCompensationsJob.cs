using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.UserCompensations;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class CalculateUserCompensationsJob : BackgroundJobBase
    {
        private readonly IUserCompensationDomainService _userCompensationDomainService;

        public CalculateUserCompensationsJob(IUserCompensationDomainService userCompensationDomainService,
            ISmtpEmailSender emailSender,
            ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _userCompensationDomainService = userCompensationDomainService;
        }

        protected override async Task DoExecute()
        {
            var now = DateTime.UtcNow;
            await _userCompensationDomainService.CalculateCompensations(true, true, isHappyDay: false, null, now.Month, now.Year);
            await _userCompensationDomainService.CalculateCompensations(true, true, isHappyDay: true, null, now.Month, now.Year);
        }
    }
}