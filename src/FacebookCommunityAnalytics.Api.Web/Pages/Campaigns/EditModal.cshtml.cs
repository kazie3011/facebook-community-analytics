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
    public class EditModalModel : ApiPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public CampaignUpdateDto EditCampaign { get; set; }
        public IReadOnlyList<LookupDto<Guid?>> PartnerUsers { get; set; } = new List<LookupDto<Guid?>>();

        private readonly IPartnerModuleAppService _partnerModuleAppService;
        
        public EditModalModel(IPartnerModuleAppService partnerModuleAppService)
        {
            _partnerModuleAppService = partnerModuleAppService;
        }

        public async Task OnGetAsync()
        {
            var partner = await _partnerModuleAppService.GetCampaign(Id);
            EditCampaign = ObjectMapper.Map<CampaignDto, CampaignUpdateDto>(partner);
            PartnerUsers = await _partnerModuleAppService.GetPartnersLookup(new LookupRequestDto() { MaxResultCount = 1000 });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _partnerModuleAppService.EditCampaign(Id, EditCampaign);
            return NoContent();
        }
    }
}