using System;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.Integrations.Tiktok
{
    public class GetTiktokHashTagsApiResponse : BaseApiResponse
    {
        public List<string> HashTags { get; set; }
    }
    
    public class SaveTiktokStatApiRequest
    {
        public string Hashtag { get; set; }
        public long Count { get; set; }
    }
    
    public class SaveTiktokStatApiResponse: BaseApiResponse
    {
    }
    
    public class SaveChannelStatApiRequest
    {
        public string Title { get; set; }
        public string ChannelId { get; set; }
        public string Description { get; set; }
        public int Followers { get; set; }
        public int Likes { get; set; }
        public string ThumbnailImage { get; set; }
        public DateTime UpdatedAt { get; set; }

        public SaveChannelStatApiRequest()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
    
    public class SaveChannelStatApiResponse: BaseApiResponse
    {
    }
    
    public class SaveChannelVideoRequest
    {
        public string ChannelId { get; set; }
        public List<TiktokVideoDto> Videos { get; set; }
        public DateTime UpdatedAt { get; set; }

        public SaveChannelVideoRequest()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
    
    public class SaveChannelVideoResponse : BaseApiResponse
    {
        
    }

    public class TiktokVideoDto
    {
        public string VideoId { get; set; }
        public string VideoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Like { get; set; }
        public int Comment { get; set; }
        public int Share { get; set; }
        public int ViewCount { get; set; }
        public string Content { get; set; }
        public  string ThumbnailImage { get; set; }
        public List<string> HashTags { get; set; }
    }

    public class UpdateTiktokVideosStateRequest
    {
        public List<string> VideoIds { get; set; }
        public bool IsNew { get; set; }
    }
}