using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.PartnerModule;
using Microsoft.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Web.Controllers
{
    [Route("Exports")]
    public class ExportsController : Controller
    {
        private readonly IPartnerModuleAppService _partnerModuleAppService;

        public ExportsController(IPartnerModuleAppService partnerModuleAppService)
        {
            _partnerModuleAppService = partnerModuleAppService;
        }

        // GET
        [Route("CampaignPosts/{campaignId}")]
        public async Task<IActionResult> CampaignPosts(Guid campaignId)
        {
            var data = await _partnerModuleAppService.ExportCampaign(campaignId);
            var campaign = await _partnerModuleAppService.GetCampaign(campaignId);
            var fileName = $"CP_{campaign.Code}";
            if (data != null)
            {
                return new JsonResult(new { hasData = true, fileName = $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx", content = Convert.ToBase64String(data) });
            }
            else
            {
                return new JsonResult(new { hasData = false });
            }
        }
    }
}