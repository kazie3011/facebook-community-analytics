using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.PartnerModule;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Web.Pages.Campaigns
{
    public class CreateModalModel : ApiPageModel
    {
        [BindProperty]
        public CampaignCreateDto NewCampaign { get; set; }
        
        public DateTime? StartDate { get; set; }
        public List<LookupDto<Guid?>> PartnerLookupDtos { get; set; } = new List<LookupDto<Guid?>>();

        private readonly IPartnerModuleAppService _partnerModuleAppService;

        public CreateModalModel(IPartnerModuleAppService partnerModuleAppService)
        {
            _partnerModuleAppService = partnerModuleAppService;
        }

        public async Task OnGetAsync()
        {
            NewCampaign = new CampaignCreateDto();
            PartnerLookupDtos = await _partnerModuleAppService.GetPartnersLookup(new LookupRequestDto {MaxResultCount = 1000});
            PartnerLookupDtos.Insert(0, new LookupDto<Guid?>()
            {
                Id = null,
                DisplayName = @L["SelectItem.DefaultText", L["Partner"]].Value
            });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _partnerModuleAppService.CreateCampaign(NewCampaign);
            return NoContent();
        }
    }
}