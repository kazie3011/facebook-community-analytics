using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public class CampaignPostDto : PostDto
    {
        public string GroupName { get; set; }
        public string Username { get; set; }
    }
}