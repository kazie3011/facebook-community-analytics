using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using FacebookCommunityAnalytics.Api.Localization;
using FacebookCommunityAnalytics.Api.Permissions;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Configs;
using Flurl;
using Volo.Abp.Account.Localization;
using Volo.Abp.AuditLogging.Blazor.Menus;
using Volo.Abp.Identity.Pro.Blazor.Navigation;
using Volo.Abp.IdentityServer.Blazor.Navigation;
using Volo.Abp.LanguageManagement.Blazor.Menus;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TextTemplateManagement.Blazor.Menus;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Users;
using Volo.CmsKit.Permissions;
using Volo.CmsKit.Pro.Admin.Web.Menus;
using Volo.Saas.Host.Blazor.Navigation;

namespace FacebookCommunityAnalytics.Api.Blazor.Menus
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
            // global config
            var globalConfigurationSection = _configuration.GetSection(nameof(Configs.GlobalConfiguration));
            var globalConfiguration = globalConfigurationSection.Get<Configs.GlobalConfiguration>();
            
            switch (context.Menu.Name)
            {
                case StandardMenus.Main:
                    
                    await ConfigureMainMenuAsync(context);
                    
                    if (globalConfiguration.PartnerConfiguration.IsPartnerTool)
                    {
                        await ConfigPartnerModuleMenu(context);
                        await ConfigureUserProfileMenuAsync(context);
                        await ConfigTeamMenuAsync(context);
                        await ConfigHrMenuAsync(context);
                        //await ConfigContentMenuAsync(context);
                        await ConfigBookingMenuAsync(context);
                        await ConfigAffiliatesMenuAsync(context);
                        await ConfigTiktokGroupMenuAsync(context);
                        await ConfigFacebookGroupMenuAsync(context);
                        await ConfigPartnerSystemGroupMenuAsync(context);
                    }
                    else
                    {
                        await ConfigureUserProfileMenuAsync(context);
                        await ConfigTeamMenuAsync(context);
                        await ConfigHrMenuAsync(context);
                        //await ConfigContentMenuAsync(context);
                        await ConfigBookingMenuAsync(context);
                        await ConfigAffiliatesMenuAsync(context);
                        await ConfigTiktokGroupMenuAsync(context);
                        await ConfigFacebookGroupMenuAsync(context);
                        await ConfigSystemGroupMenuAsync(context);
                    }
                    break;
                case StandardMenus.User: await ConfigureUserMenuAsync(context,globalConfiguration.PartnerConfiguration.IsPartnerTool);
                    break;
            }
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
        private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<ApiResource>();

            context.Menu.Items.Insert
            (
                0,
                new ApplicationMenuItem
                (
                    ApiMenus.Home,
                    l["Menu:Home"],
                    "/",
                    icon: "fas fa-home",
                    order: 1,
                    requiredPermissionName: ApiPermissions.Index.Default
                )
            );

            /* Example nested menu definition:

            context.Menu.AddItem(
                new ApplicationMenuItem("Menu0", "Menu Level 0")
                .AddItem(new ApplicationMenuItem("Menu0.1", "Menu Level 0.1", url: "/test01"))
                .AddItem(
                    new ApplicationMenuItem("Menu0.2", "Menu Level 0.2")
                        .AddItem(new ApplicationMenuItem("Menu0.2.1", "Menu Level 0.2.1", url: "/test021"))
                        .AddItem(new ApplicationMenuItem("Menu0.2.2", "Menu Level 0.2.2")
                            .AddItem(new ApplicationMenuItem("Menu0.2.2.1", "Menu Level 0.2.2.1", "/test0221"))
                            .AddItem(new ApplicationMenuItem("Menu0.2.2.2", "Menu Level 0.2.2.2", "/test0222"))
                        )
                        .AddItem(new ApplicationMenuItem("Menu0.2.3", "Menu Level 0.2.3", url: "/test023"))
                        .AddItem(new ApplicationMenuItem("Menu0.2.4", "Menu Level 0.2.4", url: "/test024")
                            .AddItem(new ApplicationMenuItem("Menu0.2.4.1", "Menu Level 0.2.4.1", "/test0241"))
                    )
                    .AddItem(new ApplicationMenuItem("Menu0.2.5", "Menu Level 0.2.5", url: "/test025"))
                )
                .AddItem(new ApplicationMenuItem("Menu0.2", "Menu Level 0.2", url: "/test02"))
            );
            */

            //Administration
            var administration = context.Menu.GetAdministration();
            //var abpIdentity = administration.GetMenuItem(IdentityProMenus.GroupName);
            administration.Order = 4;

            //Administration->Identity
            administration.SetSubItemOrder(IdentityProMenus.GroupName, 1);

            //Administration->Identity Server
            administration.SetSubItemOrder(AbpIdentityServerMenuNames.GroupName, 2);

            //Administration->Language Management
            administration.SetSubItemOrder(LanguageManagementMenus.GroupName, 3);

            //Administration->Text Template Management
            administration.SetSubItemOrder(TextTemplateManagementMenus.GroupName, 4);

            //Administration->Audit Logs
            administration.SetSubItemOrder(AbpAuditLoggingMenus.GroupName, 5);

            //Administration->Settings
            administration.SetSubItemOrder(SettingManagementMenus.GroupName, 6);

            return Task.CompletedTask;
        }

        private Task ConfigTeamMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<ApiResource>();
            
            var teamMenu = new ApplicationMenuItem
            (
                ApiMenus.UserInfos,
                l["Menu:Teams"],
                url: "#",
                icon: "fas fa-users",
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
                requiredPermissionName: ApiPermissions.StaffEvaluations.Default
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
            //         icon: "fas fa-users",
            //         requiredPermissionName: ApiPermissions.TeamMembers.Default
            //     )
            // );

            hrMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.StaffEvaluations,
                    l["Menu:StaffEvaluation"],
                    url: "/staffevaluation",
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
            
            hrMenu.AddItem
            (
                new ApplicationMenuItem
                (
                    ApiMenus.UserSalaryConfigurations,
                    l["Menu:UserSalaryConfigs"],
                    url: "/salary-configs",
                    icon: string.Empty,
                    requiredPermissionName: ApiPermissions.UserSalaryConfiguration.Default
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
                    url: "/partnerlist",
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
                    url: "/partnercampaigns",
                    icon: "fa fa-bullhorn",
                    requiredPermissionName: ApiPermissions.PartnerModule.PartnerCampaigns
                )
            );
            

            // context.Menu.AddItem
            // (
            //     new ApplicationMenuItem
            //     (
            //         ApiMenus.Contracts,
            //         l["Menu:Partner.Contracts"],
            //         url: "/partnercontracts",
            //         icon: "fas fa-file-contract",
            //         requiredPermissionName: ApiPermissions.PartnerModule.PartnerContracts
            //     )
            // );
            //
            // var facebookMenu = new ApplicationMenuItem
            // (
            //     ApiMenus.Posts,
            //     l["Menu:Facebook"],
            //     url: "#",
            //     icon: "fab fa-facebook",
            //     requiredPermissionName: ApiPermissions.PartnerModule.PartnerPosts
            // );

            // facebookMenu.Items.Add
            // (
            //     new ApplicationMenuItem
            //     (
            //         ApiMenus.Posts,
            //         l["Menu:Posts"],
            //         url: "/partnerposts",
            //         icon: "fab fa-facebook",
            //         requiredPermissionName: ApiPermissions.PartnerModule.PartnerPosts
            //     )
            // );

            // context.Menu.AddItem(facebookMenu);


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

            // userAffiliateMenu.Items.Add
            // (
            //     new ApplicationMenuItem
            //     (
            //         ApiMenus.PayrollsConfiguration,
            //         l["Menu:AffLinksTiki"],
            //         url: "/afflink/tiki",
            //         icon: "fas fa-chart-line",
            //         requiredPermissionName: ApiPermissions.UserAffiliates.Default
            //     )
            // );

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
                    l["Menu:TiktokDashboard"],
                    url: "/tiktok-dashboard",
                    icon: "fab fa-tiktok",
                    requiredPermissionName: ApiPermissions.Tiktok.Dashboard
                )
            );
            
            tiktokMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Tiktok,
                    l["Menu:TiktokMCN"],
                    url: "/tiktok-mcns",
                    icon: "fab fa-tiktok",
                    requiredPermissionName: ApiPermissions.Tiktok.MNC
                )
            );
            
            tiktokMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Tiktok,
                    l["TiktokChannels"],
                    url: "/tiktok-channels",
                    icon: "fab fa-tiktok",
                    requiredPermissionName: ApiPermissions.Tiktok.Channels
                )
            );

            tiktokMenu.Items.Add
            (
                new ApplicationMenuItem
                (
                    ApiMenus.Tiktok,
                    l["Menu:TiktokReports"],
                    url: "/tiktok-reports",
                    icon: "fab fa-tiktok",
                    requiredPermissionName: ApiPermissions.Tiktok.Reports
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

            // facebookMenu.Items.Add
            // (
            //     new ApplicationMenuItem
            //     (
            //         ApiMenus.Posts,
            //         l["Menu:ScheduledPosts"],
            //         url: "/scheduled-posts",
            //         icon: "fas fa-stopwatch",
            //         requiredPermissionName: ApiPermissions.ScheduledPosts.Default
            //     )
            // );

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
                    ApiMenus.UncrawledPosts,
                    l["Menu:UncrawledPosts"],
                    url: "/uncrawled-posts",
                    icon: "fas fa-user-circle",
                    requiredPermissionName: ApiPermissions.UncrawledPosts.Default
                )
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
        
        private Task ConfigPartnerSystemGroupMenuAsync(MenuConfigurationContext context)
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
                    l["Menu:Partner.Communities"],
                    url: "/partner-communities",
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
                    ApiMenus.UncrawledPosts,
                    l["Menu:UncrawledPosts"],
                    url: "/uncrawled-posts",
                    icon: "fas fa-user-circle",
                    requiredPermissionName: ApiPermissions.UncrawledPosts.Default
                )
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
        
        
        private Task ConfigureUserMenuAsync(MenuConfigurationContext context, bool isPartnerTool)
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

                if (isPartnerTool)
                {
                    context.Menu.AddItem
                    (
                        new ApplicationMenuItem
                        (
                            ApiMenus.Campaigns,
                            l["Menu:Partner.Users"],
                            url: "/partner-users",
                            icon: "fa fa-user",
                            requiredPermissionName: ApiPermissions.PartnerModule.Default,
                            order: 1001
                        )
                    );
                }

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