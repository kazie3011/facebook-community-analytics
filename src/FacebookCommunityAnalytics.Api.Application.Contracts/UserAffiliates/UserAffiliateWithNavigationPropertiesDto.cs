using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Categories;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;

namespace FacebookCommunityAnalytics.Api.UserAffiliates
{
    public class UserAffiliateWithNavigationPropertiesDto
    {
        public UserAffiliateDto UserAffiliate { get; set; }
        public AppUserDto AppUser { get; set; }
        public UserInfoDto UserInfo { get; set; }
        public CategoryDto Category { get; set; }
        public PartnerDto Partner { get; set; }
        public CampaignDto Campaign { get; set; }
    }
}