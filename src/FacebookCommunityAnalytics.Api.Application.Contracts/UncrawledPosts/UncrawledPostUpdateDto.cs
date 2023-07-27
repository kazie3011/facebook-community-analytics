using System;
using System.ComponentModel.DataAnnotations;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.UncrawledPosts
{
    public class UncrawledPostUpdateDto
    {
        public string Url { get; set; }
        [Required]
        public PostSourceType PostSourceType { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}