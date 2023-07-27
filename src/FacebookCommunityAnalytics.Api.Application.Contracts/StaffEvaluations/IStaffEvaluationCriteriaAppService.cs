using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public interface IStaffEvaluationCriteriaAppService 
        : IBaseApiCrudAppService<StaffEvaluationCriteriaDto, Guid, GetStaffEvaluationCriteriaInput, CreateUpdateStaffEvaluationCriteriaDto>
    {
        Task<List<OrganizationUnitDto>> GetTeams();
        Task<PagedResultDto<StaffEvaluationCriteriaDto>> GetListExtendAsync(GetStaffEvaluationCriteriaInput input);
    }
}