using FacebookCommunityAnalytics.Api.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.UserPayrollCommissions
{
    public interface IUserPayrollCommissionsAppService : IApplicationService
    {
        Task<PagedResultDto<UserPayrollCommissionWithNavigationPropertiesDto>> GetListAsync(GetUserPayrollCommissionsInput input);

        Task<UserPayrollCommissionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<UserPayrollCommissionDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid?>>> GetPayrollLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<UserPayrollCommissionDto> CreateAsync(UserPayrollCommissionCreateDto input);

        Task<UserPayrollCommissionDto> UpdateAsync(Guid id, UserPayrollCommissionUpdateDto input);
    }
}