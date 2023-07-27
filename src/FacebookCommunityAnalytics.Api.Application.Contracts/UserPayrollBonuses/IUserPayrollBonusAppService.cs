using FacebookCommunityAnalytics.Api.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.UserPayrollBonuses
{
    public interface IUserPayrollBonusesAppService : IApplicationService
    {
        Task<PagedResultDto<UserPayrollBonusWithNavigationPropertiesDto>> GetListAsync(GetUserPayrollBonusesInput input);

        Task<UserPayrollBonusWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<UserPayrollBonusDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid?>>> GetPayrollLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<UserPayrollBonusDto> CreateAsync(UserPayrollBonusCreateDto input);

        Task<UserPayrollBonusDto> UpdateAsync(Guid id, UserPayrollBonusUpdateDto input);
    }
}