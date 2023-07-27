using System;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public interface IUserBonusConfigAppService :
        IBaseApiCrudAppService<UserBonusConfigDto, Guid, GetUserBonusConfigsInput, CreateUpdateUserBonusConfigDto>
    {
        
    }
}