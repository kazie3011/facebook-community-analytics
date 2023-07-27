using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Groups;
using Microsoft.Extensions.DependencyInjection;

namespace FacebookCommunityAnalytics.Api.Crawl
{
    public class GetUncrawledGroupApiRequest
    {
        public GroupSourceType GroupSourceType { get; set; }
        public bool IgnoreTime { get; set; }
    }

    public class GetUncrawledGroupApiResponse
    {
        public int Count { get; set; }
        public List<GroupDto> Groups { get; set; }

        public GetUncrawledGroupApiResponse()
        {
            Groups = new List<GroupDto>();
        }
    }
}