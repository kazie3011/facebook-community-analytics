using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Medias
{
    public class MediaDto : AuditedEntityDto<Guid>
    {
        public MediaDto()
        {
            Tags = new List<string>();
        }
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
        
        public List<string>   Tags          { get; set; }
        public MediaCategory? MediaCategory { get; set; }   
        public byte[]         FileData      { get; set; }
        public string         ImagePath     { get; set; }
    }
}