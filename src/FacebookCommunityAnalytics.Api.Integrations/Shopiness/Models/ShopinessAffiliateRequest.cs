using System;

namespace FacebookCommunityAnalytics.Api.Integrations.Shopiness.Models
{
    public class ShopinessUnixTimeBase
    {
        private long _unixStartMs;
        private long _unixEndMs;

        public long UnixStartMs { get; set; }

        public long UnixEndMs { get; set; }
    }
    
    public class TrackingRequest
    {
        public string link { get; set; }
        public string subId1 { get; set; }
        public string subId2 { get; set; }
        public string subId3 { get; set; }
        public bool IsGdl { get; set; }
    }

    public class StatisticRequest : ShopinessUnixTimeBase
    {
        public string SubId1 { get; set; }
        
        // shortkey can be passed as a list in form of string: key1,key2,key3,etc. Maximun 100 keys
        public string ShortKey { get; set; }
        public string Shortlink { get; set; }
        public bool IsGdl { get; set; }
    }
    public class ConversionRequest : ShopinessUnixTimeBase
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10; 
        public bool IsGdl { get; set; }
    }
}