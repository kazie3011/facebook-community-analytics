using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.PartnerModule;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Web.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace FacebookCommunityAnalytics.Api.Web.Pages.Posts
{
    public class IndexModel : AbpPageModel
    {
        private readonly IPartnerModuleAppService _partnerModuleAppService;
        public IList<SelectListItem> CampaignFilter { get; set; } = new List<SelectListItem>();
        public IList<SelectListItem> GroupsFilter { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PartnerFilter { get; set; } = new List<SelectListItem>();
        public GetPostsInputExtend Filter { get; set; }
        public IndexModel(IPartnerModuleAppService partnerModuleAppService)
        {
            _partnerModuleAppService = partnerModuleAppService;
        }

        public async Task OnGetAsync()
        {
           var campaignsNullable = await _partnerModuleAppService.GetGroupLookup(new GroupLookupRequestDto());
           CampaignFilter = campaignsNullable.ToSelectListItems();
           
           var groupsNullable = await _partnerModuleAppService.GetGroupLookup(new GroupLookupRequestDto());
           GroupsFilter = groupsNullable.ToSelectListItems();
           
           var partnerNullable = await _partnerModuleAppService.GetPartnersLookup(new LookupRequestDto());
           PartnerFilter = partnerNullable.Select(x => new SelectListItem
           {
               Text = x.DisplayName,
               Value = x.Id.ToString()
           }).ToList();
           
           CampaignFilter.Insert(0, new SelectListItem
           {
               Value = string.Empty,
               Text = @L["SelectItem.DefaultText", L["Campaign"]].Value
           });
           
           GroupsFilter.Insert(0, new SelectListItem
           {
               Value = string.Empty,
               Text = @L["SelectItem.DefaultText", L["Group"]].Value
           });
           
           PartnerFilter.Insert(0, new SelectListItem
           {
               Value = string.Empty,
               Text = @L["SelectItem.DefaultText", L["Partner"]].Value
           });
           
            await Task.CompletedTask;
        }
    }
}