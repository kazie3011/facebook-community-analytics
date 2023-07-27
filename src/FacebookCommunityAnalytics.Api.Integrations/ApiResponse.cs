using System.Net;
using System.Text.Json.Serialization;

namespace FacebookCommunityAnalytics.Api.Integrations
{
    public class ApiResponse<T>
    {
        [JsonIgnore]
        public HttpStatusCode Status { get; set; }
        public bool Success { get; set; }
        public T Payload { get; set; }
    }
}
