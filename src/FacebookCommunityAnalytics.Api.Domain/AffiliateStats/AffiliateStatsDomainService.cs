using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Integrations.Shopiness;
using FacebookCommunityAnalytics.Api.Integrations.Shopiness.Models;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml.DataValidation.Formulas.Contracts;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api.AffiliateStats
{
    public class SyncGeneralAffStatApiRequest
    {
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
    }

    public class SyncGeneralAffStatApiResponse
    {
    }

    public interface IAffiliateStatsDomainService : IDomainService
    {
        Task<SyncGeneralAffStatApiResponse> SyncGeneralAffStats(SyncGeneralAffStatApiRequest apiRequest);
        Task<AffDailySummaryApiResponse> GetDailyAffSummary(GeneralStatsApiRequest apiRequest);
        Task<AffTopLinkSummaryApiResponse> GetTopLinkAffSummary(GeneralStatsApiRequest apiRequest);
    }

    public class AffiliateStatsDomainService : BaseDomainService, IAffiliateStatsDomainService
    {
        private readonly IAffiliateStatRepository _affiliateStatRepository;
        private readonly IUserAffiliateRepository _userAffiliateRepository;
        private readonly IShopinessDomainService _shopinessDomainService;

        public AffiliateStatsDomainService(
            IAffiliateStatRepository affiliateStatRepository,
            IShopinessDomainService shopinessDomainService,
            IUserAffiliateRepository userAffiliateRepository)
        {
            _affiliateStatRepository = affiliateStatRepository;
            _shopinessDomainService = shopinessDomainService;
            _userAffiliateRepository = userAffiliateRepository;
        }

        // aff-toplink-summary
        public async Task<AffTopLinkSummaryApiResponse> GetTopLinkAffSummary(GeneralStatsApiRequest apiRequest)
        {
            var userAffiliates = await _userAffiliateRepository.GetListAsync(createdAtMin: apiRequest.FromDateTime, createdAtMax: apiRequest.ToDateTime);
            var userAffiliatesDto = ObjectMapper.Map<List<UserAffiliate>, List<AffTopLinkSummaryApiItem>>(userAffiliates);
            var affTopLinkSummaryApiResponse = new AffTopLinkSummaryApiResponse()
            {
                TopClicks = userAffiliatesDto.OrderByDescending(_ => _.Click).Take(300).ToList(),
                TopAmounts = userAffiliatesDto.OrderByDescending(_ => _.Amount).Take(300).ToList(),
                TopConversions = userAffiliatesDto.OrderByDescending(_ => _.Conversion).Take(300).ToList(),
            };
            return affTopLinkSummaryApiResponse;
        }

        // aff-daily-summary
        public async Task<AffDailySummaryApiResponse> GetDailyAffSummary(GeneralStatsApiRequest apiRequest)
        {
            var result = new AffDailySummaryApiResponse();
            var affiliateStats = await _affiliateStatRepository.GetListAsync
            (
                affiliateOwnershipType: AffiliateOwnershipType.GDL,
                clientOffsetInMinutes: apiRequest.ClientOffsetInMinutes,
                createdAtMin: apiRequest.FromDateTime,
                createdAtMax: apiRequest.ToDateTime
            );

            foreach (var item in affiliateStats.OrderBy(_ => _.CreatedAt))
            {
                result.Items.Add
                (
                    new AffDailySummaryItem
                    {
                        Display = item.CreatedAt.ToString("dd-MM"),
                        CreatedAt = item.CreatedAt,
                        Click = item.Click,
                        Amount = item.Amount,
                        Conversion = item.Conversion,
                        Commission = item.Commission,
                        CommissionBonus = item.CommissionBonus,
                    }
                );
            }

            return result;
        }

        public async Task<SyncGeneralAffStatApiResponse> SyncGeneralAffStats(SyncGeneralAffStatApiRequest apiRequest)
        {
            return await DoSyncGeneralAffStats(apiRequest);
        }

        private async Task<SyncGeneralAffStatApiResponse> DoSyncGeneralAffStats(SyncGeneralAffStatApiRequest apiRequest)
        {
            foreach (var from in apiRequest.FromDateTime.EachDay(apiRequest.ToDateTime))
            {
                var to = from.AddDays(1);
                var currents = await _affiliateStatRepository.GetListAsync(createdAtMin: from, createdAtMax: to.AddTicks(-1));
                Console.WriteLine($"SyncGeneralAffStats {from} - {to}");
                await DoSync(from, to, AffiliateOwnershipType.GDL, currents);
                await DoSync(from, to, AffiliateOwnershipType.HappyDay, currents);
                // NOTE: vu.nguyen comment this due to we need to overwrite the data as soon as possible
                // var currentGDL = currents.FirstOrDefault(_ => _.AffiliateOwnershipType == AffiliateOwnershipType.GDL);
                // if (currentGDL == null) { await DoSync(from, to, true); }
                //
                // var currentHPD = currents.FirstOrDefault(_ => _.AffiliateOwnershipType == AffiliateOwnershipType.HappyDay);
                // if (currentHPD == null) { await DoSync(from, to, false); }
            }

            return new SyncGeneralAffStatApiResponse();
        }

        // private async Task DoSync(DateTime startDateTime, DateTime endDateTime, bool isGDL, AffiliateStat current)
        private async Task DoSync(DateTime startDateTime, DateTime endDateTime, AffiliateOwnershipType affiliateOwnershipType, List<AffiliateStat> currentAffs)
        {
            var response = await _shopinessDomainService.GetStat
            (
                new ShopinessAffiliateStatRequest
                {
                    StartDate = startDateTime,
                    EndDate = endDateTime,
                    IsGDL = affiliateOwnershipType == AffiliateOwnershipType.GDL
                }
            );
            await Task.Delay(ShopinessApiConfig.ApiDelayInMs);

            if (response != null && response.ListData.IsNotNullOrEmpty())
            {
                ShopinessAffiliateStatistic first = new();
                foreach (var statistic in response.ListData)
                {
                    first = statistic;
                    break;
                }

                var current = currentAffs.FirstOrDefault(_ => _.AffiliateOwnershipType == affiliateOwnershipType);
                if (current != null)
                {
                    Console.WriteLine($"Update Affiliate Stat {current.CreatedAt}");
                    current.Click = first.Click;
                    current.Conversion = first.Conversion;
                    current.Amount = first.Value;
                    current.Commission = first.Commission;
                    current.CommissionBonus = first.CommissionBonus;
                    await _affiliateStatRepository.UpdateAsync(current);
                }
                else
                {
                    Console.WriteLine($"Insert Affiliate Stat {startDateTime}");
                    var affiliateStat = new AffiliateStat()
                    {
                        AffiliateOwnershipType = affiliateOwnershipType,
                        
                        Click = first.Click,
                        Conversion = first.Conversion,
                        Amount = first.Value,
                        Commission = first.Commission,
                        CommissionBonus = first.CommissionBonus,
                        
                        CreatedAt = startDateTime
                    };
                    await _affiliateStatRepository.InsertAsync(affiliateStat);
                }
            }
        }
    }
}