using FacebookCommunityAnalytics.Api.AffiliateStats;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.UserPayrollBonuses;
using FacebookCommunityAnalytics.Api.UserWaves;
using FacebookCommunityAnalytics.Api.UserPayrolls;
using FacebookCommunityAnalytics.Api.UserPayrollCommissions;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.AccountProxies;
using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.Proxies;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Categories;
using FacebookCommunityAnalytics.Api.Groups;
using AutoMapper;
using FacebookCommunityAnalytics.Api.ApiConfigurations;
using FacebookCommunityAnalytics.Api.CmsSites;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.GroupCosts;
using FacebookCommunityAnalytics.Api.Medias;
using FacebookCommunityAnalytics.Api.ScheduledPosts;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using FacebookCommunityAnalytics.Api.TikTokMCNs;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.UncrawledPosts;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserCompensations;
using FacebookCommunityAnalytics.Api.UserEvaluationConfigurations;
using FacebookCommunityAnalytics.Api.UserSalaryConfigurations;
using Volo.Abp.AutoMapper;

namespace FacebookCommunityAnalytics.Api.Blazor
{
    public class ApiBlazorAutoMapperProfile : Profile
    {
        public ApiBlazorAutoMapperProfile()
        {
            //Define your AutoMapper configuration here for the Blazor project.

            CreateMap<GroupDto, GroupUpdateDto>();

            CreateMap<CategoryDto, CategoryUpdateDto>();

            CreateMap<PostDto, PostUpdateDto>();
            CreateMap<PostDto, PostUpdateNoteDto>();

            CreateMap<PostImportDto, PostCreateDto>();

            CreateMap<PostImportDto, PostCreateDto>();

            CreateMap<ScheduledPostDto, ScheduledPostUpdateDto>();

            CreateMap<ProxyDto, ProxyUpdateDto>();

            CreateMap<AccountDto, AccountUpdateDto>();

            CreateMap<AccountProxyDto, AccountProxyUpdateDto>();

            CreateMap<UserInfoDto, UserInfoUpdateDto>();

            CreateMap<PayrollDto, PayrollUpdateDto>();

            CreateMap<UserPayrollCommissionDto, UserPayrollCommissionUpdateDto>();

            CreateMap<UserPayrollDto, UserPayrollUpdateDto>();

            CreateMap<UserWaveDto, UserWaveUpdateDto>();

            CreateMap<UserPayrollBonusDto, UserPayrollBonusUpdateDto>();

            CreateMap<ApiConfigurationDto, ApiConfigurationUpdateDto>();
            CreateMap<UserAffiliateDto, UserAffiliateUpdateDto>();

            CreateMap<PartnerDto, PartnerUpdateDto>();

            CreateMap<CampaignDto, CampaignUpdateDto>();
            CreateMap<UncrawledPostDto,UncrawledPostUpdateDto>();
            CreateMap<AffiliateStatDto, AffiliateStatUpdateDto>();
            CreateMap<ContractDto, CreateUpdateContractDto>();

            CreateMap<MediaDto, MediaCreateUpdateDto>();
            
            CreateMap<StaffEvaluationDto, CreateUpdateStaffEvaluationDto>();

            CreateMap<StaffEvaluationCriteriaDto, CreateUpdateStaffEvaluationCriteriaDto>();

            CreateMap<UserBonusConfigDto, CreateUpdateUserBonusConfigDto>();
            CreateMap<UserEvaluationConfigurationDto, UserEvaluationConfigurationCreateUpdateDto>();
            CreateMap<ContractTransactionDto, CreateUpdateContractTransactionDto>();
            CreateMap<TiktokDto, TiktokCreateUpdateDto>();
            CreateMap<UserSalaryConfigurationDto, UpdateUserSalaryConfigurationDto>().ReverseMap();
            
            CreateMap<TikTokMCNDto, CreateUpdateTikTokMCNDto>();
            CreateMap<GroupCost, GroupCostDto>().ReverseMap();
            CreateMap<GroupCostDto, GroupCostInfoDto>().ForMember(_ => _.Id, opt => opt.MapFrom(src => src.Id)).ReverseMap();

        }
    }

    public class CmsBlazorAutoMapperProfile : Profile
    {
        public CmsBlazorAutoMapperProfile()
        {
            CreateMap<CmsSiteDto, CreateUpdateCmsSiteDto>();
        }
    }
}