using System;
using System.Collections.Generic;
using System.Linq;
using FacebookCommunityAnalytics.Api.AffiliateStats;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.UserAffiliates
{
    public class UserAffSummaryApiRequest
    {
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public AffiliateOwnershipType? AffiliateOwnershipType { get; set; }
    }

    public class UserAffSummaryApiResponse
    {
        public int Click { get; set; }
        public int Conversion { get; set; }
        public decimal Amount { get; set; }
        public decimal Commission { get; set; }

        public List<Item> Items { get; set; }

        public UserAffSummaryApiResponse()
        {
            Items = new List<Item>();
        }

        public class Item
        {
            public string UserCode { get; set; }
            public string UserDisplayName { get; set; }
            public string Team { get; set; }
            public int Click { get; set; }
            public int Conversion { get; set; }
            public decimal Amount { get; set; }
            public decimal Commission { get; set; }
        }
    }

    public class AffTopLinkSummaryApiResponse
    {
        public List<AffTopLinkSummaryApiItem> TopClicks { get; set; }
        public List<AffTopLinkSummaryApiItem> TopConversions { get; set; }
        public List<AffTopLinkSummaryApiItem> TopAmounts { get; set; }
    }

    public class AffTopLinkSummaryApiItem : AffiliateModel
    {
        public string Shortlink { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FacebookUrl { get; set; }
    }

    public class SyncUserAffConversionEmailModel
    {
        public int ShortlinkCount { get; set; }
        public List<UserAffiliateDto> GDL { get; set; }
        public List<UserAffiliateDto> HPD { get; set; }
        public TimeSpan ApiTimeTaken { get; set; }

        public List<UserAffiliateDto> GetAllAffs()
        {
            return GDL.Union(HPD).ToList();
        }
    }
    
    public class AffiliateStatEmailModel
    {
        public List<UserAffiliateDto> Affiliates { get; set; } = new List<UserAffiliateDto>();
        public TimeSpan ApiTimeTaken { get; set; }
    }
}