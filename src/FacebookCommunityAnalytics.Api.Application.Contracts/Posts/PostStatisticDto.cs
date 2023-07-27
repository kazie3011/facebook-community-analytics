using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public class PostStatisticDto
    {
        public string AuthorName { get; set; }
        public string GroupName { get; set; }
        public PostContentType PostContentType { get; set; }
        public string Url { get; set; }
        public string Fid { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public int ShareCount { get; set; }
        public int TotalCount { get; set; }
    }
}