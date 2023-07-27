using System.Collections.Generic;
using System.Linq;
using FacebookCommunityAnalytics.Api.Posts;

namespace FacebookCommunityAnalytics.Api.Statistics
{
    public class GrowthCampaignChartDto
    {
        public GrowthCampaignChartDto()
        {
            ChartLabels = new List<string>();
            TopReactionPosts = new List<PostStatisticDto>();
            TotalInteractionsLineCharts = new List<DataChartItemDto<int, string>>();
            Stats = new List<PartnerGeneralStatsResponseItem>();
        }
        
        public List<PostStatisticDto> TopReactionPosts { get; set; }
        public List<string> ChartLabels { get; set; }
        public List<DataChartItemDto<int, string>> TotalInteractionsLineCharts { get; set; }
        public List<PartnerGeneralStatsResponseItem> Stats { get; set; }
        
        public int TotalPartner { get; set; }
        public int TotalCampaign { get; set; }
        public int TotalPostFb { get; set; }
        public int TotalPostTiktok { get; set; }
        
        public int GetTotalReactionPosts()
        {
            var totalReaction = this.TopReactionPosts.Sum(item => item.TotalCount);
            return totalReaction;
        }

        public int GetTotalLikePosts()
        {
            var totalLike = this.TopReactionPosts.Sum(item => item.LikeCount);
            return totalLike;
        }

        public int GetTotalSharePosts()
        {
            var totalShare = this.TopReactionPosts.Sum(item => item.ShareCount);
            return totalShare;
        }

        public int GetTotalCommentPosts()
        {
            var totalComment = this.TopReactionPosts.Sum(item => item.CommentCount);
            return totalComment;
        }
        
        public class PartnerGeneralStatsResponseItem
        {
            public string Group { get; set; }
            public int TotalPost { get; set; }
            public double TotalReactions { get; set; }
            public double AvgReactionPerPost { get; set; }
        }
    }
}