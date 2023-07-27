using System;
using System.Threading.Tasks;
using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using FacebookCommunityAnalytics.Api.Localization;
using FacebookCommunityAnalytics.Api.Permissions;
using Volo.Abp.Account.Localization;
using Volo.Abp.AuditLogging.Web.Navigation;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.IdentityServer.Web.Navigation;
using Volo.Abp.LanguageManagement.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TextTemplateManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Users;
using Volo.Saas.Host.Navigation;

namespace FacebookCommunityAnalytics.Api.Web.Menus
{
    public class ApiMenuContributor : IMenuContributor
    {
        private readonly IConfiguration _configuration;

        public ApiMenuContributor(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            switch (context.Menu.Name)
            {
                case StandardMenus.Main:
                    await ConfigureMainMenuAsync(context);
                    await ConfigureUserProfileMenuAsync(context);
                    //await ConfigTeamMenuAsync(context);
                    //await ConfigHrMenuAsync(context);
                    //await ConfigContentMenuAsync(context);
                    //await ConfigBookingMenuAsync(context);
                    await ConfigPartnerModuleMenu(context);
                    //await ConfigAffiliatesMenuAsync(context);
                    //await ConfigTiktokGroupMenuAsync(context);
                    await ConfigFacebookGroupMenuAsync(context);
                    //await ConfigSystemGroupMenuAsync(context);
                    break;
                case StandardMenus.User:
                    await ConfigureUserMenuAsync(context);
                    break;
            }
        }

        private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<ApiResource>();
            //Home
            context.Menu.AddItem(
                new ApplicationMenuItem(
                    ApiMenus.Home,
                    l["Menu:Home"],
                    "~/",
                    icon: "fa fa-home",
                    order: 1
                )
            );
            //
            // //Host Dashboard
            // context.Menu.AddItem(
            //     new ApplicationMenuItem(
            //         ApiMenus.HostDashboard,
            //         l["Menu:Dashboard"],
            //         "~/HostDashboard",
            //         icon: "fa fa-line-chart",
            //         order: 2,
            //         requiredPermissionName: ApiPermissions.Dashboard.Host
            //     )
            // );
            //
            // //Tenant Dashboard
            // context.Menu.AddItem(
            //     new ApplicationMenuItem(
            //         ApiMenus.TenantDashboard,
            //         l["Menu:Dashboard"],
            //         "~/Dashboard",
            //         icon: "fa fa-line-chart",
            //         order: 2,
            //         requiredPermissionName: ApiPermissions.Dashboard.Tenant
            //     )
            // );

            //Saas
            //context.Menu.SetSubItemOrder(SaasHostMenuNames.GroupName, 3);


            //Administration
            var administration = context.Menu.GetAdministration();
            administration.Order = 5;

            //Administration->Identity
            administration.SetSubItemOrder(IdentityMenuNames.GroupName, 1);

            //Administration->Identity Server
            administration.SetSubItemOrder(AbpIdentityServerMenuNames.GroupName, 2);

            //Administration->Language Management
            administration.SetSubItemOrder(LanguageManagementMenuNames.GroupName, 3);

            //Administration->Text Template Management
            administration.SetSubItemOrder(TextTemplateManagementMainMenuNames.GroupName, 4);

            //Administration->Audit Logs
            administration.SetSubItemOrder(AbpAuditLoggingMainMenuNames.GroupName, 5);

            //Administration->Settings
            administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 6);

            return Task.CompletedTask;
        }

        // private Task ConfigureUserMenuAsync(MenuConfigurationContext context)
        // {
        //     var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();
        //
        //     if (currentUser.IsAuthenticated)
        //     {
        //         var identityServerUrl = _configuration["AuthServer:Authority"] ?? "~";
        //         var uiResource = context.GetLocalizer<AbpUiResource>();
        //         var accountResource = context.GetLocalizer<AccountResource>();
        //         context.Menu.AddItem(new ApplicationMenuItem("Account.Manage", accountResource["ManageYourProfile"], $"{identityServerUrl.EnsureEndsWith('/')}Account/Manage", icon: "fa fa-cog", order: 1000, null, "_blank"));
        //         context.Menu.AddItem(new ApplicationMenuItem("Account.SecurityLogs", accountResource["MySecurityLogs"], $"{identityServerUrl.EnsureEndsWith('/')}Account/SecurityLogs", target: "_blank"));
        //         context.Menu.AddItem(new ApplicationMenuItem("Account.Logout", uiResource["Logout"], url: "~/Account/Logout", icon: "fa fa-power-off", order: int.MaxValue - 1000));
        //     }
        //
        //     return Task.CompletedTask;
        // }
        //

        private Task ConfigTeamMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<ApiResource>();

            var teamMenu = new ApplicationMenuItem
            (
                ApiMenus.UserInfos,
                l["Menu:Teams"],
                url: "#",
                icon: "fas fa-user-tag",
                requiredPermissionName: ApiPermissions.UserInfos.Default
            );

            teamMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.UserInfos,
                    l["Menu:TeamUserInfos"],
                    url: "/user-infos",
                    icon: "fas fa-user-tag",
                    requiredPermissionName: ApiPermissions.UserInfos.Default
                )
            );

            teamMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.TeamMembers,
                    l["Menu:TeamMembers"],
                    url: "/teams",
                    icon: "fas fa-user-tag",
                    requiredPermissionName: ApiPermissions.TeamMembers.Default
                )
            );

            context.Menu.AddItem(teamMenu);
            return Task.CompletedTask;
        }

        private Task ConfigHrMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<ApiResource>();

            var hrMenu = new ApplicationMenuItem
            (
                ApiMenus.UserInfos,
                l["Menu:HR"],
                url: "#",
                icon: "fas fa-user-tag",
                requiredPermissionName: ApiPermissions.BoD.Default
            );

            // hrMenu.Items.Add
            // (
            //     new ApplicationMenuItem
            //     (
            //         ApiMenus.UserInfos,
            //         l["Menu:HRUserInfos"],
            //         url: "/user-infos",
            //         icon: "fas fa-user-tag",
            //         requiredPermissionName: ApiPermissions.UserInfos.Default
            //     )
            // );
            //
            // hrMenu.Items.Add
            // (
            //     new ApplicationMenuItem
            //     (
            //         ApiMenus.TeamMembers,
            //         l["Menu:HRTeamMembers"],
            //         url: "/teams",
            //         icon: "fas fa-user-tag",
            //         requiredPermissionName: ApiPermissions.TeamMembers.Default
            //     )
            // );

            hrMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.StaffEvaluations,
                    l["Menu:StaffEvaluations"],
                    url: "/staff-evaluations",
                    icon: "fas fa-user-tag",
                    requiredPermissionName: ApiPermissions.StaffEvaluations.Default
                )
            );

            hrMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.UserEvaluationConfigurations,
                    l["Menu:EvaluationConfigurations"],
                    url: "/evaluation-configs",
                    icon: "fas fa-user-tag",
                    requiredPermissionName: ApiPermissions.UserEvaluationConfigurations.Default
                )
            );

            hrMenu.AddItem
            (
                new ApplicationMenuItem
                (
                    ApiMenus.UserCompensations,
                    l["Menu:UserCompensations"],
                    url: "/compensations",
                    icon: string.Empty,
                    requiredPermissionName: ApiPermissions.UserCompensations.Default
                )
            );

            hrMenu.AddItem
            (
                new ApplicationMenuItem
                (
                    ApiMenus.UserCompensations,
                    l["Menu:UserBonusConfigs"],
                    url: "/user-bonus-configs",
                    icon: string.Empty,
                    requiredPermissionName: ApiPermissions.UserBonusConfigs.Default
                )
            );


            context.Menu.AddItem(hrMenu);

            return Task.CompletedTask;
        }

        private Task ConfigContentMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<ApiResource>();

            var contentMenu = new ApplicationMenuItem
            (
                ApiMenus.Contents,
                l["Menu:Content"],
                url: "#",
                icon: "fas fa-file-image",
                requiredPermissionName: ApiPermissions.Content.Default
            );

            contentMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Contents,
                    l["Menu:ContentMedia"],
                    url: "/contents",
                    icon: "fas fa-file-image",
                    requiredPermissionName: ApiPermissions.Content.Default
                )
            );

            context.Menu.AddItem
            (
                contentMenu
            );
            return Task.CompletedTask;
        }

        private Task ConfigBookingMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<ApiResource>();

            var bookingMenu = new ApplicationMenuItem
            (
                ApiMenus.Partners,
                l["Menu:Booking"],
                url: "#",
                icon: "fa fa-handshake",
                requiredPermissionName: ApiPermissions.Bookings.Default
            );
            bookingMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Partners,
                    l["Menu:Partners"],
                    url: "/partners",
                    icon: "fa fa-handshake",
                    requiredPermissionName: ApiPermissions.Partners.Default
                )
            );

            bookingMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Campaigns,
                    l["Menu:Campaigns"],
                    url: "/campaigns",
                    icon: "fa fa-bullhorn",
                    requiredPermissionName: ApiPermissions.Campaigns.Default
                )
            );

            bookingMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Contracts,
                    l["Menu:Contracts"],
                    url: "/contracts",
                    icon: "fas fa-file-contract",
                    requiredPermissionName: ApiPermissions.Contracts.Default
                )
            );


            context.Menu.AddItem
            (
                bookingMenu
            );
            return Task.CompletedTask;
        }

        private Task ConfigPartnerModuleMenu(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<ApiResource>();

            context.Menu.AddItem
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Partners,
                    l["Menu:Partner.List"],
                    url: "/partners",
                    icon: "fa fa-handshake",
                    requiredPermissionName: ApiPermissions.PartnerModule.Default
                )
            );

            context.Menu.AddItem
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Campaigns,
                    l["Menu:Partner.Campaigns"],
                    url: "/campaigns",
                    icon: "fa fa-bullhorn",
                    requiredPermissionName: ApiPermissions.PartnerModule.PartnerCampaigns
                )
            );

            context.Menu.AddItem
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Contracts,
                    l["Menu:Partner.Contracts"],
                    url: "/partnercontracts",
                    icon: "fas fa-file-contract",
                    requiredPermissionName: ApiPermissions.PartnerModule.PartnerContracts
                )
            );
            context.Menu.AddItem(
                new ApplicationMenuItem
                (
                    ApiMenus.Posts,
                    l["Menu:Posts"],
                    url: "/partner-posts",
                    icon: "fab fa-facebook",
                    requiredPermissionName: ApiPermissions.PartnerModule.PartnerPosts
                )
            );
            // var facebookMenu = new ApplicationMenuItem
            // (
            //     ApiMenus.Posts,
            //     l["Menu:Facebook"],
            //     url: "#",
            //     icon: "fab fa-facebook",
            //     requiredPermissionName: ApiPermissions.PartnerModule.PartnerPosts
            // );
            //
            // facebookMenu.Items.Add
            // (
            //     new ApplicationMenuItem
            //     (
            //         ApiMenus.Posts,
            //         l["Menu:Posts"],
            //         url: "/partner-posts",
            //         icon: "fab fa-facebook",
            //         requiredPermissionName: ApiPermissions.PartnerModule.PartnerPosts
            //     )
            // );

            //context.Menu.AddItem(facebookMenu);


            return Task.CompletedTask;
        }

        private Task ConfigAffiliatesMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<ApiResource>();

            var userAffiliateMenu = new ApplicationMenuItem
            (
                ApiMenus.PayrollsConfiguration,
                l["Menu:UserAffiliates"],
                url: "#",
                icon: "fas fa-chart-line",
                requiredPermissionName: ApiPermissions.UserAffiliates.Default
            );

            userAffiliateMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.PayrollsConfiguration,
                    l["Menu:AffSummary"],
                    url: "/affsummary",
                    icon: "fas fa-chart-line",
                    requiredPermissionName: ApiPermissions.AffiliateStats.Default
                )
            );

            userAffiliateMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.PayrollsConfiguration,
                    l["Menu:AffReports"],
                    url: "/affreport",
                    icon: "fas fa-chart-line",
                    requiredPermissionName: ApiPermissions.UserAffiliates.Default
                )
            );

            userAffiliateMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.PayrollsConfiguration,
                    l["Menu:AffLinks"],
                    url: "/afflink",
                    icon: "fas fa-chart-line",
                    requiredPermissionName: ApiPermissions.UserAffiliates.Default
                )
            );

            userAffiliateMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.PayrollsConfiguration,
                    l["Menu:AffLinksTiki"],
                    url: "/afflink/tiki",
                    icon: "fas fa-chart-line",
                    requiredPermissionName: ApiPermissions.UserAffiliates.Default
                )
            );

            userAffiliateMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.PayrollsConfiguration,
                    l["Menu:AffLinkDetails"],
                    url: "/linkdetails",
                    icon: "fas fa-newspaper",
                    requiredPermissionName: ApiPermissions.UserAffiliates.Default
                )
            );

            context.Menu.AddItem
            (
                userAffiliateMenu
            );

            return Task.CompletedTask;
        }

        private Task ConfigTiktokGroupMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<ApiResource>();

            var tiktokMenu = new ApplicationMenuItem
            (
                ApiMenus.Posts,
                l["Menu:Tiktok"],
                url: "#",
                icon: "fab fa-tiktok",
                requiredPermissionName: ApiPermissions.Tiktok.Default
            );

            tiktokMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Tiktok,
                    l["TiktokChannels"],
                    url: "/tiktok-channels",
                    icon: "fab fa-tiktok",
                    requiredPermissionName: ApiPermissions.Tiktok.Default
                )
            );

            tiktokMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Tiktok,
                    l["Menu:TiktokDailyReports"],
                    url: "/tiktoks",
                    icon: "fab fa-tiktok",
                    requiredPermissionName: ApiPermissions.Tiktok.Default
                )
            );

            tiktokMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Tiktok,
                    l["Menu:TiktokWeeklyReports"],
                    url: "/tiktok-weekly-reports",
                    icon: "fab fa-tiktok",
                    requiredPermissionName: ApiPermissions.Tiktok.Default
                )
            );

            tiktokMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Tiktok,
                    l["Menu:TiktokMonthlyReports"],
                    url: "/tiktok-monthly-reports",
                    icon: "fab fa-tiktok",
                    requiredPermissionName: ApiPermissions.Tiktok.Default
                )
            );

            context.Menu.AddItem
            (
                tiktokMenu
            );

            return Task.CompletedTask;
        }

        private Task ConfigFacebookGroupMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<ApiResource>();

            var facebookMenu = new ApplicationMenuItem
            (
                ApiMenus.Posts,
                l["Menu:Facebook"],
                url: "#",
                icon: "fab fa-facebook",
                requiredPermissionName: ApiPermissions.Posts.Default
            );

            facebookMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Posts,
                    l["Menu:Posts"],
                    url: "/posts",
                    icon: "fab fa-facebook",
                    requiredPermissionName: ApiPermissions.Posts.Default
                )
            );

            facebookMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Posts,
                    l["Menu:ScheduledPosts"],
                    url: "/scheduled-posts",
                    icon: "fas fa-stopwatch",
                    requiredPermissionName: ApiPermissions.ScheduledPosts.Default
                )
            );

            context.Menu.AddItem
            (
                facebookMenu
            );

            return Task.CompletedTask;
        }

        private Task ConfigSystemGroupMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<ApiResource>();

            var systemMenu = new ApplicationMenuItem
            (
                ApiMenus.Dev,
                l["Menu:SystemMenu"],
                url: "#",
                icon: "fas fa-user-cog",
                requiredPermissionName: ApiPermissions.Systems.Default
            );

            var taxonomyMenu = new ApplicationMenuItem
            (
                ApiMenus.Groups,
                l["Menu:Taxonomy"],
                url: "#",
                icon: "fas fa-file-alt",
                requiredPermissionName: ApiPermissions.Groups.Default
            );

            taxonomyMenu.AddItem
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Groups,
                    l["Menu:Groups"],
                    url: "/communities",
                    icon: "fas fa-users",
                    requiredPermissionName: ApiPermissions.Groups.Default
                )
            );

            taxonomyMenu.AddItem
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Categories,
                    l["Menu:Categories"],
                    url: "/categories",
                    icon: "fas fa-list",
                    requiredPermissionName: ApiPermissions.Categories.Default
                )
            );

            systemMenu.AddItem(taxonomyMenu);

            // var taxonomyPartnerMenu = new ApplicationMenuItem
            // (
            //     ApiMenus.AccountProxies,
            //     l["Menu:Taxonomy"],
            //     url: "#",
            //     icon: "fas fa-file-alt",
            //     requiredPermissionName: ApiPermissions.PartnerModule.Default
            // );
            //
            // taxonomyPartnerMenu.AddItem
            // (
            //     new ApplicationMenuItem
            //     (
            //         ApiMenus.Groups,
            //         l["Menu:Partner.Communities"],
            //         url: "/partner-communities",
            //         icon: "fas fa-users",
            //         requiredPermissionName: ApiPermissions.PartnerModule.PartnerCommunities
            //     )
            // );
            //
            // systemMenu.AddItem(taxonomyPartnerMenu);

            var crawlMenu = new ApplicationMenuItem
            (
                ApiMenus.AccountProxies,
                l["Menu:Crawl"],
                url: "#",
                icon: "fas fa-sliders-h",
                requiredPermissionName: ApiPermissions.AccountProxies.Default
            );

            crawlMenu.AddItem
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Accounts,
                    l["Menu:Accounts"],
                    url: "/accounts",
                    icon: "fas fa-user-circle",
                    requiredPermissionName: ApiPermissions.Accounts.Default
                )
            );

            crawlMenu.AddItem
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Proxies,
                    l["Menu:Proxies"],
                    url: "/proxies",
                    icon: "fa fa-file-alt",
                    requiredPermissionName: ApiPermissions.Proxies.Default
                )
            );

            crawlMenu.AddItem
            (
                new ApplicationMenuItem
                (
                    ApiMenus.AccountProxies,
                    l["Menu:AccountProxies"],
                    url: "/account-proxies",
                    icon: "fa fa-file-alt",
                    requiredPermissionName: ApiPermissions.AccountProxies.Default
                )
            );

            systemMenu.AddItem
            (
                crawlMenu
            );

            systemMenu.AddItem
            (
                new ApplicationMenuItem
                (
                    ApiMenus.PayrollsConfiguration,
                    l["Menu:PayrollsConfiguration"],
                    url: "/payrolls-configuration",
                    icon: "fas fa-cogs",
                    requiredPermissionName: ApiPermissions.PayrollsConfiguration.Default
                )
            );

            systemMenu.AddItem
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Dev,
                    l["Menu:Dev"],
                    url: "/dev",
                    icon: "fas fa-user-cog",
                    requiredPermissionName: ApiPermissions.Dev.Default
                )
            );
            context.Menu.AddItem(systemMenu);
            return Task.CompletedTask;
        }

        private async Task ConfigureUserProfileMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<ApiResource>();
            context.Menu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.UserInfos,
                    l["Menu:UserProfile"],
                    url: "/user-profile",
                    icon: "fas fa-user-tag",
                    requiredPermissionName: ApiPermissions.UserInfos.Default
                )
            );
        }

        private Task ConfigureUserMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<ApiResource>();

            var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

            if (currentUser.IsAuthenticated)
            {
                var identityServerUrl = _configuration["AuthServer:Authority"] ?? "~";
                var uiResource = context.GetLocalizer<AbpUiResource>();
                var accountResource = context.GetLocalizer<AccountResource>();
                context.Menu.AddItem
                (
                    new ApplicationMenuItem
                    (
                        "Account.Manage",
                        accountResource["ManageYourProfile"],
                        $"{identityServerUrl.EnsureEndsWith('/')}Account/Manage",
                        icon: "fa fa-cog",
                        order: 1000,
                        null,
                        "_blank"
                    )
                );
                context.Menu.AddItem
                    (new ApplicationMenuItem("Account.SecurityLogs", accountResource["MySecurityLogs"], $"{identityServerUrl.EnsureEndsWith('/')}Account/SecurityLogs", target: "_blank"));
                context.Menu.AddItem
                (
                    new ApplicationMenuItem
                    (
                        "Account.Logout",
                        uiResource["Logout"],
                        url: "~/Account/Logout",
                        icon: "fa fa-power-off",
                        order: int.MaxValue - 1000
                    )
                );
            }

            return Task.CompletedTask;
        }
    }
}