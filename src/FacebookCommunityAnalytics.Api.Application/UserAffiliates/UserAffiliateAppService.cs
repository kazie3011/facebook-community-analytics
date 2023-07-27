using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.UserAffiliates
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.UserAffiliates.Default)]
    public class UserAffiliateAppService : ApplicationService, IUserAffiliateAppService
    {
        private readonly IUserAffiliateRepository _userAffiliateRepository;
        private readonly IUserAffiliateDomainService _userAffiliateDomainService;
        private readonly IOrganizationDomainService _organizationDomainService;
        private readonly IRepository<AppUser, Guid> _appUserRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IPostDomainService _postDomainService;

        public UserAffiliateAppService(
            IUserAffiliateRepository userAffiliateRepository,
            IUserAffiliateDomainService userAffiliateDomainService,
            IOrganizationDomainService organizationDomainService,
            IRepository<AppUser, Guid> appUserRepository,
            ICampaignRepository campaignRepository,
            IPostDomainService postDomainService)
        {
            _userAffiliateRepository = userAffiliateRepository;
            _userAffiliateDomainService = userAffiliateDomainService;
            _organizationDomainService = organizationDomainService;
            _appUserRepository = appUserRepository;
            _campaignRepository = campaignRepository;
            _postDomainService = postDomainService;
        }

        public async Task<UserAffDetailDto> GetUserAffDetails(UserAffDetailApiRequest apiRequest)
        {
            return await _userAffiliateDomainService.GetUserAffDetails(apiRequest);
        }

        public async Task<PagedResultDto<UserAffiliateDto>> GetListAsync(GetUserAffiliatesInput input)
        {
            var items = await _userAffiliateRepository.GetListAsync(input.FilterText);
            var count = await _userAffiliateRepository.GetCountAsync(input.FilterText);
            return new PagedResultDto<UserAffiliateDto>()
            {
                Items = ObjectMapper.Map<List<UserAffiliate>, List<UserAffiliateDto>>(items),
                TotalCount = count
            };
        }

        public async Task<UserAffiliateDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<UserAffiliate, UserAffiliateDto>(await _userAffiliateRepository.GetAsync(id));
        }

        public Task DeleteAsync(Guid id)
        {
            return _userAffiliateRepository.DeleteAsync(id);
        }

        public async Task<UserAffiliateDto> CreateAsync(UserAffiliateCreateDto input)
        {
            var entity = ObjectMapper.Map<UserAffiliateCreateDto, UserAffiliate>(input);
            entity = _userAffiliateDomainService.InitUserAffiliateCreation(entity);
            entity.TenantId = CurrentTenant.Id;
            var userAffiliate = await _userAffiliateRepository.InsertAsync(entity);
            return ObjectMapper.Map<UserAffiliate, UserAffiliateDto>(userAffiliate);
        }

        public async Task<UserAffiliateDto> UpdateAsync(Guid id, UserAffiliateUpdateDto input)
        {
            var userAffiliate = await _userAffiliateRepository.GetAsync(id);
            ObjectMapper.Map(input, userAffiliate);
            userAffiliate = await _userAffiliateRepository.UpdateAsync(userAffiliate);
            return ObjectMapper.Map<UserAffiliate, UserAffiliateDto>(userAffiliate);
        }

        public async Task<PagedResultDto<UserAffiliateWithNavigationPropertiesDto>> GetUserAffiliateWithNavigationProperties(GetUserAffiliatesInputExtend input)
        {
            if (input.ConversionOwnerFilter == ConversionOwnerFilter.Own) { input.AppUserId = CurrentUser.Id; }

            var userIds = await _userAffiliateDomainService.UserIds(input, CurrentUser.Id);
            var items = await _userAffiliateRepository.GetUserAffiliateWithNavigationProperties
            (
                input.FilterText,
                input.MarketplaceType,
                input.AffiliateProviderType,
                input.Url,
                input.AffiliateUrl,
                input.CreatedAtMin,
                input.CreatedAtMax,
                input.GroupId,
                input.PartnerId,
                input.CampaignId,
                input.AppUserId,
                userIds,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount,
                input.HasConversion,
                input.Shortlinks
            );
            var count = await _userAffiliateRepository.GetCountAsync
            (
                input.FilterText,
                input.MarketplaceType,
                input.AffiliateProviderType,
                input.Url,
                input.AffiliateUrl,
                input.CreatedAtMin,
                input.CreatedAtMax,
                input.GroupId,
                input.PartnerId,
                input.CampaignId,
                input.AppUserId,
                userIds,
                input.HasConversion,
                input.Shortlinks
            );

            return new PagedResultDto<UserAffiliateWithNavigationPropertiesDto>
            {
                Items = ObjectMapper
                    .Map<List<UserAffiliateWithNavigationProperties>,
                        List<UserAffiliateWithNavigationPropertiesDto>>(items),
                TotalCount = count
            };
        }

        public async Task<UserAffiliateWithNavigationPropertiesDto> GetUserAffiliateWithNavigationProperties(Guid id)
        {
            var userAffiliateWithNavigationProperties = await _userAffiliateRepository.GetUserAffiliateWithNavigationProperties(id);
            return ObjectMapper.Map<UserAffiliateWithNavigationProperties, UserAffiliateWithNavigationPropertiesDto>(userAffiliateWithNavigationProperties);
        }

        public async Task<PagedResultDto<LookupDto<Guid?>>> GetGroupLookupAsync(GroupLookupRequestDto input)
        {
            return await _postDomainService.GetGroupLookupAsync(input);
        }

        public async Task<PagedResultDto<LookupDto<Guid?>>> GetCategoryLookupAsync(LookupRequestDto input)
        {
            return await _postDomainService.GetCategoryLookupAsync(input);
        }

        public async Task<PagedResultDto<LookupDto<Guid?>>> GetPartnerLookupAsync(LookupRequestDto input)
        {
            return await _postDomainService.GetPartnerLookupAsync(input);
        }

        public async Task<PagedResultDto<LookupDto<Guid?>>> GetCampaignLookupAsync(LookupRequestDto input)
        {
            return await _postDomainService.GetCampaignLookupAsync(input);
        }

        public async Task<ShortlinkDto> GenerateAffiliate(GenerateShortlinkApiRequest generateShortlinkApiRequest)
        {
            return await _userAffiliateDomainService.GenerateShortlink(generateShortlinkApiRequest);
        }

        public async Task<List<UserAffiliateDto>> GetListUserAffiliate(GetUserAffiliatesInputExtend input)
        {
            if (input.ConversionOwnerFilter == ConversionOwnerFilter.Own) { input.AppUserId = CurrentUser.Id; }

            var userIds = await _userAffiliateDomainService.UserIds(input, CurrentUser.Id);
            var items =
                await _userAffiliateRepository.GetListAsync
                (
                    input.FilterText,
                    input.MarketplaceType,
                    input.Url,
                    input.AffiliateUrl,
                    input.CreatedAtMin,
                    input.CreatedAtMax,
                    input.GroupId,
                    input.PartnerId,
                    input.CampaignId,
                    input.AppUserId,
                    userIds,
                    hasConversion: input.HasConversion
                );
            return ObjectMapper
                .Map<List<UserAffiliate>,
                    List<UserAffiliateDto>>(items);
        }

        public async Task<PagedResultDto<CampaignAffiliateDto>> GetUserAffiliatePageByCampaignAsync(Guid campaignId, GetUserAffiliatesInputExtend input)
        {
            if (campaignId == Guid.Empty) return null;

            var campaign = await _campaignRepository.GetAsync(campaignId);
            if (input.ConversionOwnerFilter == ConversionOwnerFilter.Own) { input.AppUserId = CurrentUser.Id; }
            
            var userIds = await _userAffiliateDomainService.UserIds(input, CurrentUser.Id);
            var items = await _userAffiliateRepository.GetUserAffiliateWithNavigationProperties
            (
                input.FilterText,
                input.MarketplaceType,
                input.AffiliateProviderType,
                input.Url,
                input.AffiliateUrl,
                input.CreatedAtMin,
                input.CreatedAtMax,
                input.GroupId,
                input.PartnerId,
                input.CampaignId,
                input.AppUserId,
                userIds,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount,
                input.HasConversion,
                input.Shortlinks
            );
            var count = await _userAffiliateRepository.GetCountAsync
            (
                input.FilterText,
                input.MarketplaceType,
                input.AffiliateProviderType,
                input.Url,
                input.AffiliateUrl,
                input.CreatedAtMin,
                input.CreatedAtMax,
                input.GroupId,
                input.PartnerId,
                input.CampaignId,
                input.AppUserId,
                userIds,
                input.HasConversion,
                input.Shortlinks
            );
            var resultItems = items.Select
                (
                    affiliateNav =>
                    {
                        var campaignPost = new CampaignAffiliateDto
                        {
                            Author = $"{affiliateNav.AppUser.UserName}({affiliateNav.UserInfo.Code})",
                            GroupName = $"{affiliateNav.Group.Title}({affiliateNav.Group.GroupSourceType})",
                            MarketplaceType = affiliateNav.UserAffiliate.MarketplaceType,
                            AffiliateProviderType = affiliateNav.UserAffiliate.AffiliateProviderType,
                            Url = affiliateNav.UserAffiliate.Url,
                            Shortlink = affiliateNav.UserAffiliate.AffiliateUrl,
                            ClickCount = affiliateNav.UserAffiliate.AffConversionModel.ClickCount,
                            ConversionCount = affiliateNav.UserAffiliate.AffConversionModel.ConversionCount,
                            ConversionAmount = affiliateNav.UserAffiliate.AffConversionModel.ConversionAmount,
                            CommissionAmount = affiliateNav.UserAffiliate.AffConversionModel.CommissionAmount,
                            CreatedDateTime = affiliateNav.UserAffiliate.CreatedAt,
                            // Progress = (campaign.Target is {AverageClickPerAffiliate: > 0})
                            //     ? ((affiliateNav.UserAffiliate.UserAffiliateConversion.ClickCount / (double)campaign.Target.AverageClickPerAffiliate) * 100)
                            //     : 0
                        };
                        return campaignPost;
                    }
                )
                .ToList();
            return new PagedResultDto<CampaignAffiliateDto>
            {
                Items = resultItems,
                TotalCount = count
            };
        }

        public async Task<UserAffSummaryApiResponse> GetUserAffiliateSummary(UserAffSummaryApiRequest request)
        {
            return await _userAffiliateDomainService.GetUserAffiliateSummary(request);
        }

        public async Task<byte[]> GetUserAffiliateExcelAsync(GetUserAffiliatesInputExtend input)
        {
            return await _userAffiliateDomainService.ExportUserAffiliate(input);
        }
    }
}