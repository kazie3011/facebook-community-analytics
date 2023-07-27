using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.ApiConfigurations;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Integrations.Shopiness.Models;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using Volo.Abp.Domain.Services;
using UrlHelper = FacebookCommunityAnalytics.Api.Core.Helpers.UrlHelper;

namespace FacebookCommunityAnalytics.Api.Integrations.Shopiness
{
    public interface IShopinessDomainService : IDomainService
    {
        Task<ShopinessAffiliateStatResponse> GetStat(ShopinessAffiliateStatRequest request);
        Task<ShopinessPayloadTrackingResponse> CreateTracking(TrackingRequest trackingRequest);
        Task<ShopinessPayloadConversionsResponse> GetConversion(ShopinessAffiliateConversionRequest request);
        Task<List<ShopinessConversion>> GetConversions(ShopinessAffiliateConversionRequest request);
    }

    public class ShopinessDomainService : BaseDomainService, IShopinessDomainService
    {
        private readonly IShopinessApiConsumer _apiConsumer;

        public ShopinessDomainService(IShopinessApiConsumer apiConsumer)
        {
            _apiConsumer = apiConsumer;
        }

        public async Task<ShopinessAffiliateStatResponse> GetStat(ShopinessAffiliateStatRequest request)
        {
            long unixTimeStart = request.StartDate.UtcToOffsetOrDefault().ToUnixTimeMilliseconds();
            long unixTimeEnd = request.EndDate.UtcToOffsetOrDefault().ToUnixTimeMilliseconds();

            var shortKeyBatch = string.Empty;
            if (request.Shortlinks.IsNotNullOrEmpty())
            {
                var shortKeys = request.Shortlinks.Select(UrlHelper.GetShortKey);
                shortKeyBatch = string.Join(',', shortKeys);
            }

            var statistic = await _apiConsumer.GetStatistic
            (
                new StatisticRequest
                {
                    UnixStartMs = unixTimeStart,
                    UnixEndMs = unixTimeEnd,
                    SubId1 = request.UserCode,
                    //Shortlink = shortLink,
                    ShortKey = shortKeyBatch,
                    IsGdl = request.IsGDL
                }
            );
            if (statistic == null || statistic.ListData.IsNullOrEmpty())
            {
                Console.WriteLine
                (
                    $"{DateTime.Now} =================================SHOPINESS: affiliate/statistic {request.StartDate} to {request.EndDate} - Empty Response: {shortKeyBatch.MaybeSubstring(10)}..."
                );
                return null;
            }

#if DEBUG
            foreach (var data in statistic.ListData)
            {
                if (data.Click > 0)
                {
                    Console.WriteLine
                    (
                        $"{DateTime.Now} =================================SHOPINESS: affiliate/statistic {request.StartDate} to {request.EndDate} - {shortKeyBatch.MaybeSubstring(10)}... "
                        + (request.IsGDL ? $"{GlobalConsts.BaseAffiliateDomain}/{data.Key}" : $"{GlobalConsts.HPDDomain}/{data.Key}")
                        + $"{JsonConvert.SerializeObject(data)}"
                    );
                }
            }
#endif

            return new ShopinessAffiliateStatResponse
            {
                Date = statistic.Date,
                ListData = statistic.ListData
            };
        }
        
        public Task<ShopinessPayloadTrackingResponse> CreateTracking(TrackingRequest trackingRequest)
        {
            return _apiConsumer.CreateTracking(trackingRequest);
        }

        public async Task<ShopinessPayloadConversionsResponse> GetConversion(ShopinessAffiliateConversionRequest request)
        {
            long unixTimeStart = request.StartDate.UtcToOffsetOrDefault().ToUnixTimeMilliseconds();
            long unixTimeEnd = request.EndDate.UtcToOffsetOrDefault().ToUnixTimeMilliseconds();

            var conversionsResponse = await _apiConsumer.GetConversion
            (
                new ConversionRequest
                {
                    UnixStartMs = unixTimeStart,
                    UnixEndMs = unixTimeEnd,
                    Page = request.Page,
                    Size = request.Size,
                    IsGdl = request.IsGDL
                }
            );
            if (conversionsResponse == null || conversionsResponse.ListData.IsNullOrEmpty())
            {
                Console.WriteLine
                (
                    $"{DateTime.Now} =================================SHOPINESS: affiliate/conversion {request.StartDate} to {request.EndDate} Page: {request.Page} Size: {request.Size}  - Empty Response: ..."
                );
                return null;
            }

            Console.WriteLine
            (
                $"{DateTime.Now} =================================SHOPINESS: affiliate/conversion {request.StartDate} to {request.EndDate} - Page: {conversionsResponse.Page} Size: {conversionsResponse.Size} ConversionCount {conversionsResponse.ListData.Count} conversionsResponse {conversionsResponse.Total}"
            );

            return new ShopinessPayloadConversionsResponse
            {
                ListData = conversionsResponse.ListData,
                Total = conversionsResponse.Total,
                Page = conversionsResponse.Page,
                Size = conversionsResponse.Size
            };
        }

        public async Task<List<ShopinessConversion>> GetConversions(ShopinessAffiliateConversionRequest request)
        {
            var conversions = new List<ShopinessConversion>();
            var page = 1;
            var pageSize = 200;
            do
            {
                var stopWatch = Stopwatch.StartNew();
                var res = await GetConversion
                (
                    new ShopinessAffiliateConversionRequest()
                    {
                        StartDate = request.StartDate,
                        EndDate = request.EndDate,
                        Page = page,
                        Size = pageSize,
                        IsGDL = request.IsGDL
                    }
                );
                if (res != null && res.ListData.IsNotNullOrEmpty())
                {
                    conversions.AddRange(res.ListData);
                }

                if (res != null && res.Total / pageSize < res.Page)
                {
                    break;
                }

                page++;
                stopWatch.Stop();

                var timeTakenInMs = stopWatch.ElapsedMilliseconds;
                if (timeTakenInMs < ShopinessApiConfig.ApiDelayInMs + 100)
                {
                    var apiDelayInMs = ShopinessApiConfig.ApiDelayInMs + 100 - timeTakenInMs;
                    await Task.Delay((int) apiDelayInMs);
                }
            } while (true);

            return conversions;
        }
        
    }

    public class ShopinessAffiliateStatResponse : ShopinessPayloadStatisticResponse
    {
    }

    public class ShopinessAffiliateStatRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<string> Shortlinks { get; set; }
        public string UserCode { get; set; }

        public bool IsGDL { get; set; }
    }

    public class ShopinessAffiliateConversionRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }

        public bool IsGDL { get; set; }
    }
}