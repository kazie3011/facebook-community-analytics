using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Integrations.Shopiness.Models;
using Newtonsoft.Json;
using RestSharp;

namespace FacebookCommunityAnalytics.Api.Integrations.Shopiness
{
    public interface IShopinessApiConsumer
    {
        Task<ShopinessPayloadTrackingResponse> CreateTracking(TrackingRequest trackingRequest);
        Task<ShopinessPayloadStatisticResponse> GetStatistic(StatisticRequest request);
        Task<ShopinessPayloadConversionsResponse> GetConversion(ConversionRequest conversionRequest);
    }
    
    public class ShopinessApiConsumer : IShopinessApiConsumer
    {
        private readonly string _gdlToken;
        private readonly string _happyDayToken;
        private readonly string _rootApiUrl;
        
        public ShopinessApiConsumer()
        {
            _gdlToken = ShopinessApiConfig.GdlToken;
            _happyDayToken = ShopinessApiConfig.HappyDayToken;
            _rootApiUrl = ShopinessApiConfig.RootApiUrl;
        }
        
        private ApiResponse<T> HandleResponse<T>(IRestResponse response) where T : new()
        {
            var apiResponse = new ApiResponse<T>
            {
                Status = response.StatusCode
            };
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode ==HttpStatusCode.Created )
            {
                apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(response.Content);
            }
            else
            {
                //apiResponse.Error = new ApiError(response.Content);
            }

            return apiResponse;
        }

        private RestClient GetClient()
        {
            return new RestClient(_rootApiUrl);
        }

        private RestRequest GetRequest(string urlPath, Method method, bool isGdl)
        {
            var request = new RestRequest(urlPath, method);
            request.AddHeader("Authorization", isGdl ? $"Bearer {_gdlToken}" : $"Bearer {_happyDayToken}");
            request.AddHeader("Content-Type", "application/json");
            return request;
        }
        
        public async Task<ShopinessPayloadTrackingResponse> CreateTracking(TrackingRequest trackingRequest)
        {
            var client = GetClient();
            var request = GetRequest("affiliate/tracking", Method.POST, trackingRequest.IsGdl);
            request.AddJsonBody(new
            {
                trackingRequest.link,
                trackingRequest.subId1,
                trackingRequest.subId2,
                trackingRequest.subId3
            });
            
            var response = await client.ExecuteAsync(request);
            var apiResponse = HandleResponse<ShopinessPayloadTrackingResponse>(response);

            if (apiResponse.Payload != null)
            {
                apiResponse.Payload.CustomLink = apiResponse.Payload.CustomLink.Replace("shopiness", "gdll");
            }
            
            return apiResponse.Payload;
        }

        public async Task<ShopinessPayloadStatisticResponse> GetStatistic(StatisticRequest request)
        {
            var client = GetClient();
            var restRequest = GetRequest("affiliate/statistic", Method.GET, request.IsGdl);

            if (request.UnixStartMs > 0) restRequest.AddParameter("startDate", request.UnixStartMs);
            if (request.UnixEndMs > 0) restRequest.AddParameter("endDate", request.UnixEndMs);

            if (request.SubId1.IsNotNullOrWhiteSpace()) restRequest.AddParameter("subId1", request.SubId1);

            if (request.Shortlink.IsNotNullOrWhiteSpace()) restRequest.AddParameter("shortLink", request.Shortlink);
            if (request.ShortKey.IsNotNullOrWhiteSpace()) restRequest.AddParameter("shortKey", request.ShortKey);

            var response = await client.ExecuteAsync(restRequest);
#if DEBUG
            if (!response.IsSuccessful) Debug.WriteLine($"Error: {response.StatusCode}");
#endif
            var apiResponse = HandleResponse<ShopinessPayloadStatisticResponse>(response);
            
            return apiResponse.Payload;
        }

        public async Task<ShopinessPayloadConversionsResponse> GetConversion(ConversionRequest conversionRequest)
        {
            var client = GetClient();
            var request = GetRequest("affiliate/conversion", Method.GET, conversionRequest.IsGdl);
            request.AddParameter("page", conversionRequest.Page);
            request.AddParameter("size", conversionRequest.Size);
            request.AddParameter("conversionTimeStart", conversionRequest.UnixStartMs);
            request.AddParameter("conversionTimeEnd", conversionRequest.UnixEndMs);

            var response = await client.ExecuteAsync(request);
            var apiResponse = HandleResponse<ShopinessPayloadConversionsResponse>(response);
            return apiResponse.Payload;
        }
    }
}