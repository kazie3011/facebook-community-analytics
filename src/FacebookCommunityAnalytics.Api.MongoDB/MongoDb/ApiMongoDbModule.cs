using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.AffiliateStats;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.UncrawledPosts;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.UserPayrollBonuses;
using FacebookCommunityAnalytics.Api.UserWaves;
using FacebookCommunityAnalytics.Api.UserPayrollCommissions;
using FacebookCommunityAnalytics.Api.UserPayrolls;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Proxies;
using FacebookCommunityAnalytics.Api.AccountProxies;
using FacebookCommunityAnalytics.Api.Categories;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.TikTokMCNs;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserSalaryConfigurations;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AuditLogging.MongoDB;
using Volo.Abp.BackgroundJobs.MongoDB;
using Volo.Abp.FeatureManagement.MongoDB;
using Volo.Abp.Identity.MongoDB;
using Volo.Abp.IdentityServer.MongoDB;
using Volo.Abp.LanguageManagement.MongoDB;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.MongoDB;
using Volo.Abp.SettingManagement.MongoDB;
using Volo.Abp.TextTemplateManagement.MongoDB;
using Volo.Saas.MongoDB;
using Volo.Abp.BlobStoring.Database.MongoDB;
using Volo.Abp.Uow;
using Volo.CmsKit.MongoDB;

namespace FacebookCommunityAnalytics.Api.MongoDB
{
    [DependsOn(
        typeof(ApiDomainModule),
        typeof(AbpPermissionManagementMongoDbModule),
        typeof(AbpSettingManagementMongoDbModule),
        typeof(AbpIdentityProMongoDbModule),
        typeof(AbpIdentityServerMongoDbModule),
        typeof(AbpBackgroundJobsMongoDbModule),
        typeof(AbpAuditLoggingMongoDbModule),
        typeof(AbpFeatureManagementMongoDbModule),
        typeof(LanguageManagementMongoDbModule),
        typeof(SaasMongoDbModule),
        typeof(TextTemplateManagementMongoDbModule),
        typeof(CmsKitProMongoDbModule),
        typeof(BlobStoringDatabaseMongoDbModule)
    )]
    public class ApiMongoDbModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            
            context.Services.AddMongoDbContext<ApiMongoDbContext>(options =>
            {
                options.AddDefaultRepositories(true);

                options.AddRepository<Group, Groups.MongoGroupRepository>();

                options.AddRepository<Category, Categories.MongoCategoryRepository>();

                options.AddRepository<Post, MongoPostRepository>();
                options.AddRepository<Tiktok, MongoTiktokRepository>();

                options.AddRepository<Proxy, Proxies.MongoProxyRepository>();

                options.AddRepository<Account, Accounts.MongoAccountRepository>();

                options.AddRepository<AccountProxy, AccountProxies.MongoAccountProxyRepository>();

                options.AddRepository<UserInfo, UserInfos.MongoUserInfoRepository>();

                options.AddRepository<Payroll, Payrolls.MongoPayrollRepository>();

                options.AddRepository<UserPayroll, UserPayrolls.MongoUserPayrollRepository>();

                options.AddRepository<UserWave, UserWaves.MongoUserWaveRepository>();

                options.AddRepository<UserPayrollCommission, UserPayrollCommissions.MongoUserPayrollCommissionRepository>();

                options.AddRepository<UserPayrollBonus, UserPayrollBonuses.MongoUserPayrollBonusRepository>();

                options.AddRepository<UncrawledPost, UncrawledPosts.MongoUncrawledPostRepository>();

                options.AddRepository<UserAffiliate, MongoUserAffiliateRepository>();

                options.AddRepository<Partner, Partners.MongoPartnerRepository>();

                options.AddRepository<Campaign, Campaigns.MongoCampaignRepository>();

                options.AddRepository<AffiliateStat, AffiliateStats.MongoAffiliateStatRepository>();

                options.AddRepository<Contract, MongoContractRepository>();
                
                options.AddRepository<UserSalaryConfiguration, MongoUserSalaryConfigurationRepository>();
                
                options.AddRepository<TikTokMCN, MongoTikTokMCNRepository>();
            });

            Configure<AbpUnitOfWorkDefaultOptions>(options =>
            {
                options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
            });
        }
    }
}