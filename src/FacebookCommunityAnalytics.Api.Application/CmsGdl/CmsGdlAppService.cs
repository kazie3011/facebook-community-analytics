using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.CmsGdl
{
    [RemoteService(IsEnabled = false)]
    [AllowAnonymous]
    public class CmsGdlAppService: ApiAppService, ICmsGdlAppService
    {
        private readonly ICmsGdlDomainService _cmsGdlDomainService;

        public CmsGdlAppService(ICmsGdlDomainService cmsGdlDomainService)
        {
            _cmsGdlDomainService = cmsGdlDomainService;
        }

        public async Task<CmsGdlLandingPageModel> GetLandingPageModel()
        {
            return await _cmsGdlDomainService.GetLandingPageModel();
        }

        public async Task<CmsGdlCommunityGroup> GetCommunityDetails(GroupCategoryType groupCategoryType)
        {
            return await _cmsGdlDomainService.GetCommunityDetails(groupCategoryType);
        }
    }
}