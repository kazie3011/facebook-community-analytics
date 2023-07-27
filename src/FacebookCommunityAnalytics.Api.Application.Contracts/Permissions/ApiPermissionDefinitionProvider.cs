using FacebookCommunityAnalytics.Api.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace FacebookCommunityAnalytics.Api.Permissions
{
    public class ApiPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(ApiPermissions.GroupName);

            myGroup.AddPermission(ApiPermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
            myGroup.AddPermission(ApiPermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);
            
            myGroup.AddPermission(ApiPermissions.Index.Default, L("Permission:Index"));
            
            myGroup.AddPermission(ApiPermissions.Systems.Default, L("Permission:Systems"));
            //Define your own permissions here. Example:
            //myGroup.AddPermission(ApiPermissions.MyPermission1, L("Permission:MyPermission1"));

            var groupPermission = myGroup.AddPermission(ApiPermissions.Groups.Default, L("Permission:Groups"));
            groupPermission.AddChild(ApiPermissions.Groups.Create, L("Permission:Create"));
            groupPermission.AddChild(ApiPermissions.Groups.Edit, L("Permission:Edit"));
            groupPermission.AddChild(ApiPermissions.Groups.Delete, L("Permission:Delete"));

            var categoryPermission = myGroup.AddPermission(ApiPermissions.Categories.Default, L("Permission:Categories"));
            categoryPermission.AddChild(ApiPermissions.Categories.Create, L("Permission:Create"));
            categoryPermission.AddChild(ApiPermissions.Categories.Edit, L("Permission:Edit"));
            categoryPermission.AddChild(ApiPermissions.Categories.Delete, L("Permission:Delete"));

            var proxyPermission = myGroup.AddPermission(ApiPermissions.Proxies.Default, L("Permission:Proxies"));
            proxyPermission.AddChild(ApiPermissions.Proxies.Create, L("Permission:Create"));
            proxyPermission.AddChild(ApiPermissions.Proxies.Edit, L("Permission:Edit"));
            proxyPermission.AddChild(ApiPermissions.Proxies.Delete, L("Permission:Delete"));

            var accountPermission = myGroup.AddPermission(ApiPermissions.Accounts.Default, L("Permission:Accounts"));
            accountPermission.AddChild(ApiPermissions.Accounts.Create, L("Permission:Create"));
            accountPermission.AddChild(ApiPermissions.Accounts.Edit, L("Permission:Edit"));
            accountPermission.AddChild(ApiPermissions.Accounts.Delete, L("Permission:Delete"));

            var accountProxyPermission = myGroup.AddPermission(ApiPermissions.AccountProxies.Default, L("Permission:AccountProxies"));
            accountProxyPermission.AddChild(ApiPermissions.AccountProxies.Create, L("Permission:Create"));
            accountProxyPermission.AddChild(ApiPermissions.AccountProxies.Edit, L("Permission:Edit"));
            accountProxyPermission.AddChild(ApiPermissions.AccountProxies.Delete, L("Permission:Delete"));
            
            myGroup.AddPermission(ApiPermissions.PayrollsConfiguration.Default, L("Permission:PayrollsConfiguration"));

            myGroup.AddPermission(ApiPermissions.Dev.Default, L("Permission:Dev"));
            
            //======================= 
            var postGroup = myGroup.AddPermission(ApiPermissions.PostGroup.Default, L("Permission:PostGroup"));
            
            var postPermission = postGroup.AddChild(ApiPermissions.Posts.Default, L("Permission:Posts"));
            postPermission.AddChild(ApiPermissions.Posts.Create, L("Permission:Create"));
            postPermission.AddChild(ApiPermissions.Posts.Edit, L("Permission:Edit"));
            postPermission.AddChild(ApiPermissions.Posts.EditNote, L("Permission:EditNote"));
            postPermission.AddChild(ApiPermissions.Posts.Delete, L("Permission:Delete"));

            var scheduledPostPermission = postGroup.AddChild(ApiPermissions.ScheduledPosts.Default, L("Permission:ScheduledPosts"));
            scheduledPostPermission.AddChild(ApiPermissions.ScheduledPosts.Create, L("Permission:Create"));
            scheduledPostPermission.AddChild(ApiPermissions.ScheduledPosts.Edit, L("Permission:Edit"));
            scheduledPostPermission.AddChild(ApiPermissions.ScheduledPosts.Delete, L("Permission:Delete"));

            
            var crawlPermission = myGroup.AddPermission(ApiPermissions.Crawl.Default, L("Permission:Crawl"));

            var userInfoPermission = myGroup.AddPermission(ApiPermissions.UserInfos.Default, L("Permission:UserInfos"));
            userInfoPermission.AddChild(ApiPermissions.UserInfos.Create, L("Permission:Create"));
            userInfoPermission.AddChild(ApiPermissions.UserInfos.Edit, L("Permission:Edit"));
            userInfoPermission.AddChild(ApiPermissions.UserInfos.Delete, L("Permission:Delete"));

            var payrollPermission = myGroup.AddPermission(ApiPermissions.Payrolls.Default, L("Permission:Payrolls"));
            payrollPermission.AddChild(ApiPermissions.Payrolls.Create, L("Permission:Create"));
            payrollPermission.AddChild(ApiPermissions.Payrolls.Edit, L("Permission:Edit"));
            payrollPermission.AddChild(ApiPermissions.Payrolls.Delete, L("Permission:Delete"));

            var userPayrollCommissionPermission = myGroup.AddPermission(ApiPermissions.UserPayrollCommissions.Default, L("Permission:UserPayrollCommissions"));
            userPayrollCommissionPermission.AddChild(ApiPermissions.UserPayrollCommissions.Create, L("Permission:Create"));
            userPayrollCommissionPermission.AddChild(ApiPermissions.UserPayrollCommissions.Edit, L("Permission:Edit"));
            userPayrollCommissionPermission.AddChild(ApiPermissions.UserPayrollCommissions.Delete, L("Permission:Delete"));

            var userPayrollPermission = myGroup.AddPermission(ApiPermissions.UserPayrolls.Default, L("Permission:UserPayrolls"));
            userPayrollPermission.AddChild(ApiPermissions.UserPayrolls.Create, L("Permission:Create"));
            userPayrollPermission.AddChild(ApiPermissions.UserPayrolls.ViewDetail, L("Permission:ViewDetail"));
            userPayrollPermission.AddChild(ApiPermissions.UserPayrolls.Edit, L("Permission:Edit"));
            userPayrollPermission.AddChild(ApiPermissions.UserPayrolls.Delete, L("Permission:Delete"));

            var userWavePermission = myGroup.AddPermission(ApiPermissions.UserWaves.Default, L("Permission:UserWaves"));
            userWavePermission.AddChild(ApiPermissions.UserWaves.Create, L("Permission:Create"));
            userWavePermission.AddChild(ApiPermissions.UserWaves.Edit, L("Permission:Edit"));
            userWavePermission.AddChild(ApiPermissions.UserWaves.Delete, L("Permission:Delete"));

            var userPayrollBonusPermission = myGroup.AddPermission(ApiPermissions.UserPayrollBonuses.Default, L("Permission:UserPayrollBonuses"));
            userPayrollBonusPermission.AddChild(ApiPermissions.UserPayrollBonuses.Create, L("Permission:Create"));
            userPayrollBonusPermission.AddChild(ApiPermissions.UserPayrollBonuses.Edit, L("Permission:Edit"));
            userPayrollBonusPermission.AddChild(ApiPermissions.UserPayrollBonuses.Delete, L("Permission:Delete"));

            var uncrawledPostPermission = myGroup.AddPermission(ApiPermissions.UncrawledPosts.Default, L("Permission:UncrawledPosts"));
            uncrawledPostPermission.AddChild(ApiPermissions.UncrawledPosts.Create, L("Permission:Create"));
            uncrawledPostPermission.AddChild(ApiPermissions.UncrawledPosts.Edit, L("Permission:Edit"));
            uncrawledPostPermission.AddChild(ApiPermissions.UncrawledPosts.Delete, L("Permission:Delete"));

            var apiConfigurationPermission = myGroup.AddPermission(ApiPermissions.ApiConfigurations.Default, L("Permission:ApiConfigurations"));
            apiConfigurationPermission.AddChild(ApiPermissions.ApiConfigurations.Create, L("Permission:Create"));
            apiConfigurationPermission.AddChild(ApiPermissions.ApiConfigurations.Edit, L("Permission:Edit"));
            apiConfigurationPermission.AddChild(ApiPermissions.ApiConfigurations.Delete, L("Permission:Delete"));

            var userAffiliatePermission = myGroup.AddPermission(ApiPermissions.UserAffiliates.Default, L("Permission:UserAffiliates"));
            userAffiliatePermission.AddChild(ApiPermissions.UserAffiliates.Create, L("Permission:Create"));
            userAffiliatePermission.AddChild(ApiPermissions.UserAffiliates.Edit, L("Permission:Edit"));
            userAffiliatePermission.AddChild(ApiPermissions.UserAffiliates.Delete, L("Permission:Delete"));

            myGroup.AddPermission(ApiPermissions.Bookings.Default, L("Permission:Bookings"));
            var partnerPermission = myGroup.AddPermission(ApiPermissions.Partners.Default, L("Permission:Partners"));
            partnerPermission.AddChild(ApiPermissions.Partners.Create, L("Permission:Create"));
            partnerPermission.AddChild(ApiPermissions.Partners.Edit, L("Permission:Edit"));
            partnerPermission.AddChild(ApiPermissions.Partners.Delete, L("Permission:Delete"));

            var campaignPermission = myGroup.AddPermission(ApiPermissions.Campaigns.Default, L("Permission:Campaigns"));
            campaignPermission.AddChild(ApiPermissions.Campaigns.Create, L("Permission:Create"));
            campaignPermission.AddChild(ApiPermissions.Campaigns.Edit, L("Permission:Edit"));
            campaignPermission.AddChild(ApiPermissions.Campaigns.Delete, L("Permission:Delete"));
            campaignPermission.AddChild(ApiPermissions.Campaigns.Export, L("Permission:Export"));

            var affiliateStatPermission = myGroup.AddPermission(ApiPermissions.AffiliateStats.Default, L("Permission:AffiliateStats"));
            affiliateStatPermission.AddChild(ApiPermissions.AffiliateStats.Create, L("Permission:Create"));
            affiliateStatPermission.AddChild(ApiPermissions.AffiliateStats.Edit, L("Permission:Edit"));
            affiliateStatPermission.AddChild(ApiPermissions.AffiliateStats.Delete, L("Permission:Delete"));
            

            var contractPermission = myGroup.AddPermission(ApiPermissions.Contracts.Default, L("Permission:Contracts"));
            contractPermission.AddChild(ApiPermissions.Contracts.Create, L("Permission:Create"));
            contractPermission.AddChild(ApiPermissions.Contracts.Edit, L("Permission:Edit"));
            contractPermission.AddChild(ApiPermissions.Contracts.Delete, L("Permission:Delete"));
            
            var transactionPermission = myGroup.AddPermission(ApiPermissions.ContractTransactions.Default, L("Permission:ContractTransactions"));
            transactionPermission.AddChild(ApiPermissions.ContractTransactions.Create, L("Permission:Create"));
            transactionPermission.AddChild(ApiPermissions.ContractTransactions.Edit, L("Permission:Edit"));
            transactionPermission.AddChild(ApiPermissions.ContractTransactions.Delete, L("Permission:Delete"));
            
            var tiktokPermission = myGroup.AddPermission(ApiPermissions.Tiktok.Default, L("Permission:Tiktok"));
            tiktokPermission.AddChild(ApiPermissions.Tiktok.Dashboard, L("Permission:Dashboard"));
            tiktokPermission.AddChild(ApiPermissions.Tiktok.MNC, L("Permission:MNC"));
            tiktokPermission.AddChild(ApiPermissions.Tiktok.Channels, L("Permission:Channels"));
            tiktokPermission.AddChild(ApiPermissions.Tiktok.Reports, L("Permission:Reports"));

            var teamMemberPermission = myGroup.AddPermission(ApiPermissions.TeamMembers.Default, L("Permission:TeamMembers"));
            teamMemberPermission.AddChild(ApiPermissions.TeamMembers.Create, L("Permission:Create"));
            teamMemberPermission.AddChild(ApiPermissions.TeamMembers.Edit, L("Permission:Edit"));
            teamMemberPermission.AddChild(ApiPermissions.TeamMembers.Delete, L("Permission:Delete"));
            
            var contentPermission = myGroup.AddPermission(ApiPermissions.Content.Default, L("Permission:Contents"));
            contentPermission.AddChild(ApiPermissions.Content.Create, L("Permission:Create"));
            contentPermission.AddChild(ApiPermissions.Content.Edit, L("Permission:Edit"));
            contentPermission.AddChild(ApiPermissions.Content.Delete, L("Permission:Delete"));
            
            var staffEvaluationsPermission = myGroup.AddPermission(ApiPermissions.StaffEvaluations.Default, L("Permission:StaffEvaluations"));
            staffEvaluationsPermission.AddChild(ApiPermissions.StaffEvaluations.Create, L("Permission:Create"));
            staffEvaluationsPermission.AddChild(ApiPermissions.StaffEvaluations.Edit, L("Permission:Edit"));
            staffEvaluationsPermission.AddChild(ApiPermissions.StaffEvaluations.Delete, L("Permission:Delete"));
            
            var userEvaluationConfigPermission = myGroup.AddPermission(ApiPermissions.UserEvaluationConfigurations.Default, L("Permission:UserEvaluationsConfigs"));
            userEvaluationConfigPermission.AddChild(ApiPermissions.UserEvaluationConfigurations.ConfigSale, L("Permission:UserEvaluationsConfigs.Sale"));
            userEvaluationConfigPermission.AddChild(ApiPermissions.UserEvaluationConfigurations.ConfigTiktok, L("Permission:UserEvaluationsConfigs.Tiktok"));
            userEvaluationConfigPermission.AddChild(ApiPermissions.UserEvaluationConfigurations.ConfigContent, L("Permission:UserEvaluationsConfigs.Content"));
            userEvaluationConfigPermission.AddChild(ApiPermissions.UserEvaluationConfigurations.ConfigCommunity, L("Permission:UserEvaluationsConfigs.Community"));

            var managerPermission = myGroup.AddPermission(ApiPermissions.BoD.Default, L("Permission:BoD"));
            managerPermission.AddChild(ApiPermissions.BoD.Director, L("Permission:BoD.Director"));
            managerPermission.AddChild(ApiPermissions.BoD.Manager, L("Permission:BoD.Manager"));
            managerPermission.AddChild(ApiPermissions.BoD.Leader, L("Permission:BoD.Leader"));
            
            var userCompensationPermission = myGroup.AddPermission(ApiPermissions.UserCompensations.Default, L("Permission:UserCompensations"));
            userCompensationPermission.AddChild(ApiPermissions.UserCompensations.Create, L("Permission:Create"));
            userCompensationPermission.AddChild(ApiPermissions.UserCompensations.Edit, L("Permission:Edit"));
            userCompensationPermission.AddChild(ApiPermissions.UserCompensations.Delete, L("Permission:Delete"));
            
            var userBonusConfigPermission = myGroup.AddPermission(ApiPermissions.UserBonusConfigs.Default, L("Permission:UserBonusConfigs"));
            userBonusConfigPermission.AddChild(ApiPermissions.UserBonusConfigs.Create, L("Permission:Create"));
            userBonusConfigPermission.AddChild(ApiPermissions.UserBonusConfigs.Edit, L("Permission:Edit"));
            userBonusConfigPermission.AddChild(ApiPermissions.UserBonusConfigs.Delete, L("Permission:Delete"));
            
            var userSalaryPermission = myGroup.AddPermission(ApiPermissions.UserSalaries.Default, L("Permission:UserSalaries"));
            userSalaryPermission.AddChild(ApiPermissions.UserSalaries.Create, L("Permission:Create"));
            userSalaryPermission.AddChild(ApiPermissions.UserSalaries.Edit, L("Permission:Edit"));
            userSalaryPermission.AddChild(ApiPermissions.UserSalaries.Delete, L("Permission:Delete"));
            
            var userSalaryConfigPermission = myGroup.AddPermission(ApiPermissions.UserSalaryConfiguration.Default, L("Permission:UserSalaryConfigurations"));
            userSalaryConfigPermission.AddChild(ApiPermissions.UserSalaryConfiguration.Create, L("Permission:Create"));
            userSalaryConfigPermission.AddChild(ApiPermissions.UserSalaryConfiguration.Edit, L("Permission:Edit"));
            userSalaryConfigPermission.AddChild(ApiPermissions.UserSalaryConfiguration.Delete, L("Permission:Delete"));
            
            var partnerModulePermission = myGroup.AddPermission(ApiPermissions.PartnerModule.Default, L("Permission:PartnerModule"));
            partnerModulePermission.AddChild(ApiPermissions.PartnerModule.PartnerDashboard, L("Permission:PartnerModule.PartnerDashboard"));
            partnerModulePermission.AddChild(ApiPermissions.PartnerModule.PartnerCampaigns, L("Permission:PartnerModule.PartnerCampaigns"));
            partnerModulePermission.AddChild(ApiPermissions.PartnerModule.PartnerContracts, L("Permission:PartnerModule.PartnerContracts"));
            partnerModulePermission.AddChild(ApiPermissions.PartnerModule.PartnerCommunities, L("Permission:PartnerModule.PartnerCommunities"));
            partnerModulePermission.AddChild(ApiPermissions.PartnerModule.PartnerPosts, L("Permission:PartnerModule.PartnerPosts"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<ApiResource>(name);
        }
    }
}