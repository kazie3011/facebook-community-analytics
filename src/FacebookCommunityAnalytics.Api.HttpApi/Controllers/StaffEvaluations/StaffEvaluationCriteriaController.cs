using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.Controllers.StaffEvaluations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("StaffEvaluationCriteria")]
    [Route("api/app/staff-evaluation-criteria")]
    public class StaffEvaluationCriteriaController : AbpController, IStaffEvaluationCriteriaAppService
    {
        private readonly IStaffEvaluationCriteriaAppService _staffEvaluationCriteriaAppService;

        public StaffEvaluationCriteriaController(IStaffEvaluationCriteriaAppService staffEvaluationCriteriaAppService)
        {
            _staffEvaluationCriteriaAppService = staffEvaluationCriteriaAppService;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<StaffEvaluationCriteriaDto> GetAsync(Guid id)
        {
            return _staffEvaluationCriteriaAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("get-list-async")]
        public Task<PagedResultDto<StaffEvaluationCriteriaDto>> GetListAsync(GetStaffEvaluationCriteriaInput input)
        {
            return _staffEvaluationCriteriaAppService.GetListAsync(input);
        }

        [HttpPost]
        public Task<StaffEvaluationCriteriaDto> CreateAsync(CreateUpdateStaffEvaluationCriteriaDto input)
        {
            return _staffEvaluationCriteriaAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public Task<StaffEvaluationCriteriaDto> UpdateAsync(Guid id, CreateUpdateStaffEvaluationCriteriaDto input)
        {
            return _staffEvaluationCriteriaAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public Task DeleteAsync(Guid id)
        {
            return _staffEvaluationCriteriaAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("get-teams")]
        public Task<List<OrganizationUnitDto>> GetTeams()
        {
            return _staffEvaluationCriteriaAppService.GetTeams();
        }

        [HttpGet]
        [Route("get-list-extend-async")]
        public Task<PagedResultDto<StaffEvaluationCriteriaDto>> GetListExtendAsync(GetStaffEvaluationCriteriaInput input)
        {
            return _staffEvaluationCriteriaAppService.GetListExtendAsync(input);
        }
    }
}