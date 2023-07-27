using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Integrations.Tiktok;

namespace FacebookCommunityAnalytics.Api.Crawl
{
    public class CrawlMCNVideoInput
    {
        public string Hashtag { get; set; }
        public List<ChannelVideoDto> ChannelVideos { get; set; }
    }

    public class ChannelVideoDto
    {
        public SaveChannelStatApiRequest ChannelStat { get; set; }
        public List<TiktokVideoDto> TiktokVideos { get; set; }
    }
}