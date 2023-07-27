using System;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.GlobalFeatures;
using Volo.CmsKit.Admin.Pages;
using Volo.CmsKit.GlobalFeatures;
using Volo.CmsKit.Permissions;

namespace FacebookCommunityAnalytics.Api.Cms.Pages
{    
    [RemoteService(IsEnabled = false)]
    [RequiresGlobalFeature(typeof(PagesFeature))]
    [Authorize(CmsKitAdminPermissions.Pages.Default)]
    public class CmsPageAppService : CrudAppService<CmsPage,CmsPageDto,Guid,GetCmsPagesInputDto,CreateUpdateCmsPageDto>, ICmsPageAppService
    {
        public CmsPageAppService(IRepository<CmsPage, Guid> repository) : base(repository)
        {
        }
    }
}