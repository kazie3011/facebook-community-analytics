using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.PartnerModule;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Web.Controllers
{
    [Route("ApiCampaign")]
    public class CampaignController : AbpController
    {
        private readonly IPartnerModuleAppService _partnerModuleAppService;

        public CampaignController(IPartnerModuleAppService partnerModuleAppService)
        {
            _partnerModuleAppService = partnerModuleAppService;
        }

        [HttpDelete]
        [Route("RemovePostCampaign/{id}")]
        public async Task RemovePostCampaign(Guid id)
        {
            await _partnerModuleAppService.RemoveCampaignPost(id);
        }
    }
}