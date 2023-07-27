using FacebookCommunityAnalytics.Api.Users;

using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.UserInfos
{
    public class UserInfoWithNavigationPropertiesDto
    {
        public UserInfoDto UserInfo { get; set; }

        public AppUserDto AppUser { get; set; }
        
        public List<OrganizationUnitDto> OrgUnits { get; set; }

    }
}