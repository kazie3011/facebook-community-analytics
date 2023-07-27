using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.UserCompensations;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class CalculateUserCompensationsJob : BackgroundJobBase
    {
        private readonly IUserCompensationDomainService _userCompensationDomainService;

        public CalculateUserCompensationsJob(IUserCompensationDomainService userCompensationDomainService)
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