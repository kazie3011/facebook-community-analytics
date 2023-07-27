using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.AutoPosts
{
    public class CreateUpdateAutoPostFacebookDto
    {
        public CreateUpdateAutoPostFacebookDto()
        {
            Comments = new List<string>();
        }
        public string Url { get; set; }
        public int TotalLike { get; set; }
        public int CurrentLike { get; set; }
        
        public int TotalComment { get; set; }
        public int CurrentComment { get; set; }
        
        public List<string> Comments { get; set; }
        public bool IsDone { get; set; }
    }
}