using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Shared;

namespace FacebookCommunityAnalytics.Api.Controllers.Partners
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Partner")]
    [Route("api/app/partners")]

    public class PartnerController : AbpController, IPartnersAppService
    {
        private readonly IPartnersAppService _partnersAppService;

        public PartnerController(IPartnersAppService partnersAppService)
        {
            _partnersAppService = partnersAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<PartnerDto>> GetListAsync(GetPartnersInput input)
        {
            return _partnersAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<PartnerDto> GetAsync(Guid id)
        {
            return _partnersAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<PartnerDto> CreateAsync(PartnerCreateDto input)
        {
            return _partnersAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<PartnerDto> UpdateAsync(Guid id, PartnerUpdateDto input)
        {
            return _partnersAppService.UpdateAsync(id, input);
        }

        [HttpGet]
        [Route("get-partner-user-lookup")]
        public Task<List<LookupDto<Guid>>> GetPartnerUserLookupAsync(LookupRequestDto input)
        {
            return _partnersAppService.GetPartnerUserLookupAsync(input);
        }
        
        

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _partnersAppService.DeleteAsync(id);
        }
    }
}