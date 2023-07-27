using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.CmsGdl;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.Controllers.CmsGdl
{
    [RemoteService]
    [Area("app")]
    [ControllerName("CmsGdl")]
    [Route("api/app/cms-gdl")]
    public class CmsGdlController:ApiController, ICmsGdlAppService
    {
        private readonly ICmsGdlAppService _cmsGdlAppService;

        public CmsGdlController(ICmsGdlAppService cmsGdlAppService)
        {
            _cmsGdlAppService = cmsGdlAppService;
        }

        [HttpGet]
        [Route("landing")]
        public Task<CmsGdlLandingPageModel> GetLandingPageModel()
        {
            return _cmsGdlAppService.GetLandingPageModel();
        }

        [HttpGet]
        [Route("communities")]
        public Task<CmsGdlCommunityGroup> GetCommunityDetails(GroupCategoryType groupCategoryType)
        {
            return _cmsGdlAppService.GetCommunityDetails(groupCategoryType);
        }
    }
}