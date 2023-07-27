using System;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Cms.Pages
{
    public interface ICmsPageAppService : ICrudAppService<CmsPageDto,Guid,GetCmsPagesInputDto,CreateUpdateCmsPageDto>
    {
        
    }
}