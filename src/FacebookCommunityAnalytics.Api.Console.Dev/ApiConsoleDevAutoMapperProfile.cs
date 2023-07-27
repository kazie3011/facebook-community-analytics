using System;
using AutoMapper;
using FacebookCommunityAnalytics.Api.AccountProxies;
using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.AffiliateStats;
using FacebookCommunityAnalytics.Api.ApiConfigurations;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Categories;
using FacebookCommunityAnalytics.Api.Cms.Pages;
using FacebookCommunityAnalytics.Api.CmsSites;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.DashBoards;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Medias;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Proxies;
using FacebookCommunityAnalytics.Api.ScheduledPosts;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.UncrawledPosts;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserCompensations;
using FacebookCommunityAnalytics.Api.UserEvaluationConfigurations;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.UserPayrollBonuses;
using FacebookCommunityAnalytics.Api.UserPayrollCommissions;
using FacebookCommunityAnalytics.Api.UserPayrolls;
using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.UserWaves;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.Console.Dev
{
    public class ApiConsoleDevAutoMapperProfile : Profile
    {
        public ApiConsoleDevAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */

            CreateMap<AppUser, AppUserDto>().Ignore(x => x.ExtraProperties);
            CreateMap<AppUserDto, AppUser>().Ignore(x => x.ExtraProperties);

            CreateMap<AppUser, LookupDto<Guid?>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.UserName} ({src.Email})"));
            CreateMap<AppUser, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.UserName} ({src.Email})"));
            CreateMap<IdentityUser, LookupDto<Guid?>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.UserName} ({src.Email})"));
            CreateMap<IdentityUser, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.UserName} ({src.Email})"));

            CreateMap<AppOrganizationUnit, OrganizationUnitDto>();
            CreateMap<OrganizationUnit, OrganizationUnitDto>();

            CreateMap<AffiliateStatCreateDto, AffiliateStat>().Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<AffiliateStatUpdateDto, AffiliateStat>().Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<AffiliateStat, AffiliateStatDto>();
            CreateMap<Group, LookupDto<Guid?>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.Title}({src.GroupSourceType})"));

            CreateMap<OrganizationUnit, OrganizationUnitDto>().Ignore(x => x.ExtraProperties);
            CreateMap<OrganizationUnitDto, OrganizationUnit>().Ignore(x => x.ExtraProperties);

            CreateMap<UncrawledPostCreateDto, UncrawledPost>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Id)
                .Ignore(x => x.TenantId);
            CreateMap<UncrawledPostUpdateDto, UncrawledPost>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Id)
                .Ignore(x => x.TenantId);
            CreateMap<UncrawledPost, UncrawledPostDto>();

            CreateMap<GroupCreateDto, Group>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<GroupUpdateDto, Group>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<Group, GroupDto>();
            CreateMap<GroupStats, GroupStatsDto>();
            CreateMap<GroupStatsDto, GroupStats>();
            CreateMap<GroupCrawlInfo, GroupCrawlInfoDto>();

            CreateMap<CategoryCreateDto, Category>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<CategoryUpdateDto, Category>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<Category, CategoryDto>();

            CreateMap<CategoryCreateDto, Category>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<CategoryUpdateDto, Category>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<Category, CategoryDto>();


            CreateMap<PostCreateDto, Post>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<PostUpdateDto, Post>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<PostUpdateManyDto, Post>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<Post, PostDto>();
            CreateMap<Post, CampaignPost>();
            CreateMap<CampaignPost, CampaignPostDto>();
            CreateMap<PostImportDto, Post>();

            CreateMap<ScheduledPostCreateDto, ScheduledPost>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Id)
                .Ignore(x => x.TenantId);
            CreateMap<ScheduledPostUpdateDto, ScheduledPost>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Id)
                .Ignore(x => x.TenantId);
            CreateMap<ScheduledPost, ScheduledPostDto>();
            CreateMap<SchedulePostWithNavigationProperties, SchedulePostWithNavigationPropertiesDto>();

            CreateMap<ApiConfigurationCreateDto, ApiConfiguration>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Id)
                .Ignore(x => x.TenantId);
            CreateMap<ApiConfigurationUpdateDto, ApiConfiguration>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Id)
                .Ignore(x => x.TenantId);
            CreateMap<ApiConfiguration, ApiConfigurationDto>();

            CreateMap<UserAffiliateCreateDto, UserAffiliate>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Id)
                .Ignore(x => x.TenantId);
            CreateMap<UserAffiliateUpdateDto, UserAffiliate>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Id)
                .Ignore(x => x.TenantId);
            CreateMap<UserAffiliate, UserAffiliateDto>();
            CreateMap<UserAffiliateWithNavigationProperties, UserAffiliateWithNavigationPropertiesDto>();

            CreateMap<UserAffiliate, AffTopLinkSummaryApiItem>()
                .ForMember(row => row.Shortlink, opt => opt.MapFrom(src => src.AffiliateUrl))
                .ForMember(row => row.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(row => row.Click, opt => opt.MapFrom(src => src.AffConversionModel.ClickCount))
                .ForMember(row => row.Conversion, opt => opt.MapFrom(src => src.AffConversionModel.ConversionCount))
                .ForMember(row => row.Commission, opt => opt.MapFrom(src => src.AffConversionModel.CommissionAmount))
                .ForMember(row => row.Amount, opt => opt.MapFrom(src => src.AffConversionModel.ConversionAmount));

            CreateMap<PostWithNavigationProperties, PostWithNavigationPropertiesDto>();
            CreateMap<Category, LookupDto<Guid?>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

            CreateMap<ProxyCreateDto, Proxy>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<ProxyUpdateDto, Proxy>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<Proxy, ProxyDto>();
            CreateMap<ProxyImportDto, Proxy>();

            CreateMap<AccountCreateDto, Account>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<AccountUpdateDto, Account>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<Account, AccountDto>();
            CreateMap<AccountImportDto, Account>();

            CreateMap<Account, ExportAccountDto>()
                .ForMember(row => row.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(row => row.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(row => row.TwoFactorCode, opt => opt.MapFrom(src => src.TwoFactorCode))
                .ForMember(row => row.AccountType, opt => opt.MapFrom(src => src.AccountType))
                .ForMember(row => row.AccountStatus, opt => opt.MapFrom(src => src.AccountStatus))
                .ForMember(row => row.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(row => row.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(row => row.EmailPassword, opt => opt.MapFrom(src => src.EmailPassword))
                .ForMember(row => row.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<AccountProxyCreateDto, AccountProxy>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Id)
                .Ignore(x => x.TenantId);
            CreateMap<AccountProxyUpdateDto, AccountProxy>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Id)
                .Ignore(x => x.TenantId);
            CreateMap<AccountProxy, AccountProxyDto>();
            CreateMap<AccountProxyWithNavigationProperties, AccountProxyWithNavigationPropertiesDto>();
            CreateMap<Account, LookupDto<Guid?>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Username));
            CreateMap<Proxy, LookupDto<Guid?>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Ip));

            CreateMap<Post, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Url));

            CreateMap<PostWithNavigationProperties, PostDetailExportRow>()
                .ForMember(row => row.Author, opt => opt.MapFrom(src => src.AppUser.UserName))
                .ForMember(row => row.Group, opt => opt.MapFrom(src => src.Group.Title))
                .ForMember(row => row.PostContentType, opt => opt.MapFrom(src => src.Post.PostContentType))
                .ForMember(row => row.ShortUrl, opt => opt.MapFrom(src => string.Join(' ', src.Post.Shortlinks)))
                .ForMember(row => row.Url, opt => opt.MapFrom(src => src.Post.Url))
                .ForMember(row => row.Like, opt => opt.MapFrom(src => src.Post.LikeCount))
                .ForMember(row => row.Comment, opt => opt.MapFrom(src => src.Post.CommentCount))
                .ForMember(row => row.Share, opt => opt.MapFrom(src => src.Post.ShareCount))
                .ForMember(row => row.Total, opt => opt.MapFrom(src => src.Post.TotalCount))
                .ForMember(row => row.Hashtag, opt => opt.MapFrom(src => src.Post.Hashtag))
                .ForMember(row => row.Hashtag, opt => opt.MapFrom(src => src.Post.Hashtag))
                .ForMember(row => row.IsNotAvailable, opt => opt.MapFrom(src => src.Post.IsNotAvailable))
                .ForMember(row => row.CreatedDateTime, opt => opt.MapFrom(src => src.Post.CreatedDateTime))
                .ForMember(row => row.LastCrawledDateTime, opt => opt.MapFrom(src => src.Post.LastCrawledDateTime))
                .ForMember(row => row.SubmissionDateTime, opt => opt.MapFrom(src => src.Post.SubmissionDateTime));

            CreateMap<PostWithNavigationProperties, PostExportRow>()
                .ForMember(row => row.Author, opt => opt.MapFrom(src => src.Post.CreatedBy))
                .ForMember(row => row.Group, opt => opt.MapFrom(src => $"{src.Group.Title}({src.Group.GroupSourceType})"))
                .ForMember(row => row.PostContentType, opt => opt.MapFrom(src => src.Post.PostContentType))
                .ForMember(row => row.Url, opt => opt.MapFrom(src => src.Post.Url))
                .ForMember(row => row.Like, opt => opt.MapFrom(src => src.Post.LikeCount))
                .ForMember(row => row.Comment, opt => opt.MapFrom(src => src.Post.CommentCount))
                .ForMember(row => row.Share, opt => opt.MapFrom(src => src.Post.ShareCount))
                .ForMember(row => row.Total, opt => opt.MapFrom(src => src.Post.TotalCount));

            CreateMap<UserInfoCreateDto, UserInfo>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<UserInfoUpdateDto, UserInfo>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<UserInfo, UserInfoDto>();
            CreateMap<UserInfo, LookupDto<Guid?>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.Code}"));
            CreateMap<UserInfoWithNavigationProperties, UserInfoWithNavigationPropertiesDto>();

            CreateMap<PayrollCreateDto, Payroll>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<PayrollUpdateDto, Payroll>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<Payroll, PayrollDto>();

            CreateMap<UserPayrollCommissionCreateDto, UserPayrollCommission>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Id)
                .Ignore(x => x.TenantId);
            CreateMap<UserPayrollCommissionUpdateDto, UserPayrollCommission>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Id)
                .Ignore(x => x.TenantId);
            CreateMap<UserPayrollCommission, UserPayrollCommissionDto>();
            CreateMap<UserPayrollCommissionWithNavigationProperties, UserPayrollCommissionWithNavigationPropertiesDto>();
            CreateMap<Payroll, LookupDto<Guid?>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Code));

            CreateMap<UserPayrollCreateDto, UserPayroll>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<UserPayrollUpdateDto, UserPayroll>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<UserPayroll, UserPayrollDto>();
            CreateMap<UserPayrollWithNavigationProperties, UserPayrollWithNavigationPropertiesDto>();

            CreateMap<UserWaveCreateDto, UserWave>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<UserWaveUpdateDto, UserWave>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<UserWave, UserWaveDto>();
            CreateMap<UserWaveWithNavigationProperties, UserWaveWithNavigationPropertiesDto>();
            CreateMap<UserPayroll, LookupDto<Guid?>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Code));

            CreateMap<UserPayrollBonusCreateDto, UserPayrollBonus>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Id)
                .Ignore(x => x.TenantId);
            CreateMap<UserPayrollBonusUpdateDto, UserPayrollBonus>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Id)
                .Ignore(x => x.TenantId);
            CreateMap<UserPayrollBonus, UserPayrollBonusDto>();
            CreateMap<UserPayrollBonusWithNavigationProperties, UserPayrollBonusWithNavigationPropertiesDto>();

            CreateMap<PayrollDetail, PayrollDetailResponse>();
            CreateMap<PayrollResponse, PayrollDetailResponse>();

            CreateMap<PartnerCreateDto, Partner>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<PartnerUpdateDto, Partner>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<Partner, PartnerDto>();
            CreateMap<CampaignCreateDto, Campaign>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<CampaignUpdateDto, Campaign>().IgnoreFullAuditedObjectProperties().Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id).Ignore(x => x.TenantId);
            CreateMap<Campaign, CampaignDto>();
            CreateMap<CampaignTarget, CampaignTargetDto>().ReverseMap();
            CreateMap<CampaignPrize, CampaignPrizeDto>().ReverseMap();
            CreateMap<CampaignWithNavigationProperties, CampaignWithNavigationPropertiesDto>();
            CreateMap<Partner, LookupDto<Guid?>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
            CreateMap<Campaign, LookupDto<Guid?>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
            CreateMap<UserAffiliateWithNavigationProperties, UserAffiliateExportRow>()
                .ForMember(row => row.MarketplaceType, opt => opt.MapFrom(src => src.UserAffiliate.MarketplaceType))
                .ForMember(row => row.Url, opt => opt.MapFrom(src => src.UserAffiliate.Url))
                .ForMember(row => row.AffiliateUrl, opt => opt.MapFrom(src => src.UserAffiliate.AffiliateUrl))
                .ForMember(row => row.ClickCount, opt => opt.MapFrom(src => src.UserAffiliate.AffConversionModel.ClickCount))
                .ForMember(row => row.ConversionCount, opt => opt.MapFrom(src => src.UserAffiliate.AffConversionModel.ConversionCount))
                .ForMember(row => row.ConversionAmount, opt => opt.MapFrom(src => src.UserAffiliate.AffConversionModel.ConversionAmount))
                .ForMember(row => row.CommissionAmount, opt => opt.MapFrom(src => src.UserAffiliate.AffConversionModel.CommissionAmount))
                .ForMember(row => row.CreatedAt, opt => opt.MapFrom(src => src.UserAffiliate.CreatedAt))
                .ForMember(row => row.Group, opt => opt.MapFrom(src => src.Group.Title))
                .ForMember(row => row.Campaign, opt => opt.MapFrom(src => src.Campaign.Name));

            CreateMap<MediaCreateUpdateDto, Media>().IgnoreAuditedObjectProperties().Ignore(x => x.Id);
            CreateMap<Media, MediaDto>();

            CreateMap<GroupStatsHistoryCreateUpdateDto, GroupStatsHistory>().IgnoreAuditedObjectProperties().Ignore(x => x.Id);
            CreateMap<GroupStatsHistory, GroupStatsHistoryDto>();

            CreateMap<ContractWithNavigationProperties, ContractWithNavigationPropertiesDto>();
            CreateMap<Contract, ContractDto>().ReverseMap();
            CreateMap<CreateUpdateContractDto, Contract>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);
            
            CreateMap<ContractTransaction, ContractTransactionDto>();
            CreateMap<CreateUpdateContractTransactionDto, ContractTransaction>().ReverseMap();

            CreateMap<UserDetail, TeamMemberDto>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(src => src.Identity.Id))
                .ForMember(_ => _.UserName, opt => opt.MapFrom(src => src.Identity.UserName))
                .ForMember(_ => _.Name, opt => opt.MapFrom(src => src.Identity.Name))
                .ForMember(_ => _.Surname, opt => opt.MapFrom(src => src.Identity.Surname))
                .ForMember(_ => _.Email, opt => opt.MapFrom(src => src.Identity.Email))
                .ForMember(_ => _.PhoneNumber, opt => opt.MapFrom(src => src.Identity.PhoneNumber))
                .ForMember(_ => _.UserCode, opt => opt.MapFrom(src => src.Info.Code))
                .ForMember(_ => _.IsActive, opt => opt.MapFrom(src => src.Info.IsActive))
                .ForMember(_ => _.Position, opt => opt.MapFrom(src => src.Info.UserPosition))
                .ForMember(_ => _.IsSystemUser, opt => opt.MapFrom(src => src.Info.IsSystemUser))
                .ForMember(_ => _.IsGDLStaff, opt => opt.MapFrom(src => src.Info.IsGDLStaff))
                .ForMember(_ => _.IsCalculatePayrollUser, opt => opt.MapFrom(src => src.Info.EnablePayrollCalculation))
                .ForMember(_ => _.Teams, opt => opt.MapFrom(src => src.Teams));

            CreateMap<UserDetail, DashboardUserDto>()
                .ForMember(_ => _.Id, opt => opt.MapFrom(src => src.Identity.Id))
                .ForMember(_ => _.UserName, opt => opt.MapFrom(src => src.Identity.UserName))
                .ForMember(_ => _.UserCode, opt => opt.MapFrom(src => src.Info.Code));

            CreateMap<StaffEvaluation, StaffEvaluationDto>().ReverseMap();
            CreateMap<CreateUpdateStaffEvaluationDto, StaffEvaluation>().IgnoreAuditedObjectProperties().Ignore(x => x.Id);
            CreateMap<StaffEvaluationWithNavigationProperties, StaffEvaluationWithNavigationPropertiesDto>().ReverseMap();
            CreateMap<StaffEvaluation, TikTokChannelEvaluationExport>()
                .ForMember(_ => _.TotalPoint, opt => opt.MapFrom(scr => scr.TotalPoint))
                .ForMember(_ => _.QuantityKPI, opt => opt.MapFrom(scr => scr.QuantityKPI))
                .ForMember(_ => _.QualityKPI, opt => opt.MapFrom(scr => scr.QualityKPI))
                .ForMember(_ => _.ReviewPoint, opt => opt.MapFrom(scr => scr.ReviewPoint))
                .ForMember(_ => _.DirectorReview, opt => opt.MapFrom(scr => scr.DirectorReview))
                .ForMember(_ => _.QuantityKPIDescription, opt => opt.MapFrom(scr => scr.QuantityKPIDescription))
                .ForMember(_ => _.QualityKPIDescription, opt => opt.MapFrom(scr => scr.QualityKPIDescription));

            CreateMap<StaffEvaluationCriteria, StaffEvaluationCriteriaDto>().ReverseMap();
            CreateMap<CreateUpdateStaffEvaluationCriteriaDto, StaffEvaluationCriteria>().IgnoreAuditedObjectProperties().Ignore(x => x.Id);
            CreateMap<GetTiktoksInputExtend, UpdateTiktokStateApiRequest>().ReverseMap();

            CreateMap<Tiktok, TiktokDto>().ReverseMap();
            CreateMap<TiktokWithNavigationProperties, TiktokWithNavigationPropertiesDto>();
            CreateMap<TiktokWithNavigationProperties, TiktokExportRow>()
                .ForMember(row => row.Channel, opt => opt.MapFrom(src => src.Tiktok.ChannelId))
                .ForMember(row => row.Url, opt => opt.MapFrom(src => src.Tiktok.Url))
                .ForMember(row => row.CreatedDateTime, opt => opt.MapFrom(src => src.Tiktok.CreatedDateTime))
                .ForMember(row => row.Fid, opt => opt.MapFrom(src => src.Tiktok.VideoId))
                .ForMember(row => row.Category, opt => opt.MapFrom(src => src.Group != null ? src.Group.GroupCategoryType.ToString() : string.Empty));

            CreateMap<SaleEvaluationConfiguration, SaleEvaluationConfigurationDto>().ReverseMap();
            CreateMap<TiktokEvaluationConfiguration, TiktokEvaluationConfigurationDto>().ReverseMap();
            CreateMap<ContentEvaluationConfiguration, ContentEvaluationConfigurationDto>().ReverseMap();
            CreateMap<AffiliateEvaluationConfiguration, AffiliateEvaluationConfigurationDto>().ReverseMap();
            CreateMap<SeedingEvaluationConfiguration, SeedingEvaluationConfigurationDto>().ReverseMap();
            CreateMap<UserEvaluationConfiguration, UserEvaluationConfigurationDto>().ReverseMap();
            CreateMap<UserEvaluationConfigurationCreateUpdateDto, UserEvaluationConfiguration>().ReverseMap();

            CreateMap<UserCompensation, UserCompensationDto>();
            CreateMap<UserCompensationNavigationProperties, UserCompensationNavigationPropertiesDto>();
            CreateMap<UserCompensationBonus, UserCompensationBonusDto>();
            CreateMap<CreateUpdateUserCompensationDto, UserCompensation>().Ignore(x => x.Id).IgnoreAuditedObjectProperties();

            CreateMap<UserSalary, UserSalaryDto>();
            CreateMap<CreateUpdateUserSalaryDto, UserSalary>().Ignore(x => x.Id).IgnoreAuditedObjectProperties();

            CreateMap<UserBonusConfig, UserBonusConfigDto>();
            CreateMap<CreateUpdateUserBonusConfigDto, UserBonusConfig>().Ignore(x => x.Id).IgnoreAuditedObjectProperties();
        }
    }

    public class CmsApplicationAutoMapperProfile : Profile
    {
        public CmsApplicationAutoMapperProfile()
        {
            CreateMap<CmsPage, CmsPageDto>();
            CreateMap<CreateUpdateCmsPageDto, CmsPage>().IgnoreAuditedObjectProperties().Ignore(x => x.Id);
            CreateMap<CmsSite, CmsSiteDto>();
            CreateMap<CreateUpdateCmsSiteDto, CmsSite>().IgnoreAuditedObjectProperties().Ignore(x => x.Id);
            CreateMap<SiteSEO, SiteSEODto>().ReverseMap();
            CreateMap<FooterSite, FooterSiteDto>().ReverseMap();
        }
    }
}