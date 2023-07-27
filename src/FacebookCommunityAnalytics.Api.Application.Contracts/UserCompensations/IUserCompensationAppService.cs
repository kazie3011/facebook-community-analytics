using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public interface IUserCompensationAppService :
        IBaseApiCrudAppService<UserCompensationDto, Guid, GetUserCompensationsInput, CreateUpdateUserCompensationDto>
    {
        Task<PagedResultDto<UserCompensationNavigationPropertiesDto>> GetListWithNavigationAsync(GetUserCompensationsInput input);
        Task<UserCompensationNavigationPropertiesDto> GetWithNavigationAsync(Guid id);
        Task<UserCompensationNavigationPropertiesDto> GetWithNavigationByUserAsync(Guid userId, int month, int year);
        Task CalculateCompensation(int month, int year, bool isHappyDay = false);
        Task<byte[]> ExportCompensation(Guid payrollId);

        Task<CompensationDetailDto> GetCompensationDetailAsync(Guid payrollId);
        Task<List<CompensationAffiliateDto>> GetAffiliateConversions(DateTime fromDate, DateTime toDate, Guid userId);
    }
}