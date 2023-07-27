using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.PartnerModule;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Web.Pages.Posts
{
    public class EditModalModel : ApiPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty] public PostUpdateDto EditPost { get; set; }
        public IReadOnlyList<LookupDto<Guid?>> PartnerLookupDtos { get; set; } = new List<LookupDto<Guid?>>();
        public IReadOnlyList<LookupDto<Guid?>> CampaignLookupDtos { get; set; } = new List<LookupDto<Guid?>>();
        public IReadOnlyList<LookupDto<Guid?>> GroupLookupDtos { get; set; } = new List<LookupDto<Guid?>>();

        private readonly IPartnerModuleAppService _partnerModuleAppService;

        public EditModalModel(IPartnerModuleAppService partnerModuleAppService)
        {
            _partnerModuleAppService = partnerModuleAppService;
        }

        public async Task OnGetAsync()
        {
            var postDto = await _partnerModuleAppService.GetPostAsync(Id);
            EditPost = ObjectMapper.Map<PostDto, PostUpdateDto>(postDto);
            PartnerLookupDtos = await _partnerModuleAppService.GetPartnersLookup(new LookupRequestDto() { MaxResultCount = 1000 });
            CampaignLookupDtos = await _partnerModuleAppService.GetCampaignLookup(new GetCampaignLookupDto() { MaxResultCount = 1000 });
            GroupLookupDtos = await _partnerModuleAppService.GetGroupLookup(new GroupLookupRequestDto() { MaxResultCount = 1000 });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var postDto = await _partnerModuleAppService.GetPostAsync(Id);
            var postUpdateDto = ObjectMapper.Map<PostDto, PostUpdateDto>(postDto);
            postUpdateDto.CampaignId = EditPost.CampaignId;
            postUpdateDto.GroupId = EditPost.GroupId;
            postUpdateDto.Note = EditPost.Note;
            postUpdateDto.PostContentType = EditPost.PostContentType;
            postUpdateDto.IsNotAvailable = EditPost.IsNotAvailable;

            await _partnerModuleAppService.UpdatePost(Id, postUpdateDto);
            return NoContent();
        }
    }
}