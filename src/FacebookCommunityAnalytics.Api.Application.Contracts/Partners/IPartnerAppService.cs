using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Shared;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FacebookCommunityAnalytics.Api.Partners
{
    public interface IPartnersAppService : IApplicationService
    {
        Task<PagedResultDto<PartnerDto>> GetListAsync(GetPartnersInput input);

        Task<PartnerDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<PartnerDto> CreateAsync(PartnerCreateDto input);

        Task<PartnerDto> UpdateAsync(Guid id, PartnerUpdateDto input);
        Task<List<LookupDto<Guid>>> GetPartnerUserLookupAsync(LookupRequestDto input);
    }
}