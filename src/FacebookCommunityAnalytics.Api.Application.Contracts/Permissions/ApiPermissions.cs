using System.CodeDom;
using LdapForNet;

namespace FacebookCommunityAnalytics.Api.Permissions
{
    public static class ApiPermissions
    {
        public const string GroupName = "Api";

        public static class Dashboard
        {
            public const string DashboardGroup = GroupName + ".Dashboard";
            public const string Host = DashboardGroup + ".Host";
            public const string Tenant = DashboardGroup + ".Tenant";
        }

        //Add your own permission names. Example:
        //public const string MyPermission1 = GroupName + ".MyPermission1";
        public class Systems
        {
            public const string Default = GroupName + ".Systems";
        }
        
        public class Groups
        {
            public const string Default = GroupName + ".Groups";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }
        
        public class Categories
        {
            public const string Default = GroupName + ".Categories";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class PostGroup
        {
            public const string Default = GroupName + ".PostGroup";
        }
        public class Posts
        {
            public const string Default = GroupName + ".Posts";
            public const string Edit = Default + ".Edit";
            public const string EditNote = Default + ".EditNote";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Proxies
        {
            public const string Default = GroupName + ".Proxies";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class ScheduledPosts
        {
            public const string Default = GroupName + ".ScheduledPosts";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Accounts
        {
            public const string Default = GroupName + ".Accounts";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class AccountProxies
        {
            public const string Default = GroupName + ".AccountProxies";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Crawl
        {
            public const string Default = GroupName + ".Crawl";
        }

        public class UserInfos
        {
            public const string Default = GroupName + ".UserInfos";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Payrolls
        {
            public const string Default = GroupName + ".Payrolls";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class UserPayrollCommissions
        {
            public const string Default = GroupName + ".UserPayrollCommissions";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class UserPayrolls
        {
            public const string Default = GroupName + ".UserPayrolls";
            public const string ViewDetail = GroupName + ".ViewDetail";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class UserWaves
        {
            public const string Default = GroupName + ".UserWaves";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class UserPayrollBonuses
        {
            public const string Default = GroupName + ".UserPayrollBonuses";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class UncrawledPosts
        {
            public const string Default = GroupName + ".UncrawledPosts";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class PayrollsConfiguration
        {
            public const string Default = GroupName + ".PayrollsConfiguration";
        }

        public class PaySlip
        {
            public const string Default = GroupName + ".PaySlip";
        }

        public class Dev
        {
            public const string Default = GroupName + ".Dev";
        }

        public static class Notification
        {
            public const string Default = GroupName + ".Notifications";
            public const string ReceiveMessages = GroupName + ".ReceiveMessages";
        }

        public static class ApiConfigurations
        {
            public const string Default = GroupName + ".ApiConfigurations";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public static class UserAffiliates
        {
            public const string Default = GroupName + ".UserAffiliates";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Bookings
        {
            
            public const string Default = GroupName + ".Bookings";
        }
        public class Partners
        {
            public const string Default = GroupName + ".Partners";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Campaigns
        {
            public const string Default = GroupName + ".Campaigns";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
            public const string Export = Default + ".Export";
        }

        public class AffiliateStats
        {
            public const string Default = GroupName + ".AffiliateStats";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Index
        {
            public const string Default = GroupName + ".Index";
        }

        public class Tiktok
        {
            public const string Default = GroupName + ".Tiktok";
            public const string Dashboard = GroupName + ".Dashboard";
            public const string MNC = GroupName + ".MCN";
            public const string Channels = GroupName + ".Channels";
            public const string Reports = GroupName + ".Reports";
        }

        public class TeamMembers
        {
            public const string Default = GroupName + ".TeamMembers";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }


        public class Contracts
        {
            public const string Default = GroupName + ".Contracts";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class ContractTransactions
        {
            public const string Default = GroupName + ".ContractTransactions";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }
        
        public class Content
        {
            public const string Default = GroupName + ".Contents";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class StaffEvaluations
        {
            public const string Default = GroupName + ".StaffEvaluations";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public static class UserEvaluationConfigurations
        {
            public const string Default = GroupName + ".UserEvaluationConfigurations";
            public const string ConfigSale = Default + ".Sale";
            public const string ConfigTiktok = Default + ".Tiktok";
            public const string ConfigContent = Default + ".Content";
            public const string ConfigCommunity = Default + ".Community";
        }

        public static class BoD
        {
            public const string Default = GroupName + ".BoD";
            public const string Director = Default + ".Director";
            public const string Manager = Default + ".Manager";
            public const string Leader = Default + ".Leader";
        }
        
        public static class UserCompensations
        {
            public const string Default = GroupName + ".UserCompensations";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }
        public static class UserBonusConfigs
        {
            public const string Default = GroupName + ".UserBonusConfigs";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }
        
        public static class UserSalaries
        {
            public const string Default = GroupName + ".UserSalaryHistories";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }
        
        public static class UserSalaryConfiguration
        {
            public const string Default = GroupName + ".UserSalaryConfigurations";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }
        
        public class PartnerModule
        {
            public const string Default = GroupName + ".PartnerModule";
            public const string PartnerDashboard = Default + ".PartnerDashboard";
            public const string PartnerCampaigns = Default + ".PartnerCampaigns";
            public const string PartnerContracts = Default + ".PartnerContracts";
            public const string PartnerCommunities = Default + ".PartnerCommunities";
            public const string PartnerPosts = Default + ".PartnerPosts";
        }
    }

    public static class CmsPermissions
    {
        public const string GroupName = "Cms";

        public class Sites
        {
            public const string Default = GroupName + ".Sites";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }
    }
}