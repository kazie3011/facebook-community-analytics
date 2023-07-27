using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.StaffEvaluations;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class DailyStaffEvaluationJob : BackgroundJobBase
    {
        private readonly IStaffEvaluationDomainService _staffEvaluationDomainService;

        public DailyStaffEvaluationJob(IStaffEvaluationDomainService staffEvaluationDomainService)
        {
            _staffEvaluationDomainService = staffEvaluationDomainService;
        }

        protected override async Task DoExecute()
        {
            await _staffEvaluationDomainService.DailyEvaluateStaffs();
        }
    }
}