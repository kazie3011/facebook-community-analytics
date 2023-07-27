using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Integrations.Lazada.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace FacebookCommunityAnalytics.Api.Integrations.Lazada
{
    public interface ILazadaApiConsumer
    {
        Task<BitlyAffiliateResponse> GenerateBitlyAffiliate(string longLink);
    }

    public class LazadaApiConsumer : ILazadaApiConsumer, ITransientDependency
    {
        private readonly BitlyConfiguration _bitlyConfiguration;

        public LazadaApiConsumer(GlobalConfiguration globalConfiguration)
        {
            _bitlyConfiguration = globalConfiguration.BitlyConfiguration;
        }

        public async Task<BitlyAffiliateResponse> GenerateBitlyAffiliate(string longUrl)
        {
            var client = GetClient();
            var request = CreateRequest("shorten", Method.POST);

            request.AddJsonBody(new
            {
                long_url = longUrl
            });

            try
            {
                //  var response = await client.PostAsync<BitlyAffiliateResponse>(request);
                var response = await client.ExecuteAsync(request);

                var result = HandleResponse<BitlyAffiliateResponse>(response);

                return result;
            }
            catch (Exception ex)
            {
                return new BitlyAffiliateResponse
                {
                    LongUrl = longUrl
                };
            }
        }

        private RestRequest CreateRequest(string urlPath, Method method)
        {
            var request = new RestRequest(urlPath, method);
            request.AddHeader("Authorization", $"Bearer {_bitlyConfiguration.AccessToken}");
            request.AddHeader("Content-Type", "application/json");
            return request;
        }

        protected RestClient GetClient()
        {
            return new RestClient(_bitlyConfiguration.RootApiUrl);
        }

        private T HandleResponse<T>(IRestResponse response) where T : new()
        {
            var apiResponse = new T();
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                apiResponse = JsonConvert.DeserializeObject<T>(response.Content);
            }

            return apiResponse;
        }
    }
}
