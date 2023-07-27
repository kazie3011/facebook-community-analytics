using FacebookCommunityAnalytics.Api.AffiliateStats;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.UncrawledPosts;
using FacebookCommunityAnalytics.Api.UserPayrollBonuses;
using FacebookCommunityAnalytics.Api.UserWaves;
using FacebookCommunityAnalytics.Api.UserPayrolls;
using FacebookCommunityAnalytics.Api.UserPayrollCommissions;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.UserInfos;
using MongoDB.Driver;
using FacebookCommunityAnalytics.Api.Proxies;
using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.AccountProxies;
using FacebookCommunityAnalytics.Api.AffiliateConversions;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Categories;
using FacebookCommunityAnalytics.Api.ScheduledPosts;
using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.ApiConfigurations;
using FacebookCommunityAnalytics.Api.AutoPosts;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.GroupCosts;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Integrations.Tiki.TikiAffiliates;
using FacebookCommunityAnalytics.Api.Integrations.Tiktok;
using FacebookCommunityAnalytics.Api.Medias;
using FacebookCommunityAnalytics.Api.ScheduledPostGroups;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserAffiliateStats;
using FacebookCommunityAnalytics.Api.UserCompensations;
using FacebookCommunityAnalytics.Api.UserEvaluationConfigurations;
using FacebookCommunityAnalytics.Api.UserSalaryConfigurations;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace FacebookCommunityAnalytics.Api.MongoDB
{
    [ConnectionStringName("Default")]
    public class ApiMongoDbContext : AbpMongoDbContext
    {
        public IMongoCollection<AffiliateStat> AffiliateStats => Collection<AffiliateStat>();
        public IMongoCollection<Campaign> Campaigns => Collection<Campaign>();
        public IMongoCollection<Partner> Partners => Collection<Partner>();
        public IMongoCollection<ApiConfiguration> ApiConfigurations => Collection<ApiConfiguration>();
        public IMongoCollection<UncrawledPost> UncrawledPosts => Collection<UncrawledPost>();
        public IMongoCollection<UserPayrollBonus> UserPayrollBonuses => Collection<UserPayrollBonus>();
        public IMongoCollection<UserWave> UserWaves => Collection<UserWave>();
        public IMongoCollection<UserPayroll> UserPayrolls => Collection<UserPayroll>();
        public IMongoCollection<UserPayrollCommission> UserPayrollCommissions => Collection<UserPayrollCommission>();
        public IMongoCollection<Payroll> Payrolls => Collection<Payroll>();
        public IMongoCollection<UserInfo> UserInfos => Collection<UserInfo>();
        public IMongoCollection<AccountProxy> AccountProxies => Collection<AccountProxy>();
        public IMongoCollection<Account> Accounts => Collection<Account>();
        public IMongoCollection<Proxy> Proxies => Collection<Proxy>();
        public IMongoCollection<Post> Posts => Collection<Post>();
        public IMongoCollection<Tiktok> Tiktoks => Collection<Tiktok>();
        public IMongoCollection<ScheduledPostGroup> ScheduledPostGroups => Collection<ScheduledPostGroup>();
        public IMongoCollection<ScheduledPost> ScheduledPosts => Collection<ScheduledPost>();
        public IMongoCollection<Category> Categories => Collection<Category>();
        public IMongoCollection<Group> Groups => Collection<Group>();
        public IMongoCollection<AppUser> Users => Collection<AppUser>();
        public IMongoCollection<AppOrganizationUnit> OrganizationUnits => Collection<AppOrganizationUnit>();
        public IMongoCollection<UserAffiliate> UserAffiliates => Collection<UserAffiliate>();
        public IMongoCollection<Media> Medias => Collection<Media>();
        public IMongoCollection<GroupStatsHistory> GroupStatsHistories => Collection<GroupStatsHistory>();
        public IMongoCollection<UserAffiliateStat> UserAffiliateStats => Collection<UserAffiliateStat>();
        public IMongoCollection<TikiAffiliate> TikiAffiliates => Collection<TikiAffiliate>();
        public IMongoCollection<TikiAffiliateStat> TikiAffiliateStats => Collection<TikiAffiliateStat>();
        public IMongoCollection<TiktokStat> TiktokStat => Collection<TiktokStat>();
        public IMongoCollection<TiktokVideoStat> TiktokVideoStats => Collection<TiktokVideoStat>();
        
        public IMongoCollection<TikTokMCN> TikTokMCNs => Collection<TikTokMCN>();
        public IMongoCollection<TrendingDetail> TrendingDetails => Collection<TrendingDetail>();
        public IMongoCollection<MCNVietNamChannel> MCNVietNamChannels => Collection<MCNVietNamChannel>();
        public IMongoCollection<TikiAffiliateConversion> TikiAffiliateConversion => Collection<TikiAffiliateConversion>();
        public IMongoCollection<Contract> Contracts => Collection<Contract>();
        public IMongoCollection<GroupCost> GroupCosts => Collection<GroupCost>();

        public IMongoCollection<StaffEvaluation> StaffEvaluations => Collection<StaffEvaluation>();
        public IMongoCollection<StaffEvaluationCriteria> StaffEvaluationCriteria => Collection<StaffEvaluationCriteria>();

        public IMongoCollection<ContractTransaction> ContractTransactions => Collection<ContractTransaction>();

        public IMongoCollection<UserEvaluationConfiguration> UserEvaluationConfigurations => Collection<UserEvaluationConfiguration>();
        public IMongoCollection<AffiliateConversion> AffiliateConversions => Collection<AffiliateConversion>();

        //UserComposation
        public IMongoCollection<UserSalary> UserSalaryHistories => Collection<UserSalary>();
        public IMongoCollection<UserCompensation> UserCompensations => Collection<UserCompensation>();
        public IMongoCollection<UserBonusConfig> UserBonusConfigs => Collection<UserBonusConfig>();
        public IMongoCollection<PostHistory> PostHistories => Collection<PostHistory>();
        public IMongoCollection<UserSalaryConfiguration> UserSalaryConfigurations => Collection<UserSalaryConfiguration>();

        public IMongoCollection<AutoPostFacebook> AutoPostFacebooks => Collection<AutoPostFacebook>();
        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);

            modelBuilder.Entity<AppUser>
            (
                b =>
                {
                    /* Sharing the same "AbpUsers" collection
                     * with the Identity module's IdentityUser class. */
                    b.CollectionName = "AbpUsers";
                }
            );

            modelBuilder.Entity<AppOrganizationUnit>
            (
                b =>
                {
                    /* Sharing the same "AbpOrganizationUnits" collection */
                    b.CollectionName = "AbpOrganizationUnits";
                }
            );

            modelBuilder.Entity<Group>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "Groups"; });
            modelBuilder.Entity<Category>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "Categories"; });
            modelBuilder.Entity<Post>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "Posts"; });
            modelBuilder.Entity<Tiktok>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "Tiktoks"; });
            modelBuilder.Entity<ScheduledPost>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "ScheduledPosts"; });
            modelBuilder.Entity<ScheduledPostGroup>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "ScheduledPostGroups"; });
            modelBuilder.Entity<Proxy>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "Proxies"; });
            modelBuilder.Entity<Account>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "Accounts"; });
            modelBuilder.Entity<AccountProxy>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "AccountProxies"; });
            modelBuilder.Entity<UserInfo>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "UserInfos"; });
            modelBuilder.Entity<Payroll>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "Payrolls"; });
            modelBuilder.Entity<UserPayrollCommission>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "UserPayrollCommissions"; });
            modelBuilder.Entity<UserPayroll>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "UserPayrolls"; });
            modelBuilder.Entity<UserWave>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "UserWaves"; });
            modelBuilder.Entity<UserPayrollBonus>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "UserPayrollBonuses"; });
            modelBuilder.Entity<UncrawledPost>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "UncrawledPosts"; });
            modelBuilder.Entity<ApiConfiguration>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "ApiConfigurations"; });
            modelBuilder.Entity<UserAffiliate>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "UserAffiliates"; });
            modelBuilder.Entity<Partner>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "Partners"; });
            modelBuilder.Entity<Campaign>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "Campaigns"; });
            modelBuilder.Entity<AffiliateStat>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "AffiliateStats"; });
            modelBuilder.Entity<Media>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "Medias"; });
            modelBuilder.Entity<GroupStatsHistory>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "GroupStatsHistories"; });
            modelBuilder.Entity<UserAffiliateStat>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "UserAffiliateStats"; });
            modelBuilder.Entity<TikiAffiliate>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "TikiAffiliates"; });
            modelBuilder.Entity<TikiAffiliateStat>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "TikiAffiliateStats"; });
            modelBuilder.Entity<TikiAffiliateConversion>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "TikiAffiliateConversions"; });
            modelBuilder.Entity<Contract>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "Contracts"; });
            modelBuilder.Entity<GroupCost>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "GroupCosts"; });
            modelBuilder.Entity<TiktokStat>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "TiktokStats"; });
            modelBuilder.Entity<StaffEvaluation>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "StaffEvaluations"; });
            modelBuilder.Entity<StaffEvaluationCriteria>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "StaffEvaluationCriteria"; });
            modelBuilder.Entity<ContractTransaction>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "TransactionHistories"; });
            modelBuilder.Entity<UserEvaluationConfiguration>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "UserEvaluationConfigurations"; });
            modelBuilder.Entity<AffiliateConversion>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "AffiliateConversions"; });

            //User Composation
            modelBuilder.Entity<UserCompensation>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "UserCompensations"; });
            modelBuilder.Entity<UserSalary>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "UserSalaryHistories"; });
            modelBuilder.Entity<UserBonusConfig>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "UserBonusConfigs"; });
            
            modelBuilder.Entity<PostHistory>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "PostHistories"; });
            modelBuilder.Entity<TikTokMCN>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "TikTokMCNs"; });
            modelBuilder.Entity<MCNVietNamChannel>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "MCNVietNamChannels"; });
            modelBuilder.Entity<TrendingDetail>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "TrendingDetails"; });
            modelBuilder.Entity<UserSalaryConfiguration>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "UserSalaryConfigurations"; });
            
            modelBuilder.Entity<AutoPostFacebook>(b => { b.CollectionName = ApiConsts.DbTablePrefix + "AutoPostFacebooks"; });
        }
    }
}