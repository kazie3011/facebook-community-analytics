using System;
using System.Collections.Generic;
using System.Linq;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Posts;

namespace FacebookCommunityAnalytics.Api.Statistics
{
    public class GeneralStatsResponse
    {
        public GeneralStatsResponse()
        {
            TopReactionPosts = new List<PostStatisticDto>();
            TopReactionAffiliatePosts = new List<PostStatisticDto>();
            Stats = new List<GeneralStatsResponseItem>();
            Affiliates = new List<AffiliateInfo>();
            DataChartCountPosts = new List<DataChartItemDto<int, string>>();
            DataChartCountPostsByType = new List<DataChartItemDto<int, PostContentType>>();
            DataChartCountPostsByCampaignType = new List<DataChartItemDto<int, CampaignType>>();
        }
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

        public int GetTotalReactionAffPosts()
        {
            var totalReaction = this.TopReactionAffiliatePosts.Sum(item => item.TotalCount);
            return totalReaction;
        }

        public int GetTotalLikeAffPosts()
        {
            var totalLike = this.TopReactionAffiliatePosts.Sum(item => item.LikeCount);
            return totalLike;
        }

        public int GetTotalShareAffPosts()
        {
            var totalShare = this.TopReactionAffiliatePosts.Sum(item => item.ShareCount);
            return totalShare;
        }

        public int GetTotalCommentAffPosts()
        {
            var totalComment = this.TopReactionAffiliatePosts.Sum(item => item.CommentCount);
            return totalComment;
        }

        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }

        public int Total_Post { get; set; }
        public int Total_User { get; set; }
        public int Total_Group { get; set; }
        public int Total_Page { get; set; }

        public List<PostStatisticDto> TopReactionPosts { get; set; }
        public List<PostStatisticDto> TopReactionAffiliatePosts { get; set; }
        public List<GeneralStatsResponseItem> Stats { get; set; }
        public List<AffiliateInfo> Affiliates { get; set; }
        public List<DataChartItemDto<int, string>> DataChartCountPosts { get; set; }
        public List<DataChartItemDto<int, PostContentType>> DataChartCountPostsByType { get; set; }
        public List<DataChartItemDto<int, CampaignType>> DataChartCountPostsByCampaignType { get; set; }

        public class GeneralStatsResponseItem
        {
            public string Type { get; set; }
            public int TotalPost { get; set; }
            public int TotalUser { get; set; }
            public double AvgPostPerUser { get; set; }
            public double AvgReactionPerUser { get; set; }
            public double AvgReactionPerPost { get; set; }
        }

        public class AffiliateInfo
        {
            public string Name { get; set; }
            public int Click { get; set; }
            public int Conversion { get; set; }
            public decimal Amount { get; set; }
            public decimal Commission { get; set; }
        }
    }

    public class GeneralStatsApiRequest
    {
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }

        public int? ClientOffsetInMinutes { get; set; }

        public GroupSourceType GroupSourceType { get; set; }
        public TikTokMCNType TikTokMcnType { get; set; }
        public List<Guid> TikTokMcnIds { get; set; }
        public int Count { get; set; }
    }

    public class TimeFrame
    {
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
        public string Display { get; set; }
    }
}