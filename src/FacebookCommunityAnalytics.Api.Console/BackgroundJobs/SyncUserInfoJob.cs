using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Services;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class SyncUserInfoJob : BackgroundJobBase
    {
        private readonly IUserDomainService _userDomainService;

        public SyncUserInfoJob(IUserDomainService userDomainService)
        {
            _userDomainService = userDomainService;
        }

        protected override async Task DoExecute()
        {
            await _userDomainService.SyncUserInfos();
        }
    }
}
