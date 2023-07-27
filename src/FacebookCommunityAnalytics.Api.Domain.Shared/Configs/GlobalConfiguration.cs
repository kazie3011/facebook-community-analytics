using System.Collections.Generic;
using System.Runtime.Serialization;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace FacebookCommunityAnalytics.Api.Configs
{
    public class GlobalConfiguration
    {
        public PartnerConfiguration PartnerConfiguration { get; set; }
        public string Environment { get; set; }
        public CrawlConfiguration CrawlConfiguration { get; set; }
        public GlobalPayrollConfiguration GlobalPayrollConfiguration { get; set; }
        public AffiliateConfiguration AffiliateConfiguration { get; set; }
        public EmailConfiguration EmailConfiguration { get; set; }
        public Services Services { get; set; }
        public LazadaConfiguration LazadaConfiguration { get; set; }
        public TikiConfiguration TikiConfiguration { get; set; }
        public BitlyConfiguration BitlyConfiguration { get; set; }
        public string MediaBaseUrl { get; set; }
        public List<string> EvaluationTeams { get; set; }
        public TeamTypeMapping TeamTypeMapping { get; set; }
        public SlackConfiguration SlackConfiguration { get; set; }
        public KPIConfig KPIConfig { get; set; }
        public ChartConfig ChartConfig { get; set; }

        public MediaConfiguration MediaConfiguration { get; set; }
        public MediaPath          MediaPath      { get; set; }
    }

    public class KPIConfig
    {
        public decimal CurrentRate { get; set; }
    }
    
    public class ChartConfig
    {
        public string TikTokChartHashTag { get; set; }
    }
    
    public class SlackConfiguration
    {
        public string WebhookUrl { get; set; }
    }

    public class PartnerConfiguration
    {
        public bool IsPartnerTool { get; set; }
    }

    public class CrawlConfiguration
    {
        public int IntervalHours { get; set; }
        public int IntervalCampaigns { get; set; }
        public int BatchSize { get; set; }
        public int AccountPerProxy { get; set; }
    }

    public class GlobalPayrollConfiguration
    {
        public int PayrollTimeZone { get; set; }
        public int PayrollStartDay { get; set; }
        public int PayrollEndDay { get; set; }
    }

    public class EmailConfiguration
    {
        public string ApiKey { get; set; }
        public string ApiValue { get; set; }

        public string AdminEmail { get; set; }
        public List<string> Recipients { get; set; }
        public List<string> Recipients_Test { get; set; }
        public List<string> Recipients_PayrollEmail { get; set; }
        public List<string> Recipients_AccountantEmail { get; set; }
        public List<string> Recipients_Sale { get; set; }
        public List<string> Recipients_CampaignEmail_Primary { get; set; }
        public List<string> Recipients_CampaignEmail_Secondary { get; set; }
        public List<string> Recipients_TiktokEmail { get; set; }
    }

    public class Services
    {
        public string IdentityUrl { get; set; }
        public string ApiUrl { get; set; }
        public string BackofficeUrl { get; set; }
    }

    public class LazadaConfiguration
    {
        public string BaseLink { get; set; }
        public string BaseProductLink { get; set; }
    }

    public class TikiConfiguration
    {
        public int ShortkeyLength { get; set; }
    }

    public class BitlyConfiguration
    {
        public string RootApiUrl { get; set; }
        public string AccessToken { get; set; }
    }

    public class AffiliateConfiguration
    {
        public int InitDayCount { get; set; }
    }

    public class TeamTypeMapping
    {
        public List<string> Sale { get; set; }
        public List<string> Content { get; set; }
        public List<string> Affiliate { get; set; }
        public List<string> Seeding { get; set; }
        public List<string> Tiktok { get; set; }
    }

    public class MediaConfiguration
    {
        public string TiktokRootPathChannel { get; set; }
        public string TiktokRootPathVideo { get; set; }
    }

    public class MediaPath
    {
        public string FilePath         { get; set; }
        public string FileContractPath { get; set; }
        public string ContentPath      { get; set; }
        public string AvatarPath       { get; set; }
        public string ThumpPath        { get; set; }
    }
}