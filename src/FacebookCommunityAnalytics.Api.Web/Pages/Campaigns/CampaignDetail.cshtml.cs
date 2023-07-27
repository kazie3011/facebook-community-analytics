using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.PartnerModule;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FacebookCommunityAnalytics.Api.Web.Pages.Campaigns
{
    public class CampaignDetail : ApiPageModel
    {
        public string CampaignId { get; set; }
        public CampaignDto CampaignDto { get; set; }
        public List<CampaignPostDto> CampaignPosts = new();
        public List<CampaignPostDto> SeedingPosts = new();
        public List<CampaignPostDto> ContestPosts = new();

        private readonly IPartnerModuleAppService _partnerModuleAppService;

        public CampaignDetail(IPartnerModuleAppService partnerModuleAppService)
        {
            _partnerModuleAppService = partnerModuleAppService;
        }

        public async Task OnGetAsync(string campaignId)
        {
            CampaignId = campaignId;
            if (CampaignId.IsNotNullOrEmpty())
            {
                CampaignDto = await _partnerModuleAppService.GetCampaign(CampaignId.ToGuidOrDefault());
                CampaignPosts = await _partnerModuleAppService.GetCampaignPosts(CampaignId.ToGuidOrDefault());
                ContestPosts = CampaignPosts.Where(p => p.PostContentType == PostContentType.Contest).ToList();
                SeedingPosts = CampaignPosts.Where(p => p.PostContentType == PostContentType.Seeding).ToList();

                CampaignDto.Current.Seeding_TotalPost = SeedingPosts.Count;
                CampaignDto.Current.Seeding_TotalReaction = SeedingPosts.Sum(_ => _.TotalCount);


                CampaignDto.Current.Contest_TotalPost = ContestPosts.Count;
                CampaignDto.Current.Contest_TotalReaction = ContestPosts.Sum(_ => _.TotalCount);
            }
        }
    }
}