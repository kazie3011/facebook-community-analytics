using System;

namespace FacebookCommunityAnalytics.Api.Integrations.Tiktok
{
    public class TikTokChannelCreateDto
    {
        public Guid McnId { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
    }
}