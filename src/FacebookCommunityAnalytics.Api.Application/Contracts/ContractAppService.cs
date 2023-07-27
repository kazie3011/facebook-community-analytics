using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Transactions;
using Dasync.Collections;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Exceptions;
using FacebookCommunityAnalytics.Api.GroupCosts;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Localization;
using FacebookCommunityAnalytics.Api.Medias;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.Users;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Contracts
{
    [RemoteService(isEnabled: false)]
    [Authorize(ApiPermissions.Contracts.Default)]
    public class ContractAppService : BaseCrudApiAppService<Contract, ContractDto, Guid, GetContractsInput, CreateUpdateContractDto>, IContractAppService
    {
        private readonly IContractRepository            _contractRepository;
        private readonly ICampaignRepository            _campaignRepository;
        private readonly IPartnerRepository             _partnerRepository;
        private readonly IRepository<Media, Guid>       _mediaRepository;
        private readonly IRepository<AppUser, Guid>       _userRepository;
        private readonly IContractTransactionRepository _contractTransactionRepository;
        private readonly ITeamMemberDomainService       _teamMemberDomainService;
        private readonly IContractDomainService         _contractDomainService;
        private readonly IGroupCostRepository           _groupCostRepository;

        public ContractAppService(
            IRepository<Contract, Guid>    repository,
            IContractRepository            contractRepository,
            ICampaignRepository            campaignRepository,
            IPartnerRepository             partnerRepository,
            IRepository<Media, Guid>       mediaRepository,
            IContractTransactionRepository contractTransactionRepository,
            ITeamMemberDomainService       teamMemberDomainService,
            IContractDomainService         contractDomainService,
            IGroupCostRepository           groupCostRepository,
            IRepository<AppUser, Guid>     userRepository) : base(repository)
        {
            _contractRepository            = contractRepository;
            _campaignRepository            = campaignRepository;
            _partnerRepository             = partnerRepository;
            _mediaRepository               = mediaRepository;
            _contractTransactionRepository = contractTransactionRepository;
            _teamMemberDomainService       = teamMemberDomainService;
            _contractDomainService         = contractDomainService;
            _groupCostRepository           = groupCostRepository;
            _userRepository           = userRepository;
        }

        public async Task<PagedResultDto<ContractWithNavigationPropertiesDto>> GetPageContractNavs(GetContractsInput input)
        {
            var count = await _contractRepository.GetCountAsync
                            (
                             input.FilterText,
                             input.CreatedAtMin,
                             input.CreatedAtMax,
                             input.SignedAtMin,
                             input.SignedAtMax,
                             input.ContractStatus,
                             input.ContractPaymentStatus,
                             input.SalePersonId,
                             input.PartnerIds
                            );
            var items = await _contractRepository.GetListWithNavigationPropertiesAsync
                            (
                             input.FilterText,
                             input.CreatedAtMin,
                             input.CreatedAtMax,
                             input.SignedAtMin,
                             input.SignedAtMax,
                             input.ContractStatus,
                             input.ContractPaymentStatus,
                             input.SalePersonId,
                             input.PartnerIds,
                             input.Sorting,
                             input.MaxResultCount,
                             input.SkipCount
                            );
            return new PagedResultDto<ContractWithNavigationPropertiesDto>()
            {
                TotalCount = count,
                Items      = ObjectMapper.Map<List<ContractWithNavigationProperties>, List<ContractWithNavigationPropertiesDto>>(items)
            };
        }

        public async Task<List<ContractWithNavigationPropertiesDto>> GetContractNavs(GetContractsInput input)
        {
            var items = await _contractRepository.GetListWithNavigationPropertiesAsync
                            (
                             input.FilterText,
                             input.CreatedAtMin,
                             input.CreatedAtMax,
                             input.SignedAtMin,
                             input.SignedAtMax,
                             input.ContractStatus,
                             input.ContractPaymentStatus,
                             input.SalePersonId,
                             input.PartnerIds
                            );
            return ObjectMapper.Map<List<ContractWithNavigationProperties>, List<ContractWithNavigationPropertiesDto>>(items);
        }

        public override async Task<ContractDto> GetAsync(Guid id)
        {
            var contractDto = await base.GetAsync(id);
            if (contractDto.MediaIds.IsNullOrEmpty())
            {
                return contractDto;
            }

            var medias = _mediaRepository.Where(x => contractDto.MediaIds.Contains(x.Id)).ToList();
            contractDto.MediasDtos.AddRange(ObjectMapper.Map<List<Media>, List<MediaDto>>(medias));

            return contractDto;
        }

        public override async Task<ContractDto> CreateAsync(CreateUpdateContractDto input)
        {
            var contract = await base.MapToEntityAsync(input);
            var result   = await _contractDomainService.CreateContractAsync(contract);
            return await base.MapToGetOutputDtoAsync(result);
        }

        public override async Task<ContractDto> UpdateAsync(Guid id, CreateUpdateContractDto input)
        {
            var contractEntity = await base.GetEntityByIdAsync(id);
            await base.MapToEntityAsync(input, contractEntity);
            var result = await _contractDomainService.UpdateContractAsync(id, contractEntity);
            return await base.MapToGetOutputDtoAsync(result);
        }

        public override async Task DeleteAsync(Guid id)
        {
            var transactions = await _contractTransactionRepository.GetListAsync(_ => _.ContractId == id);
            await _contractTransactionRepository.DeleteManyAsync(transactions);
            await base.DeleteAsync(id);
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetPartnerLookup(LookupRequestDto input)
        {
            var query      = _partnerRepository.AsQueryable().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), x => x.Name != null && x.Name.Contains(input.Filter));
            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Partner>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid?>>
            {
                TotalCount = totalCount,
                Items      = ObjectMapper.Map<List<Partner>, List<LookupDto<Guid?>>>(lookupData)
            };
        }

        public virtual async Task<List<CampaignDto>> GetCampaigns()
        {
            var campaigns = _campaignRepository.AsQueryable().WhereIf(CurrentUser.IsInRole(RoleConsts.Partner), x => x.CreatorId == CurrentUser.Id).ToList();
            return ObjectMapper.Map<List<Campaign>, List<CampaignDto>>(campaigns);
        }

        [Authorize(ApiPermissions.ContractTransactions.Default)]
        public async Task<List<ContractTransactionWithNavigationPropertiesDto>> GetTransactionsByContractId(Guid contractId)
        {
            var transactions = await _contractTransactionRepository.GetListWithNavigationPropertiesAsync(contractId: contractId);
            return ObjectMapper.Map<List<ContractTransactionWithNavigationProperties>, List<ContractTransactionWithNavigationPropertiesDto>>(transactions);
        }

        [Authorize(ApiPermissions.ContractTransactions.Create)]
        public async Task<ContractTransactionDto> CreateTransaction(CreateUpdateContractTransactionDto input)
        {
            var transaction = ObjectMapper.Map<CreateUpdateContractTransactionDto, ContractTransaction>(input);
            transaction = await _contractDomainService.CreateTransactionAsync(transaction);
            return ObjectMapper.Map<ContractTransaction, ContractTransactionDto>(transaction);
        }

        [Authorize(ApiPermissions.ContractTransactions.Edit)]
        public async Task<ContractTransactionDto> UpdateTransaction(Guid id, CreateUpdateContractTransactionDto input)
        {
            var transaction = await _contractTransactionRepository.FindAsync(id);
            if (transaction == null)
            {
                throw new BusinessException(message: LD[ApiDomainErrorCodes.Transaction.TransactionNotFound]);
            }

            ObjectMapper.Map(input, transaction);
            var result = await _contractDomainService.UpdateTransactionAsync(transaction);
            return ObjectMapper.Map<ContractTransaction, ContractTransactionDto>(result);
        }

        [Authorize(ApiPermissions.ContractTransactions.Delete)]
        public virtual async Task DeleteTransaction(Guid transactionId)
        {
            await _contractDomainService.DeleteContractTransactionAsync(transactionId);
        }

        public async Task<List<LookupDto<Guid?>>> GetAppUserLookupAsync(GetMembersApiRequest input)
        {
            return await _teamMemberDomainService.GetAppUserLookupAsync(input);
        }

        public async Task<List<ContractTransactionDto>> GetTransactions(GetContractTransactionInput input)
        {
            return ObjectMapper.Map<List<ContractTransaction>, List<ContractTransactionDto>>
                (
                 await _contractTransactionRepository.GetListExtendAsync
                     (
                      input.FilterText,
                      input.Description,
                      input.PartialPaymentValue,
                      input.PaymentDueDateMin,
                      input.PaymentDueDateMax,
                      input.CreatedAtMin,
                      input.CreatedAtMax,
                      input.ContractId,
                      input.SalePersonId
                     )
                );
        }

        public async Task<PagedResultDto<ContractTransactionWithNavigationPropertiesDto>> GetTransactionsWithNav(GetContractTransactionInput input)
        {
            var transactions = await _contractTransactionRepository.GetListWithNavigationPropertiesAsync
                                   (
                                    contractId: input.ContractId,
                                    salePersonId: input.SalePersonId,
                                    paymentDueDateMin: input.PaymentDueDateMin,
                                    paymentDueDateMax: input.PaymentDueDateMax,
                                    filterText: input.FilterText
                                   );

            var partners        = await _partnerRepository.GetListAsync();
            var campaigns       = await _campaignRepository.GetListAsync();
            var saleUsers       = await _userRepository.GetListAsync();
            transactions = transactions.Select
                                        (
                                         _ =>
                                         {
                                             if (_.Contract is null) return _;

                                             _.ContractPartner    = partners.FirstOrDefault(p => p.Id == _.Contract.PartnerId);
                                             _.ContractCampaign   = campaigns.FirstOrDefault(p => p.Id == _.Contract.CampaignId);
                                             _.ContractSalePerson = saleUsers.FirstOrDefault(p => p.Id == _.Contract.SalePersonId);
                                             return _;
                                         }
                                        )
                                       .ToList();
            return new PagedResultDto<ContractTransactionWithNavigationPropertiesDto>()
            {
                TotalCount = transactions.Count,
                Items      = ObjectMapper.Map<List<ContractTransactionWithNavigationProperties>, List<ContractTransactionWithNavigationPropertiesDto>>(transactions),
            };
        }

        public async Task<List<GroupCostDto>> GetGroupCosts()
        {
            var groupCosts = await _groupCostRepository.GetListAsync();
            return ObjectMapper.Map<List<GroupCost>, List<GroupCostDto>>(groupCosts);
        }

        public async Task SaveGroupCosts(GroupCostApiRequest request)
        {
            var requestGroupCosts = ObjectMapper.Map<List<GroupCostDto>, List<GroupCost>>
                (
                 request.GroupCosts.Select
                             (
                              _ =>
                              {
                                  _.GroupName = _.GroupName.Trim();
                                  return _;
                              }
                             )
                        .ToList()
                );
            var activeGroupCosts  = new List<GroupCost>();
            var disableGroupCosts = new List<GroupCost>();
            var groupCosts        = await _groupCostRepository.GetListAsync();
            foreach (var item in groupCosts)
            {
                // Case 1: request group cost still in DB (group cost just update or no change)
                var activeGroupCost = requestGroupCosts.FirstOrDefault(_ => _.Id == item.Id);
                if (activeGroupCost is null)
                {
                    // Case 2: request group cost same name with disable group Cost (group cost is disable but request had same group cost name so we just active old group cost)
                    var remakeGroupCost = requestGroupCosts.FirstOrDefault(_ => _.GroupName == item.GroupName);
                    if (remakeGroupCost is null)
                    {
                        // Case 3: request group cost not contain records in DB so disable all records(soft delete)
                        item.Disable = true;
                        disableGroupCosts.Add(item);
                        continue;
                    }

                    requestGroupCosts.Remove(remakeGroupCost);
                    item.Cost    = remakeGroupCost.Cost;
                    item.Disable = false;
                    activeGroupCosts.Add(item);
                    continue;
                }

                requestGroupCosts.Remove(activeGroupCost);
                item.GroupName = activeGroupCost.GroupName;
                item.Cost      = activeGroupCost.Cost;
                item.Disable   = false;
                activeGroupCosts.Add(item);
            }

            var editGroupCosts = activeGroupCosts.Concat(disableGroupCosts);
            foreach (var batch in editGroupCosts.Partition(100))
            {
                await _groupCostRepository.UpdateManyAsync(batch);
            }

            // Case 4: new group cost
            var newGroupCosts = requestGroupCosts.Where(_ => _.Id == Guid.Empty).ToList();
            foreach (var batch in newGroupCosts.Partition(100))
            {
                await _groupCostRepository.InsertManyAsync(batch);
            }
        }
    }
}