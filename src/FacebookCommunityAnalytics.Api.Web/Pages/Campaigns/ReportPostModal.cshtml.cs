using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.PartnerModule;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace FacebookCommunityAnalytics.Api.Web.Pages.Campaigns
{
    public class ReportPostModel : ApiPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string CampaignId { get; set; }
        [BindProperty]
        public PostCreateDto NewPost { get; set; }
        
        [BindProperty]
        [TextArea(Rows = 10)]
        [Required]
        public string PostUrls { get; set; }
        private readonly IPartnerModuleAppService _partnerModuleAppService;

        public ReportPostModel(IPartnerModuleAppService partnerModuleAppService)
        {
            _partnerModuleAppService = partnerModuleAppService;
        }

        public async Task OnGetAsync()
        {
            
            NewPost = new PostCreateDto
            {
                SubmissionDateTime = DateTime.UtcNow,
                CampaignId = CampaignId.ToGuidOrDefault(),
                PostContentType = PostContentType.Seeding,
                PostCopyrightType = PostCopyrightType.Unknown
            };

            if (CurrentUser.Id != null)
            {
                var campaign = await _partnerModuleAppService.GetCampaign(CampaignId.ToGuidOrDefault());
                if (campaign != null)
                {
                    NewPost.PartnerId = campaign.PartnerId;
                }
            }

        }

        public async Task<IActionResult> OnPostAsync()
        {
            NewPost.Url = PostUrls;
            NewPost.CreatedDateTime =  DateTime.UtcNow;
            await CreatePostAsync();
            return NoContent();
        }

        private async Task CreatePostAsync()
        {
            var success = await Invoke(async () => { await _partnerModuleAppService.CreateMultiplePosts(NewPost); }, L, true);
        }
    }
}