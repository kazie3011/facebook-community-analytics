using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.PartnerModule;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace FacebookCommunityAnalytics.Api.Web.Pages.Posts
{
    public class CreateModalModel : ApiPageModel
    {
        [BindProperty] public PostCreateDto NewPost { get; set; }
        
        [BindProperty]
        [TextArea(Rows = 10)]
        [Required]
        public string PostUrls { get; set; }
        public IList<SelectListItem> CampaignsSelectListItems { get; set; } = new List<SelectListItem>();

        private readonly IPartnerModuleAppService _partnerModuleAppService;

        public CreateModalModel(IPartnerModuleAppService partnerModuleAppService)
        {
            _partnerModuleAppService = partnerModuleAppService;
        }

        public async Task OnGetAsync()
        {
            var campaignsDtos = await _partnerModuleAppService.GetCampaignLookup(new GetCampaignLookupDto { MaxResultCount = 1000 });
            CampaignsSelectListItems = campaignsDtos.ToSelectListItems();
            
            NewPost = new PostCreateDto
            {
                SubmissionDateTime = DateTime.UtcNow,
                CampaignId = Guid.Empty,
                PostContentType = PostContentType.Seeding,
                PostCopyrightType = PostCopyrightType.Unknown
            };

            if (CurrentUser.Id != null)
            {
                var partners = await _partnerModuleAppService.GetPartnersByUser(CurrentUser.Id.Value);
                if (partners.IsNotNullOrEmpty())
                {
                    NewPost.PartnerId = partners.FirstOrDefault()?.Id;
                }
            }

        }

        public async Task<IActionResult> OnPostAsync()
        {
            NewPost.Url = PostUrls;
            await CreatePostAsync();
            return NoContent();
        }

        private async Task CreatePostAsync()
        {
            var success = await Invoke(async () => { await _partnerModuleAppService.CreateMultiplePosts(NewPost); }, L, true);
        }
    }
}