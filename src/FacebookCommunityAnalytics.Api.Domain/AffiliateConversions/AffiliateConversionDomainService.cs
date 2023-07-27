using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Integrations.Shopiness;
using FacebookCommunityAnalytics.Api.Integrations.Shopiness.Models;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api.AffiliateConversions
{
    public interface IAffiliateConversionDomainService : IDomainService
    {
        Task SyncAffStats(int monthOld, AffiliateOwnershipType affiliateOwner, DateTime fromDateTime, DateTime toDateTime);
        Task SyncAffConversions(DateTime fromDate, DateTime toDate, AffiliateOwnershipType affiliateOwner);
    }

    public class AffiliateConversionDomainService : BaseDomainService, IAffiliateConversionDomainService
    {
        private readonly IUserAffiliateRepository _userAffiliateRepository;
        private readonly IAffiliateConversionRepository _affiliateConversionRepository;
        private readonly IShopinessDomainService _shopinessDomainService;

        public AffiliateConversionDomainService(
            IUserAffiliateRepository userAffiliateRepository,
            IAffiliateConversionRepository affiliateConversionRepository,
            IShopinessDomainService shopinessDomainService)
        {
            _userAffiliateRepository = userAffiliateRepository;
            _affiliateConversionRepository = affiliateConversionRepository;
            _shopinessDomainService = shopinessDomainService;
        }

        public async Task SyncAffStats(int monthOld, AffiliateOwnershipType affiliateOwner, DateTime fromDateTime, DateTime toDateTime)
        {
            var outdatedAffs = _userAffiliateRepository.Where
                (
                    x => x.AffiliateUrl != null
                         && x.CreatedAt.HasValue
                         && x.CreatedAt > DateTime.UtcNow.AddMonths(-monthOld)
                         && x.AffiliateOwnershipType == affiliateOwner
                )
                .ToList();

            var shortLinks = new List<string>();
            switch (affiliateOwner)
            {
                case AffiliateOwnershipType.GDL:
                    shortLinks = outdatedAffs.Where
                            (x => x.AffiliateUrl.Contains(GlobalConsts.BaseAffiliateDomain) || x.AffiliateUrl.Contains(GlobalConsts.GDLDomain))
                        .Select(x => x.AffiliateUrl)
                        .ToList();
                    break;
                case AffiliateOwnershipType.HappyDay:
                    shortLinks = outdatedAffs.Where(x => x.AffiliateUrl.Contains(GlobalConsts.HPDDomain)).Select(x => x.AffiliateUrl).ToList();
                    break;
                case AffiliateOwnershipType.Unknown:
                    break;
            }

            var partitions = shortLinks.Partition(ShopinessApiConfig.MaxApiBatchCount).ToList();
            var stack = new Stack<IEnumerable<string>>(partitions);

            while (true)
            {
                if (stack.TryPop(out var list))
                {
                    Console.WriteLine($"SyncAffs: current stack {stack.Count}");
                    await DoSyncAffStats
                    (
                        outdatedAffs,
                        list.ToList(),
                        affiliateOwner,
                        fromDateTime,
                        toDateTime
                    );
                }
                else
                {
                    break;
                }
            }
        }

        private async Task DoSyncAffStats(
            List<UserAffiliate> outdatedAffs,
            List<string> shortLinks,
            AffiliateOwnershipType affiliateOwner,
            DateTime fromDateTime,
            DateTime toDateTime)
        {
            if (shortLinks.IsNullOrEmpty()) { return; }

            var stopWatch = Stopwatch.StartNew();

            var gdlAffiliateResponse = await _shopinessDomainService.GetStat
            (
                new ShopinessAffiliateStatRequest
                {
                    StartDate = fromDateTime,
                    EndDate = toDateTime,
                    Shortlinks = shortLinks,
                    IsGDL = affiliateOwner == AffiliateOwnershipType.GDL
                }
            );

            stopWatch.Stop();
            // await Task.Delay(ShopinessApiConfig.ApiDelayInMs);

            if (gdlAffiliateResponse != null && gdlAffiliateResponse.ListData.IsNotNullOrEmpty())
            {
                foreach (var item in gdlAffiliateResponse.ListData)
                {
                    var existing = outdatedAffs.FirstOrDefault(x => x.AffiliateUrl.Contains(item.Key));
                    if (existing == null) { continue; }
                    
                    Console.WriteLine($"--------------Update User Affiliate ---- Short Link: {existing.AffiliateUrl} ------- Click: {item.Click}");
                    existing.UpdatedAt = DateTime.UtcNow;
                    existing.AffConversionModel.ClickCount += item.Click;

                    await _userAffiliateRepository.UpdateAsync(existing);
                }
            }

            var timeTakenInMs = stopWatch.ElapsedMilliseconds;
            if (timeTakenInMs < ShopinessApiConfig.ApiDelayInMs + 100)
            {
                var apiDelayInMs = ShopinessApiConfig.ApiDelayInMs + 100 - timeTakenInMs;
                await Task.Delay((int) apiDelayInMs);
            }
        }

        public async Task SyncAffConversions(DateTime fromDate, DateTime toDate, AffiliateOwnershipType affiliateOwner)
        {
            // only get collection from last 3 months

            // var existingUserAffs = await _userAffiliateRepository.GetListAsync(createdAtMin: DateTime.UtcNow.AddMonths(-3));
            try
            {
                var existingConversions = await _affiliateConversionRepository.GetListAsync(_ => _.ConversionTime >= fromDate.ConvertToUnixTimestamp());

                // switch (affiliateOwner)
                // {
                //     case AffiliateOwnershipType.Unknown: break;
                //     case AffiliateOwnershipType.GDL:
                //         existingUserAffs = existingUserAffs.Where(x => x.AffiliateUrl.Contains(GlobalConsts.BaseAffiliateDomain) || x.AffiliateUrl.Contains(GlobalConsts.GDLDomain)).ToList();
                //         break;
                //     case AffiliateOwnershipType.HappyDay: 
                //         existingUserAffs = existingUserAffs.Where(x => x.AffiliateUrl.Contains(GlobalConsts.HPDDomain)).ToList();
                //         break;
                //     case AffiliateOwnershipType.YAN: 
                //         existingUserAffs = existingUserAffs.Where(x => x.AffiliateUrl.Contains(GlobalConsts.YANDomain)).ToList();
                //         break;
                //     default: throw new ArgumentOutOfRangeException(nameof(affiliateOwner), affiliateOwner, null);
                // }

            var conversions = await _shopinessDomainService.GetConversions
            (
                new ShopinessAffiliateConversionRequest
                {
                    StartDate = fromDate,
                    EndDate = toDate,
                    IsGDL = affiliateOwner == AffiliateOwnershipType.GDL
                }
            );
            var conversionGroups = conversions.GroupBy(_ => _.ShortKey).ToList();

            foreach (var conversionGroup in conversionGroups)
            {
                var newConversions = new List<AffiliateConversion>();
                var shortKey = conversionGroup.Key;
                var items = conversionGroup.ToList();
                if (items.IsNullOrEmpty())
                {
                    continue;
                }

                foreach (var item in items.Where(item => item != null))
                {
                    Console.WriteLine($"-----------------ShopinessId: {item.ConversionItemId}-------------------------");
                    var existingConversion = existingConversions.FirstOrDefault(_ => _.ConversionItemId == item.ConversionItemId);
                    if (existingConversion == null)
                    {
                        Console.WriteLine($"Insert New Conversion-----------------ShopinessId: {item.ConversionItemId}-------------------------");
                        newConversions.Add
                        (
                            new AffiliateConversion
                            {
                                ConversionItemId = item.ConversionItemId,
                                ConversionId = item.ConversionId,
                                Status = item.Status,
                                SaleAmount = item.SaleAmount,
                                Payout = item.Payout,
                                PayoutBonus = item.PayoutBonus,
                                ConversionTime = item.ConversionTime,
                                Platform = item.Platform,
                                SubId1 = item.SubId1,
                                SubId2 = item.SubId2,
                                SubId3 = item.SubId3,
                                ShopId = item.ShopId,
                                ShortKey = item.ShortKey,
                                ShopName = item.ShopName,
                                ProductName = item.ProductName,
                                CategoryName = item.CategoryName,
                                Campaign = item.Campaign,
                                IsHappyDay = affiliateOwner == AffiliateOwnershipType.HappyDay
                            }
                        );
                    }
                }

                // 1. store new conversions
                if (newConversions.IsNotNullOrEmpty())
                {
                    foreach (var batch in newConversions.Partition(1000))
                    {
                        await _affiliateConversionRepository.InsertManyAsync(batch);
                    }
                    // var existingUserAff = existingUserAffs.FirstOrDefault
                    // (
                    //     _ => string.Equals
                    //     (
                    //         UrlHelper.GetShortKey(_.AffiliateUrl),
                    //         shortKey,
                    //         StringComparison.CurrentCultureIgnoreCase
                    //     )
                    // );

                    // 2. find existing user aff and update the stats
                    // if (existingUserAff != null)
                    // {
                    //     Debug.WriteLine($"----------------Update User AFF: {shortKey}-----------------");
                    //     existingUserAff.AffConversionModel.ConversionCount += newConversions.Count;
                    //     existingUserAff.AffConversionModel.ConversionAmount += newConversions.Sum(_ => _.SaleAmount);
                    //     existingUserAff.AffConversionModel.CommissionAmount += newConversions.Sum(_ => _.Payout);
                    //     existingUserAff.AffConversionModel.CommissionBonusAmount += newConversions.Sum(_ => _.PayoutBonus);
                    //
                    //     var firstConversion = items.FirstOrDefault();
                    //     var marketplaceType = firstConversion.Campaign.IsNotNullOrEmpty() && firstConversion.Campaign.Contains("lazada") ? MarketplaceType.Lazada
                    //         :firstConversion.Campaign.IsNotNullOrEmpty() && firstConversion.Campaign.Contains("tiki")  ? MarketplaceType.Tiki
                    //         : MarketplaceType.Shopee;
                    //     existingUserAff.MarketplaceType = marketplaceType;
                    //
                    //     await _userAffiliateRepository.UpdateAsync(existingUserAff);
                    // }
                }
            }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}