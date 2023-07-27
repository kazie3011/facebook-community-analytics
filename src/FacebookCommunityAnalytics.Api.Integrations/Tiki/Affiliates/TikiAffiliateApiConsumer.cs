using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using Microsoft.AspNetCore.WebUtilities;
using RestSharp;

namespace FacebookCommunityAnalytics.Api.Integrations.Tiki.Affiliates
{
    public interface ITikiAffiliateApiConsumer
    {
        TikiAffGetShortlink.Response GetShortlink(TikiAffGetShortlink.Request apiRequest);
        Task<TikiAffGetOrder.Response> GetOrder(TikiAffGetOrder.Request apiRequest);
    }

    public class TikiAffiliateApiConsumer : ITikiAffiliateApiConsumer
    {
        private const string MasterCampaignAffLink = "https://ti.ki/mGv0Xyfr/SCVX51JQ";
        private const string MasterCampaignAffLinkTest = "https://ti.ki/eNAY2pih/YG8F6EBZ";
        private const string BaseApiUrl = "https://affiliate.tiki.com.vn/open/v1";
        private const int GDLPublisherTier = 2;

        private const string Token = "84jjJkwzDkh9h2fhfUVuS9jZ8uVbhV3vC5AWX39IVUWSP2NcHciWvqZTa2N95RxR";

        private string _baseAffLink;

        public TikiAffiliateApiConsumer()
        {
            _baseAffLink = MasterCampaignAffLink;
#if DEBUG
            _baseAffLink = MasterCampaignAffLinkTest;
#endif

        }

        private RestClient GetClient()
        {
            return new RestClient(BaseApiUrl);
        }

        private RestRequest GetRequest(string urlPath, Method method)
        {
            var request = new RestRequest(urlPath, method);
            request.AddHeader("Authorization", $"Bearer {Token}");
            request.AddHeader("Content-Type", "application/json");
            return request;
        }

        // TODO Long: use campaign code
        // campaign: CP123456
        // partner: PN123456
        private string GetPublisherId(TikiAffGetShortlink.Request apiRequest)
        {
            var publisherId = $"GDL"
                              + (apiRequest.Shortlink.IsNotNullOrEmpty() ? $"-{UrlHelper.GetShortKey(apiRequest.Shortlink)}" : "-")
                              + $"-{apiRequest.UserCode}"
                              + (apiRequest.CommunityFid.IsNotNullOrEmpty() ? $"-{apiRequest.CommunityFid}" : "-0")
                              + (apiRequest.PartnerId.IsNotNullOrEmpty() ? $"-{apiRequest.PartnerId.Replace("-","")}" : "-0")
                              + (apiRequest.CampaignId.IsNotNullOrEmpty() ? $"-{apiRequest.CampaignId.Replace("-","")}" : "-0");
            return publisherId;
        }

        public TikiAffGetShortlink.Response GetShortlink(TikiAffGetShortlink.Request apiRequest)
        {
            string trackId = string.Empty;
            string identifyPublisher = GetPublisherId(apiRequest);
            // https://docs.google.com/document/d/1n1txLPYaTnQZTU85SVkqmgQP-0hX66epwGPrHlCy_zo/edit#
            var utmTerm = $"TAPM.{trackId}_TAPP.{identifyPublisher}_TAPT.TI{GDLPublisherTier}_TAPO.TIKI";

            var affUrl = $"{_baseAffLink}?"
                            + $"TIKI_URI={HttpUtility.UrlEncode(apiRequest.Link)}&"
                            + $"utm_term={utmTerm}";
            
            return new TikiAffGetShortlink.Response
            {
                success = true,
                Link = apiRequest.Link,
                // Shortlink = shortlink,
                AffiliateUrl = affUrl,
            };
        }

        /// <summary>
        /// https://docs.google.com/document/d/1ijDtl2R9K32RcdvjzF0QkO-DI1V7_iEl4QOwl9hIdws/edit#
        /// Notes: 
        /// Only allow partners to get data in the last 3 months. Ex: today is 31/07/2021 then only allow to get data from 30/04/2021.
        /// Response data is flat into product level. Ex: If the order has 2 products then it has 2 records returned.
        ///     How to determines which status of order is latest:
        /// - If date_key is different => the status code on newer date_key
        /// - If date_key is the same => the bigger status code
        /// Status of order:
        /// 1: Confirmed
        /// 2: Rejected
        /// 3: Returned
        /// </summary>
        /// <param name="apiRequest"></param>
        /// <returns></returns>
        public async Task<TikiAffGetOrder.Response> GetOrder(TikiAffGetOrder.Request apiRequest)
        {
            var apiUrl = $"{BaseApiUrl}/orders";
            
            var param = new Dictionary<string, string>()
            {
                { "limit", apiRequest.limit.ToString() },
                { "offset", apiRequest.offset.ToString() },
                { "date", apiRequest.dateString },
            };
            var newUrl = new Uri(QueryHelpers.AddQueryString(apiUrl, param));
            
            var client = GetClient();
            var request = GetRequest(newUrl.ToString(), Method.GET);
            
            var response = await client.ExecuteAsync<TikiAffGetOrder.Response>(request);
            return response.Data;
        }
    }

    public class TikiAffResponseBase
    {
        // public object data { get; set; }
        public string error { get; set; }
        public bool success { get; set; }
    }

    public class TikiAffPagination
    {
        public int limit { get; set; }
        public int offset { get; set; }
        public int total { get; set; }
    }

    public class TikiAffGetShortlink
    {
        public class Request
        {
            public string CommunityFid { get; set; }
            public string PartnerId { get; set; }
            public string CampaignId { get; set; }
            public string UserCode { get; set; }
            public string Link { get; set; }
            public string Shortlink { get; set; }
        }

        public class Response : TikiAffResponseBase
        {
            public string Shortlink { get; set; }
            public string Link { get; set; }
            public string AffiliateUrl { get; set; }
        }
    }

    public class TikiAffGetOrder
    {
        public class Request
        {
            /// <summary>
            /// default 10, maximum 10000 rows return per request
            /// </summary>
            public int limit { get; set; }

            /// <summary>
            /// Default 0
            /// </summary>
            public int offset { get; set; }

            public DateTime Date { get; set; }

            /// <summary>
            /// yyyy-MM-dd. Example: 2021-06-01. Default is the day make request
            /// </summary>
            public string dateString => Date.ToString("yyyy-MM-dd");

            public Request()
            {
                limit = 1000;
            }
        }

        public class Response : TikiAffResponseBase
        {
            public List<TikiAffOrder> data { get; set; }
            public TikiAffPagination pagination { get; set; }
        }
    }

    public class TikiAffOrder
    {
        public string date_key { get; set; }
        public string transaction_time { get; set; }
        public string event_id { get; set; }
        public string utm_term { get; set; }
        public string offer_id { get; set; }
        public string offer_name { get; set; }
        public string status { get; set; }
        public string platform_name { get; set; }
        public string geo_region_iso_code { get; set; }
        public string order_code { get; set; }
        public string sub_order_code { get; set; }
        public string tier { get; set; }
        public int product_amount { get; set; }
        public int discount { get; set; }
        public int quantity { get; set; }
        public string sku { get; set; }
        public string product_name { get; set; }
        public string category_id { get; set; }
        public string category_name { get; set; }
        public string cate_id_exclude { get; set; }
        public string cate_name_exclude { get; set; }
        public string brand_id { get; set; }
        public string brand_name { get; set; }
        public string seller_id { get; set; }
        public string seller_name { get; set; }
        public int commission_fee { get; set; }
        public int sponsor_commission_fee { get; set; }
        public int new_customer { get; set; }
    }
}