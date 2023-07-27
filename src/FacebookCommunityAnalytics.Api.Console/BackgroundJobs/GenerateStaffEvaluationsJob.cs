using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.StaffEvaluations;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class GenerateStaffEvaluationsJob: BackgroundJobBase
    {
        private readonly IStaffEvaluationDomainService _staffEvaluationDomainService;

        public GenerateStaffEvaluationsJob(IStaffEvaluationDomainService staffEvaluationDomainService)
        {
            _staffEvaluationDomainService = staffEvaluationDomainService;
        }

        protected override async Task DoExecute()
        {
            await _staffEvaluationDomainService.GenerateStaffEvaluations(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
        }
    }
}
