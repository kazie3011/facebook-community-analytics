using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using FacebookCommunityAnalytics.Api.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.Controllers.StaffEvaluations
{
    [RemoteService]
    [AllowAnonymous]
    [Area("app")]
    [ControllerName("StaffEvaluations")]
    [Route("api/app/staff-evaluations")]
    public class StaffEvaluationController : AbpController, IStaffEvaluationAppService
    {
        private readonly IStaffEvaluationAppService _staffEvaluationAppService;

        public StaffEvaluationController(IStaffEvaluationAppService staffEvaluationAppService)
        {
            _staffEvaluationAppService = staffEvaluationAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<StaffEvaluationDto> GetAsync(Guid id)
        {
            return _staffEvaluationAppService.GetAsync(id);
        }
        
        [HttpGet]
        [Route("get-list-async")]
        public Task<PagedResultDto<StaffEvaluationDto>> GetListAsync(GetStaffEvaluationsInput input)
        {
            return _staffEvaluationAppService.GetListAsync(input);
        }

        [HttpPost]
        public Task<StaffEvaluationDto> CreateAsync(CreateUpdateStaffEvaluationDto input)
        {
            return _staffEvaluationAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public Task<StaffEvaluationDto> UpdateAsync(Guid id, CreateUpdateStaffEvaluationDto input)
        {
            return _staffEvaluationAppService.UpdateAsync(id, input);
        }
        
        [HttpDelete]
        [Route("{id}")]
        public Task DeleteAsync(Guid id)
        {
            return _staffEvaluationAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("get-list-extend-async")]
        public Task<PagedResultDto<StaffEvaluationWithNavigationPropertiesDto>> GetListExtendAsync(GetStaffEvaluationsInput input)
        {
            return _staffEvaluationAppService.GetListExtendAsync(input);
        }

        [HttpGet]
        [Route("get-staff-eval-by-user")]
        public Task<StaffEvaluationWithNavigationPropertiesDto> GetStaffEvaluationByUser(Guid userId, int month, int year)
        {
            return _staffEvaluationAppService.GetStaffEvaluationByUser(userId, month, year);
        }

        [HttpGet]
        [Route("get-staff-eva-criteria")]
        public Task<List<StaffEvaluationCriteriaDto>> GetStaffEvaluationCriteria()
        {
            return _staffEvaluationAppService.GetStaffEvaluationCriteria();
        }

        [HttpGet]
        [Route("get-teams")]
        public Task<List<OrganizationUnitDto>> GetEvaluationTeams()
        {
            return _staffEvaluationAppService.GetEvaluationTeams();
        }

        [HttpGet]
        [Route("create-staff-evaluation")]
        public Task GenerateStaffEvaluations()
        {
            return _staffEvaluationAppService.GenerateStaffEvaluations();
        }

        [HttpPost]
        [Route("evaluate-staffs")]
        public Task EvaluateStaffs(StaffEvaluationRequest apiRequest)
        {
            return _staffEvaluationAppService.EvaluateStaffs(apiRequest);
        }
        
        [HttpGet]
        [Route("get-exports")]
        public Task<List<StaffEvaluationExportRow>> GetExportRows(GetStaffEvaluationsInput input)
        {
            return _staffEvaluationAppService.GetExportRows(input);
        }

        [HttpGet]
        [Route("seeding-evaluation-export")]
        public Task<List<PostDetailExportRow>> GetEvaluationSeedingPostExport(GetPostEvaluationRequest request)
        {
            return _staffEvaluationAppService.GetEvaluationSeedingPostExport(request);
        }

        [HttpGet]
        [Route("affiliate-evaluation-export")]
        public Task<byte[]> GetAffiliatesEvaluationExport(GetAffiliateEvaluationRequest request)
        {
            return _staffEvaluationAppService.GetAffiliatesEvaluationExport(request);
        }

        [HttpGet]
        [Route("tiktok-channel-evaluation-export")]
        public Task<List<TikTokChannelEvaluationExport>> GetEvaluationTiktokChannelExport(ExportTiktokEvaluationRequest request)
        {
            return _staffEvaluationAppService.GetEvaluationTiktokChannelExport(request);
        }

        [HttpGet]
        [Route("contract-evaluation-export")]
        public Task<List<ContractEvaluationExport>> GetContractEvaluationExport(GetContractEvaluationRequest request)
        {
            return _staffEvaluationAppService.GetContractEvaluationExport(request);
        }

        [HttpGet("evaluation-years")]
        public Task<List<int>> GetEvaluationYears()
        {
            return _staffEvaluationAppService.GetEvaluationYears();
        }

        [HttpGet("check-sale-team")]
        public Task<bool> ContainSaleTeam(params  string[] teamNames)
        {
            return _staffEvaluationAppService.ContainSaleTeam(teamNames);
        }
    }
}