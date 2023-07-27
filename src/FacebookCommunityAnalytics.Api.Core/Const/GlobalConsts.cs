using System.Collections.Generic;
using System.Linq;

namespace FacebookCommunityAnalytics.Api.Core.Const
{
    public static class GlobalConsts
    {
        public static int[] PAGE_SIZES_CONST = {10, 25, 50, 75, 100};
        public static int[] PAGE_SIZES_5_CONST = {5, 10, 25, 50, 75, 100};

        public static List<int> MONTH_OF_YEAR = Enumerable.Range(1, 12).ToList();

        public static string DateTimeFormat = "dd/MM/yyyy HH:mm";
        public static string DateFormat = "dd/MM/yyyy";

        public const string BaseAffiliateDomain = "https://gdl.vn";
        public const string GDLDomain = "https://gdll.vn";
        public const string HPDDomain = "happyday.sale";
        public const string YANDomain = "yin.vn";

        public const int ShopinessShortkeyLength = 6;

        public const string DefaultAvatar = "images/default-avatar.jpg";

        public const string TikTok = "TikTok";
        
        public const string Department = "Dept.";
        public const string NotApplicable = "N/A";
    }

    public static class EvaluationConsts
    {
        public static int QuantityMaxPoint = 50;
        public static int QualityMaxPoint = 30;
        public static int ReviewPoint = 20;
    }

    public static class RoleConsts
    {
        public const string Guest = "Guest";
        public const string Admin = "admin";
        public const string Manager = "Manager";
        public const string Leader = "Leader";
        public const string Staff = "Staff";
        public const string HR = "HR";
        public const string DirectorCommunity = "Director Community";
        public const string Sale = "Sale";
        public const string Supervisor = "Supervisor";
        public const string CampaignViewer = "Campaign Viewer";
        public const string CampaignCreator = "Campaign Creator";
        public const string Director = "Director";
        public const string Tiktok = "Tiktok";
        public const string Partner = "Partner";
        public const string SaleAdmin = "Sale Admin";
    }

    public static class TikiOrderStatus
    {
        public const string Confirmed = "1";
        public const string Rejected = "2";
        public const string Returned = "3";
    }

    public static class PartnerConst
    {
        public static string DefaultPartnerUserPassword = "GDLagency2022@";
    }
}