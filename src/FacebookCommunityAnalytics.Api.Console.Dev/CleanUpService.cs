using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Posts;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Console.Dev
{
    public class CleanUpService : ITransientDependency
    {
        private readonly IPostRepository _postRepo;
        private readonly IContractRepository _contractRepo;

        private readonly IRepository<ContractTransaction, Guid> _transactionRepo;
        private readonly IRepository<Group, Guid> _groupRepo;

        private readonly ICampaignRepository _campaignRepository;

        public IPartnerRepository PartnerRepo { get; set; }

        public CleanUpService(
            IPostRepository postRepo,
            IContractRepository contractRepo,
            IRepository<ContractTransaction, Guid> transactionRepo,
            ICampaignRepository campaignRepository,
            IRepository<Group, Guid> groupRepo)
        {
            _postRepo = postRepo;
            _contractRepo = contractRepo;
            _transactionRepo = transactionRepo;
            _campaignRepository = campaignRepository;
            _groupRepo = groupRepo;
        }

        public async Task CleanContracts()
        {
            var contracts = await _contractRepo.GetListAsync();
            var transactions = await _transactionRepo.GetListAsync();

            var contractsToCreate = new List<ContractTransaction>();
            foreach (var contract in contracts)
            {
                if (contract.ContractPaymentStatus == ContractPaymentStatus.Paid)
                {
                    var contractTnxs = transactions.Where(x => x.ContractId == contract.Id).ToList();

                    if (contractTnxs.IsNullOrEmpty())
                    {
                        var transaction = new ContractTransaction
                        {
                            ContractId = contract.Id,
                            Description = $"Paid at {contract.CreatedAt}",
                            PartialPaymentValue = contract.TotalValue,
                            SalePersonId = contract.SalePersonId
                        };
                        contractsToCreate.Add(transaction);

                        Debug.WriteLine($"================== Found contract with no transaction: {contract.ContractCode} - {contract.TotalValue}");
                    }
                    else
                    {
                        var currentPaidAmount = contractTnxs.Sum(x => x.PartialPaymentValue);
                        var transaction = new ContractTransaction
                        {
                            ContractId = contract.Id,
                            Description = $"Paid at {contract.CreatedAt}",
                            PartialPaymentValue = contract.TotalValue - currentPaidAmount,
                            SalePersonId = contract.SalePersonId
                        };
                        contractsToCreate.Add(transaction);
                        Debug.WriteLine($"================== Found contract with NOT ENOUGH transaction: {contract.ContractCode} - {currentPaidAmount}/{contract.TotalValue}");
                    }
                }
            }
        }
        //
        // public async Task RemapCampaignPosts(string hashtag, string campaignCode)
        // {
        //     var posts = await _postRepo.GetListAsync(x => x.Hashtag.ToLower().Trim().Contains(hashtag.ToLower().Trim()));
        //     if (posts.IsNullOrEmpty()) return;
        //
        //     var camp = await _campaignRepository.FindAsync(x => x.Code == campaignCode);
        //     if (camp is null) return;
        //
        //     foreach (var post in posts)
        //     {
        //         post.CampaignId = camp.Id;
        //     }
        //
        //     await _postRepo.UpdateManyAsync(posts);
        // }

        public async Task CleanCampaignPosts()
        {
            var zalo = await PartnerRepo.FindAsync(x => x.Name == "Zalo");
            if (zalo is null)
            {
                return;
            }

            var camps = await _campaignRepository.GetListAsync(x => x.PartnerId == zalo.Id);
            foreach (var campaign in camps)
            {
                System.Console.WriteLine($"CleanCampaignPosts - {campaign.Name}");
            }

            var campGDL = camps.LastOrDefault();
            if (campGDL is null)
            {
                return;
            }

            var campGDLPosts = await _postRepo.GetListAsync(p => p.CampaignId == campGDL.Id);
            System.Console.WriteLine($"Camp post count: {campGDLPosts.Count}");

            var ghiendalatCamPostZalo = _postRepo.Where(x => x.Hashtag.ToLower().Contains("tetdalat")).ToList();
            System.Console.WriteLine($"ghiendalatCamPostZalo post count: {ghiendalatCamPostZalo.Count}");

            var groupIds = ghiendalatCamPostZalo.Where(x => x.GroupId != null).Select(x => x.GroupId.Value).Distinct().ToList();
            var groups = _groupRepo.Where(x => groupIds.Contains(x.Id)).ToList();
            System.Console.WriteLine($"ghiendalatCamPostZalo group : {string.Join(", ", groups.Select(x => x.Name).ToList())}");
        }
    }
}