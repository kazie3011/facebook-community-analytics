using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.GroupCosts;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.TeamMembers;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace FacebookCommunityAnalytics.Api.Controllers.Contracts
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Contract")]
    [Route("api/app/contracts")]
    public class ContractController : AbpController, IContractAppService
    {
        private readonly IContractAppService _contractAppService;

        public ContractController(IContractAppService contractAppService)
        {
            _contractAppService = contractAppService;
        }

        [HttpGet]
        [Route("get-list-async")]
        public virtual Task<PagedResultDto<ContractDto>> GetListAsync(GetContractsInput input)
        {
            return _contractAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<ContractDto> GetAsync(Guid id)
        {
            return _contractAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<ContractDto> CreateAsync(CreateUpdateContractDto input)
        {
            return _contractAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<ContractDto> UpdateAsync(Guid id, CreateUpdateContractDto input)
        {
            return _contractAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _contractAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("get-page-contract-navs")]
        public Task<PagedResultDto<ContractWithNavigationPropertiesDto>> GetPageContractNavs(GetContractsInput input)
        {
            return _contractAppService.GetPageContractNavs(input);
        }
        
        [HttpGet]
        [Route("get-contract-navs")]
        public virtual Task<List<ContractWithNavigationPropertiesDto>> GetContractNavs(GetContractsInput input)
        {
            return _contractAppService.GetContractNavs(input);
        }
        
        [HttpGet]
        [Route("get-partner-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetPartnerLookup(LookupRequestDto input)
        {
            return _contractAppService.GetPartnerLookup(input);
        }

        [HttpGet]
        [Route("get-campaigns")]
        public Task<List<CampaignDto>> GetCampaigns()
        {   
            return _contractAppService.GetCampaigns();
        }

        [HttpGet]
        [Route("get-list-transaction")]
        public Task<List<ContractTransactionWithNavigationPropertiesDto>> GetTransactionsByContractId(Guid contractId)
        {
            return _contractAppService.GetTransactionsByContractId(contractId);
        }

        [HttpPost]
        [Route("create-transaction")]
        public Task<ContractTransactionDto> CreateTransaction(CreateUpdateContractTransactionDto input)
        {
            return _contractAppService.CreateTransaction(input);
        }

        [HttpPut]
        [Route("update-transaction")]
        public Task<ContractTransactionDto> UpdateTransaction(Guid id, CreateUpdateContractTransactionDto input)
        {
            return _contractAppService.UpdateTransaction(id, input);
        }

        [HttpDelete]
        [Route("delete-transaction")]
        public Task DeleteTransaction(Guid transactionId)
        {
            return _contractAppService.DeleteTransaction(transactionId);
        }

        [HttpGet]
        [Route("get-app-user-lookup")]
        public Task<List<LookupDto<Guid?>>> GetAppUserLookupAsync(GetMembersApiRequest input)
        {
            return _contractAppService.GetAppUserLookupAsync(input);
        }

        [HttpGet]
        [Route("get-transactions")]
        public Task<List<ContractTransactionDto>> GetTransactions(GetContractTransactionInput input)
        {
            return _contractAppService.GetTransactions(input);
        }

        [HttpGet]
        [Route("get-transactions-with-nav")]
        public async Task<PagedResultDto<ContractTransactionWithNavigationPropertiesDto>> GetTransactionsWithNav(GetContractTransactionInput input)
        {
            return await _contractAppService.GetTransactionsWithNav(input);
        }
        
        
        [HttpGet]
        [Route("get-active-group-costs")]
        public async Task<List<GroupCostDto>> GetGroupCosts()
        {
            return await _contractAppService.GetGroupCosts();
        }
        
        [HttpPost]
        [Route("save-group-costs")]
        public async Task SaveGroupCosts(GroupCostApiRequest request)
        {
             await _contractAppService.SaveGroupCosts(request);
        }
    }
}