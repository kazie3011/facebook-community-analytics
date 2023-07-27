using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Posts;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public interface IStaffEvaluationAppService 
        : IBaseApiCrudAppService<StaffEvaluationDto, Guid, GetStaffEvaluationsInput, CreateUpdateStaffEvaluationDto>
    {
        Task<PagedResultDto<StaffEvaluationWithNavigationPropertiesDto>> GetListExtendAsync(GetStaffEvaluationsInput input);
        Task<StaffEvaluationWithNavigationPropertiesDto> GetStaffEvaluationByUser(Guid userId,int month, int year);
        
        // Why don't put this method in IStaffEvaluationCriteriaAppService?
        // TODOO T.Anh: Vu Nguyen - Remove StaffEvaluationCriteria completely, this is obsolete due to many reason, mostly from biz changes
        Task<List<StaffEvaluationCriteriaDto>> GetStaffEvaluationCriteria();
        Task<List<OrganizationUnitDto>> GetEvaluationTeams();
        Task GenerateStaffEvaluations();
        Task EvaluateStaffs(StaffEvaluationRequest apiRequest);
        Task<List<StaffEvaluationExportRow>> GetExportRows(GetStaffEvaluationsInput input);
        Task<List<PostDetailExportRow>> GetEvaluationSeedingPostExport(GetPostEvaluationRequest request);
        Task<byte[]> GetAffiliatesEvaluationExport(GetAffiliateEvaluationRequest request);
        Task<List<TikTokChannelEvaluationExport>> GetEvaluationTiktokChannelExport(ExportTiktokEvaluationRequest request);
        Task<List<ContractEvaluationExport>> GetContractEvaluationExport(GetContractEvaluationRequest request);
        Task<List<int>> GetEvaluationYears();
        Task<bool> ContainSaleTeam(params string[] teamNames);
    }
}