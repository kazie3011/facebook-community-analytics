using System;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.UserInfos
{
    public class GetExtendUserInfosInput : GetUserInfosInput
    {
        public IEnumerable<Guid> AppUserIds { get; set; }
    }
}
