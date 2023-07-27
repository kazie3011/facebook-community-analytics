using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace FacebookCommunityAnalytics.Api.CmsGdl
{
    public interface ICmsGdlAppService
    {
        Task<CmsGdlLandingPageModel> GetLandingPageModel();
        Task<CmsGdlCommunityGroup> GetCommunityDetails(GroupCategoryType groupCategoryType);
    }

    public class CmsGdlLandingPageModel
    {
        public int GroupCount { get; set; }
        public int PageCount { get; set; }
        public int PartnerCount { get; set; }

        public List<BrandingCommunity> BrandingCommunities { get; set; }
        public List<CmsGdlFeatureCommunityGroup> FeatureCommunities { get; set; }

        public CmsGdlLandingPageModel()
        {
            BrandingCommunities = new List<BrandingCommunity>();
            FeatureCommunities = new List<CmsGdlFeatureCommunityGroup>();
        }

        public class BrandingCommunity
        {
            public string Fid { get; set; }
            public GroupSourceType GroupSourceType { get; set; }
            public string Name { get; set; }
            public int MemberCount { get; set; }
            public decimal GrowthPercentagePerMonth { get; set; }
            public int TotalReactionsPerMonth { get; set; }
            public string Url { get; set; }
        }

    }
    
    public class CmsGdlFeatureCommunityGroup
    {
        public string CategoryName { get; set; }
        public GroupCategoryType GroupCategoryType { get; set; }
    }

    public class CmsGdlCommunityGroup
    {
        public List<Item> Level1 { get; set; }
        public List<Item> Level2 { get; set; }
        public List<Item> Level3 { get; set; }

        public List<List<Item>> AllLevels { get; set; }

        public List<CmsGdlFeatureCommunityGroup> OtherCommunities { get; set; }

        public CmsGdlCommunityGroup()
        {
            OtherCommunities = new List<CmsGdlFeatureCommunityGroup>();
            Level1 = new List<Item>();
            Level2 = new List<Item>();
            Level3 = new List<Item>();
            AllLevels = new List<List<Item>>();
        }

        public class Item
        {
            public string Fid { get; set; }
            public GroupSourceType GroupSourceType { get; set; }
            public string Title { get; set; }
            public string ShortDescription { get; set; }
            public int MemberCount { get; set; }
            public decimal GrowthPercentagePerMonth { get; set; }
            public int TotalReactionsPerMonth { get; set; }
            public string Url { get; set; }
        }
    }
}