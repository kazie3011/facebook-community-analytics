using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Shared;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Groups
{
    public interface IGroupsAppService : IApplicationService
    {
        Task<PagedResultDto<GroupDto>> GetListAsync(GetGroupsInput input);

        Task<GroupDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<GroupDto> CreateAsync(GroupCreateDto input);

        Task<GroupDto> UpdateAsync(Guid id, GroupUpdateDto input);

        Task DeactivatedAsync(Guid id);
        Task ActivatedAsync(Guid id);
        Task<List<LookupDto<Guid>>> GetPartnerUserLookupAsync(LookupRequestDto input);
        
        Task<List<LookupDto<Guid>>> GetStaffUserLookupAsync(LookupRequestDto input);

        Task<string> GetGroupFids(Guid userId);
    }
}