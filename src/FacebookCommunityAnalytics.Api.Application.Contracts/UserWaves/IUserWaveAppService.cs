using FacebookCommunityAnalytics.Api.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.UserWaves
{
    public interface IUserWavesAppService : IApplicationService
    {
        Task<PagedResultDto<UserWaveWithNavigationPropertiesDto>> GetListAsync(GetUserWavesInput input);

        Task<UserWaveWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<UserWaveDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid?>>> GetPayrollLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<UserWaveDto> CreateAsync(UserWaveCreateDto input);

        Task<UserWaveDto> UpdateAsync(Guid id, UserWaveUpdateDto input);
    }
}