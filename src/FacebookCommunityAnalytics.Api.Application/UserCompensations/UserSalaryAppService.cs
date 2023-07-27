using System;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.UserSalaries.Default)]
    public class UserSalaryAppService : BaseCrudApiAppService<UserSalary, UserSalaryDto, Guid, GetUserSalariesInput, CreateUpdateUserSalaryDto>, IUserSalaryAppService
    {
        public UserSalaryAppService(IRepository<UserSalary, Guid> repository) : base(repository)
        {
        }
    }
}