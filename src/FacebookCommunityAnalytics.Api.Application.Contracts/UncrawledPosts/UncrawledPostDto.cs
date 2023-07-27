using System;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.UncrawledPosts
{
    public class UncrawledPostDto : FullAuditedEntityDto<Guid>
    {
        public string Url { get; set; }
        public PostSourceType PostSourceType { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}