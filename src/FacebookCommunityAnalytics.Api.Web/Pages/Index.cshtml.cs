using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ChartJSCore.Helpers;
using ChartJSCore.Models;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.PartnerModule;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Statistics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FacebookCommunityAnalytics.Api.Web.Pages
{
    public class IndexModel : ApiPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDateTime { get; set; } 

        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public DateTime? EndDateTime { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public Guid? CurrentPartnerId { get; set; }
        
        public List<LookupDto<Guid?>> CampaignLookupDtos { get; set; } = new List<LookupDto<Guid?>>();
        public List<LookupDto<Guid?>> PartnerLookupDtos { get; set; } = new List<LookupDto<Guid?>>();
        public  IEnumerable<Guid?> MultipleCampaignIds { get; set; }
        
        private readonly IPartnerModuleAppService _partnerModuleAppService;

        public IndexModel(IPartnerModuleAppService partnerModuleAppService)
        {
            _partnerModuleAppService = partnerModuleAppService;
        }
        public async Task OnGetAsync()
        {
            if (CurrentUser.IsAuthenticated && IsPartnerRole())
            {
                await InitDataFilterAsync();
            }
        }

        private async Task InitDataFilterAsync()
        {

            PartnerLookupDtos = await _partnerModuleAppService.GetPartnersLookup(new LookupRequestDto());
            if (PartnerLookupDtos.IsNotNullOrEmpty())
            {
                CurrentPartnerId = PartnerLookupDtos.FirstOrDefault()?.Id;
            }       
            
            PartnerLookupDtos.Insert(0, new LookupDto<Guid?>()
            {
                Id = null,
                DisplayName = @L["SelectItem.DefaultText", L["Partner"]].Value
            });
        }

        public async Task OnPostLoginAsync()
        {
            await HttpContext.ChallengeAsync("oidc");
        }
    }
}