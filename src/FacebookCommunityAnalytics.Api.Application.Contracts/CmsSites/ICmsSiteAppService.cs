using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.CmsSites
{
    public interface ICmsSiteAppService : ICrudAppService<CmsSiteDto, Guid, GetCmsSitesInputDto,CreateUpdateCmsSiteDto>
    {
        Task<CmsSiteDto> GetCurrentSite();
    }
}