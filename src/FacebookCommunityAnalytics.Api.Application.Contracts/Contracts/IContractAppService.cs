using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.Application.Dtos;
using FacebookCommunityAnalytics.Api.GroupCosts;

namespace FacebookCommunityAnalytics.Api.Contracts
{
    public interface IContractAppService : IBaseApiCrudAppService<ContractDto, Guid, GetContractsInput, CreateUpdateContractDto>
    {
        Task<PagedResultDto<ContractWithNavigationPropertiesDto>>            GetPageContractNavs(GetContractsInput input);
        Task<List<ContractWithNavigationPropertiesDto>>                      GetContractNavs(GetContractsInput     input);
        Task<PagedResultDto<LookupDto<Guid?>>>                               GetPartnerLookup(LookupRequestDto     input);
        Task<List<CampaignDto>>                                              GetCampaigns();
        Task<List<ContractTransactionWithNavigationPropertiesDto>>           GetTransactionsByContractId(Guid                     contractId);
        Task<ContractTransactionDto>                                         CreateTransaction(CreateUpdateContractTransactionDto input);
        Task<ContractTransactionDto>                                         UpdateTransaction(Guid                               id, CreateUpdateContractTransactionDto input);
        Task                                                                 DeleteTransaction(Guid                               transactionId);
        Task<List<LookupDto<Guid?>>>                                         GetAppUserLookupAsync(GetMembersApiRequest           input);
        Task<List<ContractTransactionDto>>                                   GetTransactions(GetContractTransactionInput          input);
        Task<PagedResultDto<ContractTransactionWithNavigationPropertiesDto>> GetTransactionsWithNav(GetContractTransactionInput   input);
        Task<List<GroupCostDto>>                                             GetGroupCosts();
        Task                                                                 SaveGroupCosts(GroupCostApiRequest request);
    }
}