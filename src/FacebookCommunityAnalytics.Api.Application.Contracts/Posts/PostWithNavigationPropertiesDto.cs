using FacebookCommunityAnalytics.Api.Categories;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Partners;

using System;
using FacebookCommunityAnalytics.Api.UserInfos;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public class PostWithNavigationPropertiesDto
    {
        public PostDto Post { get; set; }

        public CategoryDto Category { get; set; }
        public GroupDto Group { get; set; }
        public AppUserDto AppUser { get; set; }
        public UserInfoDto AppUserInfo { get; set; }
        //public AppUserDto Leader { get; set; }
        //public UserInfoDto LeaderInfo { get; set; }
        public CampaignDto Campaign { get; set; }
        public PartnerDto Partner { get; set; }

    }
}