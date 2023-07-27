using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.TikTokMCNs
{
    public interface ITikTokMCNAppService :
        IBaseApiCrudAppService<TikTokMCNDto, Guid, GetTikTokMCNsInput, CreateUpdateTikTokMCNDto>
    {
        Task<List<string>> GetHashtags();
    }
}