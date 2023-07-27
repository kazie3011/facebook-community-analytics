using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Exceptions;
using IdentityServer4.Extensions;
using FacebookCommunityAnalytics.Api.GroupCosts;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api.Services
{
    public interface IContractDomainService : IDomainService
    {
        Task<Contract> CreateContractAsync(Contract input);
        Task<Contract> UpdateContractAsync(Guid id, Contract input);
        Task<ContractTransaction> CreateTransactionAsync(ContractTransaction input);
        Task<ContractTransaction> UpdateTransactionAsync(ContractTransaction input);
        Task DeleteContractTransactionAsync(Guid transactionId);
    }

    public class ContractDomainService : BaseDomainService, IContractDomainService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IRepository<ContractTransaction> _contractTransactionRepository;
        private readonly IGroupCostRepository _groupCostRepository;

        public ContractDomainService(IContractRepository contractRepository, 
                                     IRepository<ContractTransaction> contractTransactionRepository,
                                     IGroupCostRepository groupCostRepository)
        {
            _contractRepository = contractRepository;
            _contractTransactionRepository = contractTransactionRepository;
            _groupCostRepository = groupCostRepository;
        }

        public async Task<Contract> CreateContractAsync(Contract input)
        {
            AugmentContract(input, true);
            
            var result = await _contractRepository.InsertAsync(input);

            if (result.Id != Guid.Empty)
            {
                if (input.ContractPaymentStatus == ContractPaymentStatus.Paid)
                {
                    var transaction = new ContractTransaction
                    {
                        ContractId = result.Id,
                        Description = $"Paid at {input.CreatedAt}",
                        PartialPaymentValue = input.TotalValue,
                        SalePersonId = input.SalePersonId
                    };
                    await _contractTransactionRepository.InsertAsync(transaction);

                    result.RemainingPaymentValue = 0;
                    await _contractRepository.UpdateAsync(result);
                }
            }

            return result;
        }

        public async Task<Contract> UpdateContractAsync(Guid id, Contract input)
        {
            AugmentContract(input, false);

            return await _contractRepository.UpdateAsync(input);
        }

        private void AugmentContract(Contract input, bool isCreation)
        {
            if (isCreation)
            {
                input.CreatedAt = DateTime.UtcNow;
                input.RemainingPaymentValue = input.TotalValue;
            }
            else
            {
                // todo update logic here
            }
        }

        public async Task<ContractTransaction> CreateTransactionAsync(ContractTransaction input)
        {
            if (input.PartialPaymentValue <= 0)
            {
                throw new ApiException(message: LD[ApiDomainErrorCodes.Contract.ContractRequiresTnxAmount]);
            }   

            var contract = await _contractRepository.FindAsync(_ => _.Id == input.ContractId);
            if (contract == null)
            {
                throw new ApiException(message: LD[ApiDomainErrorCodes.Contract.ContractNotFound]);
            }

            input.SalePersonId = contract.SalePersonId;
            await AugmentContract(contract, input, AugmentContractType.Create);
            await UpdateContractAsync(input.ContractId,contract);
            var result = await _contractTransactionRepository.InsertAsync(input, true);
            return result;
        }

        public async Task<ContractTransaction> UpdateTransactionAsync(ContractTransaction input)
        {
            if (input.PartialPaymentValue <= 0)
            {
                throw new ApiException(message: LD[ApiDomainErrorCodes.Contract.ContractRequiresTnxAmount]);
            }
            
            if (input.Id == Guid.Empty)
            {
                throw new ApiException(message: LD[ApiDomainErrorCodes.Contract.InvalidTransaction]);
            }

            var contract = await _contractRepository.FindAsync(_ => _.Id == input.ContractId);
            if (contract == null)
            {
                throw new ApiException(message: LD[ApiDomainErrorCodes.Contract.ContractNotFound]);
            }

            input.SalePersonId = contract.SalePersonId;

            await AugmentContract(contract, input, AugmentContractType.Update);
            var result = await _contractTransactionRepository.UpdateAsync(input, true);
            await UpdateContractAsync(input.ContractId,contract);
            return result;
        }

        public async Task DeleteContractTransactionAsync(Guid transactionId)
        {
            var transaction = await _contractTransactionRepository.FindAsync(x => x.Id == transactionId);
            if (transaction == null)
            {
                throw new ApiException(message: LD[ApiDomainErrorCodes.Transaction.TransactionNotFound]);
            }

            var contract = await _contractRepository.FindAsync(_ => _.Id == transaction.ContractId);
            if (contract == null)
            {
                throw new ApiException(message: LD[ApiDomainErrorCodes.Contract.ContractNotFound]);
            }
            await AugmentContract(contract, transaction, AugmentContractType.Delete);
            await _contractRepository.UpdateAsync(contract, true);
            await _contractTransactionRepository.DeleteAsync(transaction, true);
        }

        private async Task AugmentContract(Contract contract, ContractTransaction transaction, AugmentContractType augmentContractType)
        {
            if (contract is null || transaction is null) return;

            var currentTransactions = await _contractTransactionRepository.GetListAsync(_ => _.ContractId == contract.Id);
            
            switch (augmentContractType)
            {
                case AugmentContractType.Create:
                    currentTransactions.Add(transaction);
                    break;
                case AugmentContractType.Update:
                    var oldTransaction = currentTransactions.Find(x => x.Id == transaction.Id);
                    if (oldTransaction is null)
                    {
                        throw new ApiException(message: LD[ApiDomainErrorCodes.Contract.InvalidTransaction]);
                    }
                    currentTransactions.Remove(oldTransaction);
                    currentTransactions.Add(transaction);
                    break;
                case AugmentContractType.Delete:
                    currentTransactions.RemoveAll(_ => _.Id == transaction.Id);
                    break;
            }
            
            var currentPartialPaymentValue = currentTransactions.Sum(x => x.PartialPaymentValue);
            
            if (contract.ContractType.IsIn(ContractType.Master))
            {
                contract.TotalValue = currentPartialPaymentValue;
                contract.Cost = currentTransactions.Sum(x => x.Cost);
            }
            else
            {
                if (contract.TotalValue < currentPartialPaymentValue)
                {
                    throw new ApiException(message: LD[ApiDomainErrorCodes.Transaction.TransactionAmountInvalid]);
                }

                contract.PartialPaymentValue = currentPartialPaymentValue;
                contract.RemainingPaymentValue = contract.TotalValue - currentPartialPaymentValue;
                if (contract.RemainingPaymentValue <= 0)
                {
                    contract.ContractPaymentStatus = ContractPaymentStatus.Paid;
                    contract.RemainingPaymentValue = 0;

                    transaction.Description = $"Paid at {transaction.CreatedAt}";
                }
                else
                {
                    contract.ContractPaymentStatus = ContractPaymentStatus.PartiallyPaid;
                }

                if (currentTransactions.IsNullOrEmpty()) contract.ContractPaymentStatus = ContractPaymentStatus.Unpaid;
            }
            contract.PaymentDueDate = currentTransactions.Max(x => x.PaymentDueDate);
        }
    }
}