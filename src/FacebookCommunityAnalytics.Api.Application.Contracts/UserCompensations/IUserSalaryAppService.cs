using System;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public interface IUserSalaryAppService :
        IBaseApiCrudAppService<UserSalaryDto, Guid, GetUserSalariesInput, CreateUpdateUserSalaryDto>
    {
        
    }
}