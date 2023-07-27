using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.PartnerModule;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Web.Pages.Partners
{
    public class EditModalModel : ApiPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public PartnerUpdateDto EditPartner { get; set; }
        public IReadOnlyList<LookupDto<Guid>> PartnerUsersDtos { get; set; } = new List<LookupDto<Guid>>();

        private readonly IPartnerModuleAppService _partnerModuleAppService;
        
        public EditModalModel(IPartnerModuleAppService partnerModuleAppService)
        {
            _partnerModuleAppService = partnerModuleAppService;
        }

        public async Task OnGetAsync()
        {
            var partner = await _partnerModuleAppService.GetPartnerById(Id);
            EditPartner = ObjectMapper.Map<PartnerDto, PartnerUpdateDto>(partner);
            PartnerUsersDtos = await _partnerModuleAppService.GetPartnerUserLookup(new LookupRequestDto(){MaxResultCount = 1000});
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _partnerModuleAppService.EditPartner(Id, EditPartner);
            return NoContent();
        }
    }
}