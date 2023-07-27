using FacebookCommunityAnalytics.Api.Categories;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.UserInfos;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public class PostWithNavigationProperties
    {
        public Post Post { get; set; }

        public Category Category { get; set; }
        public Group Group { get; set; }
        public AppUser AppUser { get; set; }
        public UserInfo AppUserInfo { get; set; }
        public Campaign Campaign { get; set; }
        public Partner Partner { get; set; }
        
    }
}