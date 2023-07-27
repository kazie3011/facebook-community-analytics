using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class SyncUserInfoJob : BackgroundJobBase
    {
        private readonly IUserDomainService _userDomainService;

        public SyncUserInfoJob(IUserDomainService userDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _userDomainService = userDomainService;
        }

        protected override async Task DoExecute()
        {
            await _userDomainService.SyncUserInfos();
        }
    }
}
