using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Categories;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public class TiktokWithNavigationPropertiesDto
    {
        public TiktokDto Tiktok { get; set; }
        public GroupDto Group { get; set; }
        public AppUserDto AppUser { get; set; }
        public UserInfoDto AppUserInfo { get; set; }
        public CampaignDto Campaign { get; set; }
        public PartnerDto Partner { get; set; }

    }
}