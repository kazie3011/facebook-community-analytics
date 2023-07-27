using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.PartnerModule;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace FacebookCommunityAnalytics.Api.Web.Pages.Posts
{
    public class EditNoteModalModel : ApiPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        [TextArea]
        public string PostNoteUpdate { get; set; }

        private readonly IPartnerModuleAppService _partnerModuleAppService;
        
        public EditNoteModalModel(IPartnerModuleAppService partnerModuleAppService)
        {
            _partnerModuleAppService = partnerModuleAppService;
        }

        public async Task OnGetAsync()
        {
            var postWithNavigationPropertiesDto = await _partnerModuleAppService.GetPostAsync(Id);
            PostNoteUpdate = postWithNavigationPropertiesDto.Note;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await UpdateNotePostAsync();
            return NoContent();
        }
        
        private async Task UpdateNotePostAsync()
        {
            var success = await Invoke
            (
                async () =>
                {
                    await _partnerModuleAppService.UpdateNotePost(Id, new PostUpdateNoteDto()
                    {
                        Note = PostNoteUpdate
                    });
                },
                L,
                true
            );
        }

    }
}