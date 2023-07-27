using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.TextTemplating;

namespace FacebookCommunityAnalytics.Api.WebBackgroundJob.BackgroundJobs
{
    public class DailyStaffEvaluationJob : BackgroundJobBase
    {
        private readonly IStaffEvaluationDomainService _staffEvaluationDomainService;

        public DailyStaffEvaluationJob(IStaffEvaluationDomainService staffEvaluationDomainService, ISmtpEmailSender emailSender, ITemplateRenderer templateRenderer) : base(emailSender, templateRenderer)
        {
            _staffEvaluationDomainService = staffEvaluationDomainService;
        }

        protected override async Task DoExecute()
        {
            await _staffEvaluationDomainService.DailyEvaluateStaffs();
        }
    }
}