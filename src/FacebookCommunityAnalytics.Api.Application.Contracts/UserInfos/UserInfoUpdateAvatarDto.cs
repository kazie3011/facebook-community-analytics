using System;
using FacebookCommunityAnalytics.Api.Medias;

namespace FacebookCommunityAnalytics.Api.UserInfos
{
    public class UserInfoUpdateAvatarDto
    {
        public Guid MediaId { get; set; }
        public Guid? UserId { get; set; }
    }
}
