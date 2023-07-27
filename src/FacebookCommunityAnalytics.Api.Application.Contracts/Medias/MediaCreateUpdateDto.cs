using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Medias
{
    public class MediaCreateUpdateDto
    {
        public MediaEntityType MediaEntityType { get; set; }

        [MaxLength(250)]
        [Required]
        public string FileName { get; set; }
        [MaxLength(250)]
        public string Url { get; set; }
        [MaxLength(250)]
        public string ThumbnailUrl { get; set; }
        [MaxLength(10)]
        [Required]
        public string Extension { get; set; }

        public string FileContentType { get; set; }
        
        public List<string> Tags { get; set; }
        
        public MediaFileInfo MediaFileInfo { get; set; }
        
        public bool CreateThumbnail { get; set; }
        public MediaCategory? MediaCategory { get; set; }   
    }

    public class MediaFileInfo
    {
        public byte[] FileBytes { get; set; }
        public long Length { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}