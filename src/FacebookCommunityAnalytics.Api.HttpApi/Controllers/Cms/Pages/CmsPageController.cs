using System;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Cms.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.CmsKit.Permissions;

namespace FacebookCommunityAnalytics.Api.Controllers.Cms.Pages
{
    [RemoteService]
    [Area("app")]
    [ControllerName("CmsPage")]
    [Route("api/app/cms-pages")]
    public class CmsPageController : AbpController, ICmsPageAppService
    {
        private readonly ICmsPageAppService _cmsPageAppService;
        
        public CmsPageController(ICmsPageAppService cmsPageAppService)
        {
            _cmsPageAppService = cmsPageAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<CmsPageDto> GetAsync(Guid id)
        {
            return _cmsPageAppService.GetAsync(id);
        }

        [HttpGet]
        public Task<PagedResultDto<CmsPageDto>> GetListAsync(GetCmsPagesInputDto input)
        {
            return _cmsPageAppService.GetListAsync(input);
        }
        
        [HttpPost]
        [Authorize(CmsKitAdminPermissions.Pages.Create)]
        public Task<CmsPageDto> CreateAsync(CreateUpdateCmsPageDto input)
        {
            return _cmsPageAppService.CreateAsync(input);
        }

        [HttpPut]
        [Authorize(CmsKitAdminPermissions.Pages.Update)]
        [Route("{id}")]
        public Task<CmsPageDto> UpdateAsync(Guid id, CreateUpdateCmsPageDto input)
        {
            return _cmsPageAppService.UpdateAsync(id, input);
        }
        
        [HttpDelete]
        [Authorize(CmsKitAdminPermissions.Pages.Delete)]
        [Route("{id}")]
        public Task DeleteAsync(Guid id)
        {
            return _cmsPageAppService.DeleteAsync(id);
        }
    }
}