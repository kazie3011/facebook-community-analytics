using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Core.Internal;
using CsvHelper;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using IdentityServer4.Extensions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.Console.Dev
{
    public partial class ContractService : LoggerService,ITransientDependency
    {
        private readonly IRepository<ContractTransaction, Guid> _contractTransactionRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IIdentityUserRepository _userRepository;

        public ContractService(
            IRepository<ContractTransaction, Guid> contractTransactionRepository,
            IContractRepository contractRepository,
            IIdentityUserRepository userRepository)
        {
            _contractTransactionRepository = contractTransactionRepository;
            _contractRepository = contractRepository;
            _userRepository = userRepository;
        }

        public async Task UpdateSalePersonTransaction()
        {
            var transactions = await _contractTransactionRepository.GetListAsync(_ => _.SalePersonId == null);
            var contracts = await _contractRepository.GetListExtendAsync();
            foreach (var transaction in transactions)
            {
                transaction.SalePersonId = contracts.FirstOrDefault(_ => _.Id == transaction.ContractId)?.SalePersonId;
            }

            await _contractTransactionRepository.UpdateManyAsync(transactions);
        }

        public async Task CleanContractsType()
        {
            var contracts = await _contractRepository.GetListAsync();
            foreach (var contract in contracts)
            {
                if (contract.ContractType == ContractType.NoSelect)
                {
                    contract.ContractType = ContractType.Unknown;
                }
            }

            if (contracts.IsNotNullOrEmpty())
            {
                await _contractRepository.UpdateManyAsync(contracts);
            }
        }

        public async Task ExportSaleTransaction(string saleName, int timeZone)
        {
            var user           = (await _userRepository.GetListAsync(_ => _.UserName == saleName)).FirstOrDefault();
            var transactions   = await _contractTransactionRepository.GetListAsync(_ => _.SalePersonId == user.Id);
            var contracts      = await _contractRepository.GetListExtendAsync(salePersonId: user?.Id);
            var contractModels = contracts.OrderBy(_ => _.CreatedAt)
                                          .Select
                                               (
                                                contract => new ExportContractModel
                                                {
                                                    SalePerson            = user?.UserName,
                                                    ContractCode          = contract.ContractCode,
                                                    Content               = contract.Content,
                                                    TotalValue            = contract.TotalValue.ToString(CultureInfo.InvariantCulture),
                                                    PartialPaymentValue   = contract.PartialPaymentValue.ToString(CultureInfo.InvariantCulture),
                                                    RemainingPaymentValue = contract.RemainingPaymentValue.ToString(CultureInfo.InvariantCulture),
                                                    VATPercent            = contract.VATPercent.ToString(CultureInfo.InvariantCulture),
                                                    CreatedAt             = contract.CreatedAt?.AddHours(timeZone).ToString("dd/MM/yyyy")
                                                }
                                               )
                                          .ToList();
            var transactionModels = transactions.OrderBy(_ => _.PaymentDueDate)
                                                .Select
                                                     (
                                                      transaction => new ExportTransactionModel
                                                      {
                                                          SalePerson          = user?.UserName,
                                                          ContractCode        = contracts.FirstOrDefault(_ => _.Id == transaction.ContractId)?.ContractCode,
                                                          Description         = transaction.Description,
                                                          PartialPaymentValue = transaction.PartialPaymentValue.ToString(CultureInfo.InvariantCulture),
                                                          PaymentDueDate      = transaction.PaymentDueDate?.AddHours(timeZone).ToString("dd/MM/yyyy"),
                                                          CreatedAt           = transaction.CreatedAt.AddHours(timeZone).ToString("dd/MM/yyyy"),
                                                          VATPercent          = transaction.VATPercent.ToString(CultureInfo.InvariantCulture)
                                                      }
                                                     )
                                                .ToList();

            // await using (var writer = new StreamWriter($"{saleName}-Contracts.csv"))
            // await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            // {
            //     csv.WriteHeader<ExportContractModel>();
            //     await csv.NextRecordAsync();
            //     foreach (var record in contractModels)
            //     {
            //         csv.WriteRecord(record);
            //         await csv.NextRecordAsync();
            //     }
            // }
            await using var writer2 = new StreamWriter($"{saleName}-Transactions.csv");
            await using var csv2    = new CsvWriter(writer2, CultureInfo.InvariantCulture);
            csv2.WriteHeader<ExportTransactionModel>();
            await csv2.NextRecordAsync();
            foreach (var record in transactionModels)
            {
                csv2.WriteRecord(record);
                await csv2.NextRecordAsync();
            }
        }

        public async Task UpdateUnknownContract()
        {
            var contracts = await _contractRepository.GetListAsync();
            foreach (var contract in contracts)
            {
                var currentTransactions = await _contractTransactionRepository.GetListAsync(_ => _.ContractId == contract.Id);
                var currentPartialPaymentValue = currentTransactions.Sum(x => x.PartialPaymentValue);
                if (contract.ContractType == ContractType.NoSelect)
                {
                    contract.ContractType = ContractType.Unknown;
                }

                if (contract.ContractType == ContractType.Unknown)
                {
                    if (contract.TotalValue < currentPartialPaymentValue)
                    {
                        contract.RemainingPaymentValue = 0;
                    }

                    contract.PartialPaymentValue = currentPartialPaymentValue;
                    contract.RemainingPaymentValue = contract.TotalValue - currentPartialPaymentValue;
                    if (contract.RemainingPaymentValue <= 0)
                    {
                        contract.ContractPaymentStatus = ContractPaymentStatus.Paid;
                        contract.RemainingPaymentValue = 0;
                    }
                    else
                    {
                        contract.ContractPaymentStatus = ContractPaymentStatus.PartiallyPaid;
                    }
                }
                await _contractRepository.UpdateAsync(contract, true);
            }
        }

        public async Task RemoveDupContract()
        {
            var contractNavs = await _contractRepository.GetListWithNavigationPropertiesAsync();
            var groupContracts = contractNavs.GroupBy(x => x.Contract.ContractCode).Where(x => x.Count() > 1 && !x.Key.ToLower().Contains("không"));

            foreach (var groupContract in groupContracts)
            {
                var items = groupContract.ToList();
                var contracts = groupContract.Where(_ => _.ContractTransactions.IsNotNullOrEmpty()).ToList();
                if (contracts.IsNullOrEmpty())
                {
                    items.RemoveAt(0);
                }

                items.Remove(contracts.OrderByDescending(_ => _.Contract.CreatedAt).First());
                foreach (var item in items)
                {
                    await _contractRepository.HardDeleteAsync(item.Contract);
                    foreach (var transaction in item.ContractTransactions)
                    {
                        await _contractTransactionRepository.DeleteAsync(transaction);
                    }
                }
            }
        }

        public async Task UpdatePaymentDateTimeContract()
        {
            var contracts = await _contractRepository.GetListAsync();
            foreach (var contract in contracts)
            {
                var transactions = await _contractTransactionRepository.GetListAsync(_ => _.ContractId == contract.Id);
                contract.PaymentDueDate = transactions.Max(x => x.PaymentDueDate);
            }

            foreach (var batch in contracts.Partition(100))
            {
                await _contractRepository.UpdateManyAsync(batch);
            }
        }
        
    }

    public class ExportContractModel
    {
        public string SalePerson { get; set; }
        public string ContractCode { get; set; }
        public string Content { get; set; }
        public string TotalValue { get; set; }
        public string PartialPaymentValue { get; set; }
        public string RemainingPaymentValue { get; set; }
        public string VATPercent { get; set; }
        public string CreatedAt { get; set; }
    }

    public class ExportTransactionModel
    {
        public string SalePerson          { get; set; }
        public string ContractCode        { get; set; }
        public string Description         { get; set; }
        public string PartialPaymentValue { get; set; }
        public string PaymentDueDate      { get; set; }
        public string CreatedAt           { get; set; }
        public string VATPercent          { get; set; }
    }
}