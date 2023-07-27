using Newtonsoft.Json;
using System;

namespace FacebookCommunityAnalytics.Api.Integrations.Lazada.Models
{
    public class BitlyAffiliateResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("custom_bitlinks")]
        public string[] CustomBitLinks { get; set; }

        [JsonProperty("long_url")]
        public string LongUrl { get; set; }

        [JsonProperty("archived")]
        public bool Archived { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("deeplinks")]
        public string[] DeepLinks { get; set; }

        [JsonProperty("references")]
        public BitlyReferences References { get; set; }

        public class BitlyReferences
        {
            [JsonProperty("group")]
            public string Group { get; set; }
        }
    }


}
