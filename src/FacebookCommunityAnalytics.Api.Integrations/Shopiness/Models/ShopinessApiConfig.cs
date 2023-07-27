namespace FacebookCommunityAnalytics.Api.Integrations.Shopiness.Models
{
    public static class ShopinessApiConfig
    {
        //TODOO T.Anh: Vu.Nguyen - I think we should move this to globalconfis.json (note that there are 5 configs, need to update all 5 of them)
        public const int MaxApiBatchCount = 100;
        public const int ApiDelayInMs = 3000;
        public const string GdlToken = "FKY69lUxNzB2UW3fXmaDUw==";
        public const string HappyDayToken = "GPhudfxr/a3SHvpFR3Nfqg==";
        public const string RootApiUrl  = "https://plugin-api.shopiness.vn/";
    }
}