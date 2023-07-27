using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.PartnerModule;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Web.Pages.Partners
{
    public class CreateModalModel : ApiPageModel
    {
        [BindProperty]
        public PartnerCreateDto NewPartner { get; set; }
        public IReadOnlyList<LookupDto<Guid?>> PartnerUsersDtos { get; set; } = new List<LookupDto<Guid?>>();

        private readonly IPartnerModuleAppService _partnerModuleAppService;

        public CreateModalModel(IPartnerModuleAppService partnerModuleAppService)
        {
            _partnerModuleAppService = partnerModuleAppService;
        }

        public async Task OnGetAsync()
        {
            NewPartner = new PartnerCreateDto();
            PartnerUsersDtos = await _partnerModuleAppService.GetPartnersLookup(new LookupRequestDto {MaxResultCount = 1000});
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                NewPartner.Code = NewPartner.Name.IsNullOrWhiteSpace() ? "" : NewPartner.Name.Replace(" ", "").Trim().RemoveDiacritics().ToLower();
                
                await _partnerModuleAppService.CreatePartner(NewPartner);
            }
            return NoContent();
        }
    }
}