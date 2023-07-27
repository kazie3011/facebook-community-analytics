using Volo.Abp.Application.Dtos;
using System;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.UncrawledPosts
{
    public class GetUncrawledPostsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }
        public string Url { get; set; }
        public PostSourceType? PostSourceType { get; set; }
        public DateTime? UpdatedAtMin { get; set; }
        public DateTime? UpdatedAtMax { get; set; }

        public GetUncrawledPostsInput()
        {

        }
    }
}