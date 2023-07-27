using System;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Cms.Pages;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Permissions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.CmsSites
{
    //[RemoteService(IsEnabled = false)]
    [Authorize(CmsPermissions.Sites.Default)]
    public class CmsSiteAppService : CrudAppService<CmsSite,CmsSiteDto,Guid,GetCmsSitesInputDto,CreateUpdateCmsSiteDto>, ICmsSiteAppService
    {
        private IConfiguration _configuration;
        public CmsSiteAppService(IRepository<CmsSite, Guid> repository, IConfiguration configuration) : base(repository)
        {
            _configuration = configuration;
        }

        public async Task<CmsSiteDto> GetCurrentSite()
        {
            var siteName = _configuration["App:SiteName"];
            var entity = ReadOnlyRepository.WhereIf(siteName.IsNotNullOrEmpty(), x => x.Name == siteName).FirstOrDefault();
            return ObjectMapper.Map<CmsSite, CmsSiteDto>(entity);
        }
    }
}