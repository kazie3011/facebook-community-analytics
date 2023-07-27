using FacebookCommunityAnalytics.Api.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.AccountProxies
{
    public interface IAccountProxiesAppService : IApplicationService
    {
        Task<PagedResultDto<AccountProxyWithNavigationPropertiesDto>> GetListAsync(GetAccountProxiesInput input);

        Task<AccountProxyWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<AccountProxyDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid?>>> GetAccountLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid?>>> GetProxyLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<AccountProxyDto> CreateAsync(AccountProxyCreateDto input);

        Task<AccountProxyDto> UpdateAsync(Guid id, AccountProxyUpdateDto input);

    }
}