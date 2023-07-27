using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Categories;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public class TiktokWithNavigationProperties
    {
        public Tiktok Tiktok { get; set; }

        public Category Category { get; set; }
        public Group Group { get; set; }
        public AppUser AppUser { get; set; }
        public UserInfo AppUserInfo { get; set; }
        public Campaign Campaign { get; set; }
        public Partner Partner { get; set; }
        
    }
}