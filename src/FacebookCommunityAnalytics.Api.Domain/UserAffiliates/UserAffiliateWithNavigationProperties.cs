using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Categories;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.Groups;

namespace FacebookCommunityAnalytics.Api.UserAffiliates
{
    public class UserAffiliateWithNavigationProperties
    {
        public UserAffiliate UserAffiliate { get; set; }
        public AppUser AppUser { get; set; }
        public UserInfo UserInfo { get; set; }
        public Group Group { get; set; }
        public Category Category { get; set; }
        public Partner Partner { get; set; }
        public Campaign Campaign { get; set; }
    }
}