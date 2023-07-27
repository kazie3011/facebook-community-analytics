using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Categories;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Exceptions;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Partners;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.Services.Emails;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.TeamMembers;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace FacebookCommunityAnalytics.Api.PartnerModule
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.PartnerModule.Default)]
    public class PartnerModuleAppService : ApiAppService, IPartnerModuleAppService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IPartnerRepository _partnerRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IdentityUserManager _userManager;
        private readonly IContractRepository _contractRepository;
        private readonly IRepository<OrganizationUnit, Guid> _organizationUnitRepository;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly ICampaignDomainService _campaignDomainService;
        private readonly IPostRepository _postRepository;
        private readonly IPostDomainService _postDomainService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStatDomainService _statDomainService;
        private readonly IdentityUserManager _identityUserManager;
        private readonly IUserInfoRepository _userInfoRepository;

        private readonly IGroupDomainService _groupDomainService;
        
        private readonly ITiktokRepository _tiktokRepository;
        public PartnerModuleAppService(
            IGroupRepository groupRepository,
            IPartnerRepository partnerRepository,
            ICampaignRepository campaignRepository,
            IRepository<AppUser, Guid> userRepository,
            IdentityUserManager userManager,
            IContractRepository contractRepository,
            IRepository<OrganizationUnit, Guid> organizationUnitRepository,
            IIdentityUserRepository identityUserRepository,
            ICampaignDomainService campaignDomainService,
            IPostRepository postRepository,
            IPostDomainService postDomainService,
            ICategoryRepository categoryRepository,
            IStatDomainService statDomainService,
            IdentityUserManager identityUserManager,
            IUserInfoRepository userInfoRepository,
            ITiktokRepository tiktokRepository,
            IGroupDomainService groupDomainService)
        {
            _groupRepository = groupRepository;
            _partnerRepository = partnerRepository;
            _campaignRepository = campaignRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _contractRepository = contractRepository;
            _organizationUnitRepository = organizationUnitRepository;
            _identityUserRepository = identityUserRepository;
            _campaignDomainService = campaignDomainService;
            _postRepository = postRepository;
            _postDomainService = postDomainService;
            _categoryRepository = categoryRepository;
            _statDomainService = statDomainService;
            _identityUserManager = identityUserManager;
            _userInfoRepository = userInfoRepository;
            _tiktokRepository = tiktokRepository;
            _groupDomainService = groupDomainService;
        }

        #region Group
        public async Task<PagedResultDto<GroupDto>> GetGroups(GetGroupsInput input)
        {
            var totalCount = await _groupRepository.GetCountAsync
            (
                input.FilterText,
                input.Title,
                input.Fid,
                input.Name,
                input.AltName,
                input.Url,
                input.IsActive,
                input.GroupSourceType,
                input.GroupOwnershipType,
                input.GroupCategoryType
            );

            var items = await _groupRepository.GetListAsync
            (
                input.FilterText,
                input.Title,
                input.Fid,
                input.Name,
                input.AltName,
                input.Url,
                input.IsActive,
                input.GroupSourceType,
                input.GroupOwnershipType,
                input.GroupCategoryType,
                input.ContractStatus,
                null,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );

            return new PagedResultDto<GroupDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Group>, List<GroupDto>>(items)
            };
        }

        #endregion

        #region LOOKUP

        public async Task<List<LookupDto<Guid?>>> GetUserLookup(GetMembersApiRequest input)
        {
            var organizationUnit = _organizationUnitRepository.AsQueryable()
                .WhereIf
                (
                    !input.TeamId.HasValue && input.TeamName.IsNotNullOrEmpty(),
                    o => o.DisplayName.ToLower().Contains(input.TeamName.ToLower())
                )
                .FirstOrDefault();
            if (organizationUnit is not null) input.TeamId = organizationUnit.Id;

            if (input.FilterText.IsNotNullOrEmpty())
            {
                var users = _userRepository.AsQueryable()
                    .Where
                    (
                        _ => _.UserName.ToLower().Trim().Contains(input.FilterText.ToLower().Trim())
                             || (_.Name.IsNotNullOrEmpty() && _.Name.ToLower().Trim().Contains(input.FilterText.ToLower().Trim()))
                             || (_.Surname.IsNotNullOrEmpty() && _.Surname.ToLower().Trim().Contains(input.FilterText.ToLower().Trim()))
                             || (_.Email.IsNotNullOrEmpty() && _.Email.ToLower().Trim().Contains(input.FilterText.ToLower().Trim()))
                    )
                    .ToList();

                return ObjectMapper.Map<List<AppUser>, List<LookupDto<Guid?>>>(users);
            }

            if (input.TeamId != null && input.TeamId != Guid.Empty)
            {
                var users = await _identityUserRepository.GetUsersInOrganizationUnitAsync(input.TeamId.Value);
                return ObjectMapper.Map<List<IdentityUser>, List<LookupDto<Guid?>>>(users);
            }
            else
            {
                var users = await _userRepository.GetListAsync();
                return ObjectMapper.Map<List<AppUser>, List<LookupDto<Guid?>>>(users);
            }
        }

        public async Task<List<LookupDto<Guid>>> GetPartnerUserLookup(LookupRequestDto input)
        {
            var userIds = (await _userManager.GetUsersInRoleAsync(RoleConsts.Partner)).Select(x => x.Id).ToList();
            var query = _userRepository.AsQueryable()
                .Where(x => userIds.Contains(x.Id))
                .WhereIf
                (
                    !string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null && x.Name.Contains(input.Filter)
                )
                .OrderBy(x => x.UserName);

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AppUser>();
            return ObjectMapper.Map<List<AppUser>, List<LookupDto<Guid>>>(lookupData);
        }

        public async Task<List<LookupDto<Guid?>>> GetCampaignLookup(GetCampaignLookupDto input)
        {
            var partnerIds = GetPartnerIds();
            if (input.PartnerId.HasValue)
            {
                partnerIds = new List<Guid>() {input.PartnerId.Value};
            }

            var query = _campaignRepository.AsQueryable()
                .Where(x => x.PartnerId.HasValue && partnerIds.Contains(x.PartnerId.Value))
                .WhereIf
                (
                    !string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null && x.Name.Contains(input.Filter)
                );
            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Campaign>();

            return ObjectMapper.Map<List<Campaign>, List<LookupDto<Guid?>>>(lookupData);
        }

        public async Task<List<LookupDto<Guid?>>> GetGroupLookup(GroupLookupRequestDto input)
        {
            var filterText = string.Empty;
            if (input.Filter.IsNotNullOrWhiteSpace()) { filterText = input.Filter.ToLower().RemoveDiacritics().Trim(); }

            var groups = _groupRepository.AsQueryable()
                .WhereIf(filterText.IsNotNullOrWhiteSpace(), g => g.Title != null && g.Title.ToLower().RemoveDiacritics().IndexOf(filterText, 0, StringComparison.CurrentCultureIgnoreCase) >= 0)
                .WhereIf(input.GroupSourceType.HasValue, x => x.GroupSourceType == input.GroupSourceType)
                .OrderBy(x => x.Title)
                .ToList();

            return ObjectMapper.Map<List<Group>, List<LookupDto<Guid?>>>(groups);
        }

        public async Task<List<LookupDto<Guid?>>> GetCategoryLookup(LookupRequestDto input)
        {
            var result = _categoryRepository.AsQueryable()
                .WhereIf
                (
                    input.Filter.IsNotNullOrWhiteSpace(),
                    u => u.Name.ToLower().RemoveDiacritics().Contains(input.Filter.ToLower().RemoveDiacritics())
                )
                .ToList();

            return ObjectMapper.Map<List<Category>, List<LookupDto<Guid?>>>(result);
        }

        public virtual async Task<List<LookupDto<Guid?>>> GetRunningCampaignLookup(LookupRequestDto input)
        {
            var filterText = string.Empty;
            if (input.Filter.IsNotNullOrWhiteSpace()) { filterText = input.Filter.ToLower().RemoveDiacritics().Trim(); }

            var result = _campaignRepository.AsQueryable()
                .Where(_ => _.Status == CampaignStatus.Started || _.Status == CampaignStatus.Hold)
                .WhereIf(filterText.IsNotNullOrWhiteSpace(), c => c.Name.ToLower().RemoveDiacritics().Trim().Contains(filterText))
                .WhereIf(input.CreatorId.HasValue, x => x.CreatorId == input.CreatorId)
                .ToList();

            return ObjectMapper.Map<List<Campaign>, List<LookupDto<Guid?>>>(result);
        }

        #endregion

        #region CONTRACT

        public async Task<bool> ContractExist(string contractCode)
        {
            if (contractCode.IsNullOrWhiteSpace()) return false;
            return await _contractRepository.FindAsync(x => x.ContractCode == contractCode) is not null;
        }

        public async Task<PagedResultDto<ContractWithNavigationPropertiesDto>> GetContractNavs(GetContractsInput input)
        {
            input.PartnerIds = _partnerRepository.AsQueryable().Where(x => x.PartnerUserIds != null && x.PartnerUserIds.Contains(CurrentUser.Id.Value)).Select(x => x.Id).ToList();
            if (input.PartnerIds.IsNullOrEmpty())
            {
                return new PagedResultDto<ContractWithNavigationPropertiesDto>();
            }

            var count = await _contractRepository.GetCountAsync
            (
                input.FilterText,
                input.CreatedAtMin,
                input.CreatedAtMax,
                input.SignedAtMin,
                input.SignedAtMax,
                input.ContractStatus,
                input.ContractPaymentStatus,
                input.SalePersonId,
                input.PartnerIds
            );

            var items = await _contractRepository.GetListWithNavigationPropertiesAsync
            (
                input.FilterText,
                input.CreatedAtMin,
                input.CreatedAtMax,
                input.SignedAtMin,
                input.SignedAtMax,
                input.ContractStatus,
                input.ContractPaymentStatus,
                input.SalePersonId,
                input.PartnerIds,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );

            return new PagedResultDto<ContractWithNavigationPropertiesDto>()
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<ContractWithNavigationProperties>, List<ContractWithNavigationPropertiesDto>>(items)
            };
        }

        public async Task<ContractDto> CreateContract(CreateUpdateContractDto input)
        {
            var contract = ObjectMapper.Map<CreateUpdateContractDto, Contract>(input);
            await _contractRepository.InsertAsync(contract, autoSave: true);
            return ObjectMapper.Map<Contract, ContractDto>(contract);
        }

        public async Task<ContractDto> EditContract(Guid id, CreateUpdateContractDto input)
        {
            var contract = await _contractRepository.GetAsync(id);
            ObjectMapper.Map(input, contract);
            contract = await _contractRepository.UpdateAsync(contract, autoSave: true);
            return ObjectMapper.Map<Contract, ContractDto>(contract);
        }

        public async Task DeleteContract(Guid id)
        {
            await _contractRepository.DeleteAsync(id);
        }

        #endregion

        #region CAMPAIGN

        public async Task<CampaignDto> GetCampaign(Guid id)
        {
            var campaignDto = ObjectMapper.Map<Campaign, CampaignDto>(await _campaignRepository.GetAsync(id));
            var contracts = _contractRepository.Where(x => x.CampaignId == id).ToList();
            if (contracts.IsNotNullOrEmpty())
            {
                campaignDto.Contracts = ObjectMapper.Map<List<Contract>, List<ContractDto>>(contracts);
            }

            return campaignDto;
        }

        public async Task<List<CampaignPostDto>> GetCampaignPosts(Guid campaignId)
        {
            var campaign = await _campaignRepository.GetAsync(x => x.Id == campaignId);

            var isInViewRole = IsSaleAndAboveRole();
            if (!isInViewRole && !IsPartnerRole())
            {
                if (campaign.Emails.IsNotNullOrEmpty() && !campaign.Emails.Contains(CurrentUser.Email)) { throw new ApiException(L[ApiDomainErrorCodes.Campaign.PermissionNotGranted]); }
            }

            var campaignPosts = await _campaignDomainService.GetPosts(campaignId);
            return ObjectMapper.Map<List<CampaignPost>, List<CampaignPostDto>>(campaignPosts);
        }

        public virtual async Task<PagedResultDto<CampaignWithNavigationPropertiesDto>> GetPartnerCampaigns(GetCampaignsInput input)
        {
            input.PartnerIds = _partnerRepository.AsQueryable().Where(x => x.PartnerUserIds != null && x.PartnerUserIds.Contains(CurrentUser.Id.Value)).Select(x => x.Id).ToList();
            if (input.PartnerIds.IsNullOrEmpty())
            {
                return new PagedResultDto<CampaignWithNavigationPropertiesDto>();
            }

            var totalCount = await _campaignRepository.GetCountExtendAsync
            (
                input.FilterText,
                input.CampaignType,
                input.Status,
                input.StartDateTimeMin,
                input.StartDateTimeMax,
                input.EndDateTimeMin,
                input.EndDateTimeMax,
                input.IsActive,
                input.PartnerId,
                input.CurrentUserEmail,
                input.PartnerIds
            );
            var items = await _campaignRepository.GetListWithNavigationPropertiesExtendAsync
            (
                input.FilterText,
                input.CampaignType,
                input.Status,
                input.StartDateTimeMin,
                input.StartDateTimeMax,
                input.EndDateTimeMin,
                input.EndDateTimeMax,
                input.IsActive,
                input.PartnerId,
                input.CurrentUserEmail,
                input.PartnerIds,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );

            return new PagedResultDto<CampaignWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<CampaignWithNavigationProperties>, List<CampaignWithNavigationPropertiesDto>>(items)
            };
        }

        public async Task<PagedResultDto<CampaignWithNavigationPropertiesDto>> GetCampaignNavs(GetCampaignsInput input)
        {
            input.PartnerIds = GetPartnerIds();
            var totalCount = await _campaignRepository.GetCountAsync
            (
                input.FilterText,
                input.Name,
                input.Code,
                input.Hashtags,
                input.Description,
                input.CampaignType,
                input.Status,
                input.StartDateTimeMin,
                input.StartDateTimeMax,
                input.EndDateTimeMin,
                input.EndDateTimeMax,
                input.IsActive,
                input.PartnerId,
                input.CurrentUserEmail,
                input.PartnerIds
            );
            var items = await _campaignRepository.GetListWithNavigationPropertiesAsync
            (
                input.FilterText,
                input.Name,
                input.Code,
                input.Hashtags,
                input.Description,
                input.CampaignType,
                input.Status,
                input.StartDateTimeMin,
                input.StartDateTimeMax,
                input.EndDateTimeMin,
                input.EndDateTimeMax,
                input.IsActive,
                input.PartnerId,
                input.CurrentUserEmail,
                input.PartnerIds,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );
            
            return new PagedResultDto<CampaignWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<CampaignWithNavigationProperties>, List<CampaignWithNavigationPropertiesDto>>(items)
            };
        }

        public async Task<List<CampaignDto>> GetCampaigns(GetCampaignsInput input)
        {
            input.PartnerIds = GetPartnerIds();

            if (input.PartnerIds.IsNullOrEmpty())
            {
                return new List<CampaignDto>();
            }

            var items = await _campaignRepository.GetListAsync
            (
                input.FilterText,
                input.Name,
                input.Code,
                input.Hashtags,
                input.Description,
                input.CampaignType,
                input.Status,
                input.StartDateTimeMin,
                input.StartDateTimeMax,
                input.EndDateTimeMin,
                input.EndDateTimeMax,
                input.IsActive,
                input.PartnerId,
                input.CurrentUserEmail,
                input.PartnerIds
            );

            return ObjectMapper.Map<List<Campaign>, List<CampaignDto>>(items);
        }

        private List<Guid> GetPartnerIds()
        {
            var partnerIds = new List<Guid>();
            var partnerRoot = _partnerRepository.AsQueryable().FirstOrDefault(x => (x.PartnerIds == null || x.PartnerIds.Count == 0) && x.PartnerUserIds.Contains(CurrentUser.Id.Value));
            if (partnerRoot != null)
            {
                partnerIds.Add(partnerRoot.Id);

                var subPartnerIds = _partnerRepository.Where(x => x.PartnerIds != null && x.PartnerIds.Contains(partnerRoot.Id)).Select(x => x.Id).ToList();
                partnerIds.AddRange(subPartnerIds);

                var otherPartnerIds = _partnerRepository.Where(x => x.PartnerUserIds != null && x.PartnerUserIds.Contains(CurrentUser.Id.Value)).Select(x => x.Id).ToList();
                partnerIds.AddRange(otherPartnerIds);
            }

            return partnerIds.Distinct().ToList();
        }

        public virtual async Task<CampaignDto> CreateCampaign(CampaignCreateDto input)
        {
            var campaign = ObjectMapper.Map<CampaignCreateDto, Campaign>(input);
            campaign.TenantId = CurrentTenant.Id;

            campaign.Status = _campaignDomainService.GetCampaignStatus(input.StartDateTime, input.EndDateTime);
            campaign = await _campaignRepository.InsertAsync(campaign);

            return ObjectMapper.Map<Campaign, CampaignDto>(campaign);
        }

        public virtual async Task<CampaignDto> EditCampaign(Guid id, CampaignUpdateDto input)
        {
            var campaign = await _campaignRepository.GetAsync(id);
            ObjectMapper.Map(input, campaign);

            campaign.Status = _campaignDomainService.GetCampaignStatus(input.StartDateTime, input.EndDateTime);
            campaign = await _campaignRepository.UpdateAsync(campaign);

            return ObjectMapper.Map<Campaign, CampaignDto>(campaign);
        }

        public virtual async Task DeleteCampaign(Guid id)
        {
            await _campaignRepository.DeleteAsync(id);
        }

        public Task SendCampaignEmail(Guid campaignId)
        {
            BackgroundJob.Enqueue<ICampaignEmailDomainService>(_ => _.Send(campaignId));
            return Task.CompletedTask;
        }

        public async Task<byte[]> ExportCampaign(Guid campaignId)
        {
            return await _campaignDomainService.GetCampaignExcelAsync(campaignId);
        }

        public async Task RemoveCampaignPost(Guid postId)
        {
            await _campaignDomainService.RemoveCampaignPost(postId);
        }

        public async Task CreateCampaignPosts(PostCreateDto input)
        {
            await _campaignDomainService.CreateCampaignPosts(input, CurrentUser.GetId(), GetPartnerUserIds());
        }

        public async Task<GrowthCampaignChartDto> GetGrowthCampaignChart(GetGrowthCampaignChartsInput input)
        {
            List<Campaign> campaigns;
            var partnerIds = GetPartnerIds();

            if (input.CampaignIds.IsNullOrEmpty())
            {
                campaigns = _campaignRepository.Where(x => x.PartnerId.HasValue && partnerIds.Contains(x.PartnerId.Value)).ToList();
                input.CampaignIds = campaigns.Select(x => x.Id).ToList();
            }
            else
            {
                campaigns = _campaignRepository.Where(x => input.CampaignIds.Contains(x.Id)).ToList();
            }

            return await _statDomainService.GetPartnerGrowthChartStats(input, campaigns, partnerIds);
        }

        public Task UpdateCampaignTiktok(TiktokCreateUpdateDto input, Guid id)
        {
            return _campaignDomainService.UpdateCampaignTiktok(input, id);
        }
        
        #endregion

        #region PARTNER

        public async Task<PagedResultDto<PartnerDto>> GetPartners(GetPartnersInput input)
        {
            input.PartnerUserId = CurrentUser.Id;

            var totalCount = await _partnerRepository.GetCountAsync
            (
                input.FilterText,
                input.Name,
                input.Description,
                input.Url,
                input.Code,
                input.PartnerType,
                input.IsActive,
                input.PartnerUserId
            );
            var items = await _partnerRepository.GetListAsync
            (
                input.FilterText,
                input.Name,
                input.Description,
                input.Url,
                input.Code,
                input.PartnerType,
                input.IsActive,
                input.PartnerUserId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );

            var partners = new List<Partner>();
            foreach (var id in items.Where(item => !item.PartnerIds.IsNullOrEmpty()).SelectMany(item => item.PartnerIds))
            {
                partners.Add(await _partnerRepository.GetAsync(id));
            }

            items.AddRange(partners);

            var partnerDtos = ObjectMapper.Map<List<Partner>, List<PartnerDto>>(items);
            foreach (var partnerDto in partnerDtos)
            {
                partnerDto.TotalCampaigns = await _campaignRepository.CountAsync(x => x.PartnerId == partnerDto.Id);
            }

            return new PagedResultDto<PartnerDto>
            {
                TotalCount = totalCount + partners.Count,
                Items = partnerDtos
            };
        }

        public async Task<PartnerDto> GetPartnerById(Guid id)
        {
            var partner = await _partnerRepository.GetAsync(id);
            return ObjectMapper.Map<Partner, PartnerDto>(partner);
        }

        public virtual async Task<List<LookupDto<Guid?>>> GetPartnersLookup(LookupRequestDto input)
        {
            var partnerIds = GetPartnerIds();
            var query = _partnerRepository.AsQueryable()
                .Where(x => partnerIds.Contains(x.Id));

            var lookupData = await query.ToDynamicListAsync<Partner>();

            return ObjectMapper.Map<List<Partner>, List<LookupDto<Guid?>>>(lookupData);
        }

        public async Task<PartnerDto> CreatePartner(PartnerCreateDto input)
        {
            var partnerEntity = await _partnerRepository.FirstOrDefaultAsync(x => x.Code == input.Code);
            if (partnerEntity != null)
            {
                throw new ApiException(LD["ApiDomain:PartnerExist"]);
            }

            if (CurrentUser.Id != null && input.PartnerUserIds.Contains(CurrentUser.Id.Value))
            {
                input.PartnerUserIds.Add(CurrentUser.Id.Value);
            }

            if (IsPartnerRole())
            {
                input.PartnerUserIds.Add(CurrentUser.GetId());
            }

            var partner = ObjectMapper.Map<PartnerCreateDto, Partner>(input);
            partner.TenantId = CurrentTenant.Id;
            partner = await _partnerRepository.InsertAsync(partner, autoSave: true);
            if (IsPartnerRole())
            {
                var partners = await _partnerRepository.GetListAsync(partnerUserId: CurrentUser.GetId());
                foreach (var item in partners)
                {
                    item.PartnerIds.Add(partner.Id);
                }
            }

            return ObjectMapper.Map<Partner, PartnerDto>(partner);
        }

        public async Task<PartnerDto> EditPartner(Guid id, PartnerUpdateDto input)
        {
            if (CurrentUser.Id != null && input.PartnerUserIds.Contains(CurrentUser.Id.Value))
            {
                input.PartnerUserIds.Add(CurrentUser.Id.Value);
            }

            var partner = await _partnerRepository.GetAsync(id);
            ObjectMapper.Map(input, partner);
            partner = await _partnerRepository.UpdateAsync(partner);
            return ObjectMapper.Map<Partner, PartnerDto>(partner);
        }

        public async Task DeletePartner(Guid id)
        {
            await _partnerRepository.DeleteAsync(id);
        }

        public async Task<List<PartnerDto>> GetPartnersByUser(Guid userId)
        {
            var partners = _partnerRepository.Where(x => x.PartnerUserIds.Contains(userId)).ToList();
            return ObjectMapper.Map<List<Partner>, List<PartnerDto>>(partners);
        }

        public async Task<List<TiktokWithNavigationPropertiesDto>> GetTikToks(GetTiktoksInputExtend input)
        {
            return ObjectMapper.Map<List<TiktokWithNavigationProperties>, List<TiktokWithNavigationPropertiesDto>>(await _campaignDomainService.GetTikToks(input));
        }
        public async Task<PagedResultDto<TiktokWithNavigationPropertiesDto>> GetTikToksPaging(GetTiktoksInputExtend input)
        {
            var groupIds = new List<Guid>();
            if (input.TikTokMcnType == TikTokMCNType.MCNGdl)
            {
                groupIds = (await _groupDomainService.GetMCNGDLChannels()).Select(x => x.Id).ToList();
            }
            var posts = await _tiktokRepository.GetListWithNavigationPropertiesAsync
            (
                input.Search,
                input.CreatedDateTimeMin,
                input.CreatedDateTimeMax,
                input.SendEmail,
                null,
                groupIds,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );
            
            var count = await _tiktokRepository.GetCountAsync
            (
                input.Search,
                input.CreatedDateTimeMin,
                input.CreatedDateTimeMax,
                input.SendEmail,
                null,
                groupIds
            );

            var items = ObjectMapper.Map<List<TiktokWithNavigationProperties>, List<TiktokWithNavigationPropertiesDto>>(posts);

            return new PagedResultDto<TiktokWithNavigationPropertiesDto>()
            {
                Items = items,
                TotalCount = count
            };
        }

        private List<Guid> GetPartnerUserIds()
        {
            var partners = _partnerRepository.AsQueryable().Where(x => x.PartnerUserIds != null && x.PartnerUserIds.Contains(CurrentUser.Id.Value)).ToList();
            var result = new List<Guid>();
            foreach (var partner in partners.Where(partner => partner.PartnerUserIds.IsNotNullOrEmpty()))
            {
                result.AddRange(partner.PartnerUserIds);
            }

            return result;
        }

        private async Task<Partner> GetAgencyCurrentUser()
        {
            var agency = await _partnerRepository.FirstOrDefaultAsync(x => x.PartnerIds != null && x.PartnerUserIds.Contains(CurrentUser.Id.Value));
            return agency;
        }

        public async Task<List<AppUserDto>> GetPartnerUsers()
        {
            if (!CurrentUser.IsAuthenticated || !CurrentUser.Id.HasValue)
            {
                return null;
            }

            var agency = await GetAgencyCurrentUser();
            var users = await _userRepository.GetListAsync(x => agency.PartnerUserIds.Contains(x.Id));
            return ObjectMapper.Map<List<AppUser>, List<AppUserDto>>(users.OrderBy(x => x.Email).ToList());
        }

        public async Task AddPartnerUser(string email, string name, string surname)
        {
            var userResult = await RegisterNewUser(email, name, surname);
            if (userResult != null)
            {
                var partnerIds = GetPartnerIds();
                var partners = _partnerRepository.Where(x => partnerIds.Contains(x.Id)).ToList();
                if (partners.IsNotNullOrEmpty())
                {
                    foreach (var partner in partners)
                    {
                        if (partner.PartnerUserIds.IsNotNullOrEmpty())
                        {
                            partner.PartnerUserIds.Add(userResult.Id);
                        }
                        else
                        {
                            partner.PartnerUserIds = new List<Guid>() {userResult.Id};
                        }
                    }

                    await _partnerRepository.UpdateManyAsync(partners);
                }
            }
        }

        public async Task<bool> UserExist(string email)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Email == email);
            return user != null;
        }

        private async Task<IdentityUser> RegisterNewUser(string email, string name, string surname)
        {
            var users = await _userRepository.ToListAsync();
            var existingUser = users.FirstOrDefault(_ => _.Email.ToLower().Equals(email) || _.UserName.ToLower().Equals(email));

            if (existingUser != null) { return null; }

            var identityUser = new IdentityUser(Guid.Empty, email, email)
            {
                Name = name,
                Surname = surname,
            };

            var password = PartnerConst.DefaultPartnerUserPassword;
            await _identityUserManager.CreateAsync(identityUser, password);
            if (identityUser.Id != Guid.Empty)
            {
                await _identityUserManager.SetRolesAsync
                (
                    identityUser,
                    new List<string>
                    {
                        RoleConsts.Partner,
                        RoleConsts.Guest
                    }
                );

                var currentUserInfoCode = await _userInfoRepository.GetCurrentUserCode();
                var newUserInfoCode = (currentUserInfoCode + 1).ToString();

                var newUserInfo = new UserInfo
                {
                    AppUserId = identityUser.Id,
                    Code = newUserInfoCode,
                    ContentRoleType = ContentRoleType.Unknown,
                    JoinedDateTime = DateTime.UtcNow,
                    PromotedDateTime = DateTime.UtcNow,
                    EnablePayrollCalculation = false,
                    IsActive = true,
                };
                await _userInfoRepository.InsertAsync(newUserInfo);
            }

            return identityUser;
        }

        public async Task RemovePartnerUser(Guid userId)
        {
            var agency = await GetAgencyCurrentUser();
            if (agency != null && agency.PartnerUserIds.Contains(userId))
            {
                agency.PartnerUserIds.Remove(userId);
                await _partnerRepository.UpdateAsync(agency, true);

                var user = await _identityUserManager.GetByIdAsync(userId);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
            }
        }

        #endregion

        #region GROUP
        
        public virtual async Task<GroupDto> CreateGroup(GroupCreateDto input)
        {
            if (input.Url.IsNullOrWhiteSpace()) throw new ApiException(LD[ApiDomainErrorCodes.Groups.NoUrl, input.Url]);
            input.Url = input.Url.Trim('/');
            input.GroupSourceType = FacebookHelper.GetGroupSourceTypeWithGroupUrl(input.Url);
            var existing = _groupRepository.FirstOrDefault
            (
                g =>
                    g.Fid == input.Fid.Trim() && g.GroupSourceType == input.GroupSourceType
            );
            if (existing != null) throw new ApiException(LD[ApiDomainErrorCodes.Groups.DuplicateGroup, existing.Name]);

            var group = ObjectMapper.Map<GroupCreateDto, Group>(input);
            group.TenantId = CurrentTenant.Id;
            group = await _groupRepository.InsertAsync(group, autoSave: true);
            return ObjectMapper.Map<Group, GroupDto>(group);
        }

        [Authorize(ApiPermissions.Groups.Edit)]
        public virtual async Task<GroupDto> UpdateGroupAsync(Guid id, GroupUpdateDto input)
        {
            var group = await _groupRepository.GetAsync(id);
            ObjectMapper.Map(input, group);
            group = await _groupRepository.UpdateAsync(group);
            return ObjectMapper.Map<Group, GroupDto>(group);
        }

        #endregion

        #region POST

        [Authorize(ApiPermissions.PartnerModule.PartnerPosts)]
        public async Task<PagedResultDto<PostWithNavigationPropertiesDto>> GetPostNavs(GetPostsInputExtend input)
        {
            var totalCount = await _postRepository.GetCountExtendAsync
            (
                input.FilterText,
                input.PostContentType,
                input.PostCopyrightType,
                input.Url,
                input.ShortUrl,
                input.LikeCountMin,
                input.LikeCountMax,
                input.CommentCountMin,
                input.CommentCountMax,
                input.ShareCountMin,
                input.ShareCountMax,
                input.TotalCountMin,
                input.TotalCountMax,
                input.Hashtag,
                input.Fid,
                input.IsNotAvailable,
                input.IsValid,
                input.Status,
                input.PostSourceType,
                input.Note,
                input.ClientOffsetInMinutes,
                input.CreatedDateTimeMin,
                input.CreatedDateTimeMax,
                input.LastCrawledDateTimeMin,
                input.LastCrawledDateTimeMax,
                input.SubmissionDateTimeMin,
                input.SubmissionDateTimeMax,
                input.CategoryId,
                input.GroupId,
                input.AppUserId,
                input.CampaignId,
                input.PartnerId,
                input.AppUserIds,
                input.GroupIds,
                input.CampaignIds,
                input.PostSourceTypes
            );
            var items = await _postRepository.GetListWithNavigationPropertiesExtendAsync
            (
                input.FilterText,
                input.PostContentType,
                input.PostCopyrightType,
                input.Url,
                input.ShortUrl,
                input.LikeCountMin,
                input.LikeCountMax,
                input.CommentCountMin,
                input.CommentCountMax,
                input.ShareCountMin,
                input.ShareCountMax,
                input.TotalCountMin,
                input.TotalCountMax,
                input.Hashtag,
                input.Fid,
                input.IsNotAvailable,
                input.IsValid,
                input.Status,
                input.PostSourceType,
                input.Note,
                input.ClientOffsetInMinutes,
                input.CreatedDateTimeMin,
                input.CreatedDateTimeMax,
                input.LastCrawledDateTimeMin,
                input.LastCrawledDateTimeMax,
                input.SubmissionDateTimeMin,
                input.SubmissionDateTimeMax,
                input.CategoryId,
                input.GroupId,
                input.AppUserId,
                input.CampaignId,
                input.PartnerId,
                input.AppUserIds,
                input.GroupIds,
                input.CampaignIds,
                input.PostSourceTypes,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );

            var result = ObjectMapper.Map<List<PostWithNavigationProperties>, List<PostWithNavigationPropertiesDto>>(items);

            return new PagedResultDto<PostWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = result
            };
        }

        [Authorize(ApiPermissions.PartnerModule.PartnerPosts)]
        public virtual async Task<PostDto> CreatePost(PostCreateDto input)
        {
            var post = ObjectMapper.Map<PostCreateDto, Post>(input);
            post.TenantId = CurrentTenant.Id;
            post = await _postRepository.InsertAsync(post, autoSave: true);
            return ObjectMapper.Map<Post, PostDto>(post);
        }

        [Authorize(ApiPermissions.PartnerModule.PartnerPosts)]
        public virtual async Task CreateMultiplePosts(PostCreateDto input)
        {
            var urls = input.Url.Split("\n").Where(_ => _.IsNotNullOrWhiteSpace()).Select(_ => _.Trim()).Distinct();
            var posts = new List<Post>();
            foreach (var url in urls.Where(FacebookHelper.IsValidGroupPostUrl))
            {
                var post = ObjectMapper.Map<PostCreateDto, Post>(input);
                post.Url = url;
                post = await _postDomainService.InitPostCreation(post, CurrentUser.GetId(), GetPartnerUserIds());
                if (post == null) continue;
                if (post.CampaignId != null)
                {
                    var campaign = await _campaignRepository.GetAsync((Guid) post.CampaignId);
                    post.PartnerId = campaign.PartnerId;
                }

                posts.Add(post);
            }

            if (posts.IsNotNullOrEmpty()) await _postRepository.InsertManyAsync(posts, autoSave: true);
        }

        [Authorize(ApiPermissions.PartnerModule.PartnerPosts)]
        public virtual async Task<PostDto> UpdatePost(Guid id, PostUpdateDto input)
        {
            var post = await _postRepository.GetAsync(id);
            ObjectMapper.Map(input, post);
            if (post.CampaignId.IsNotNullOrEmpty())
            {
                // ReSharper disable once PossibleInvalidOperationException
                var campaign = await _campaignRepository.GetAsync(post.CampaignId.Value);
                post.PartnerId = campaign.PartnerId;
            }
            else
            {
                post.CampaignId = null;
                post.PartnerId = null;
            }

            post = await _postRepository.UpdateAsync(post);
            return ObjectMapper.Map<Post, PostDto>(post);
        }

        [Authorize(ApiPermissions.PartnerModule.PartnerPosts)]
        public async Task DeletePost(Guid id)
        {
            await _postRepository.DeleteAsync(id);
        }

        #endregion

        #region CHARTS
        
        public async Task<PartnerCampaignChartDto> GetPartnerCampaignsChart()
        {
            var partnerIds = (await GetPartnersByUser(CurrentUser.GetId())).Select(x => x.Id).ToList();
            var result = new PartnerCampaignChartDto();

            var chartItems = new List<PartnerCampaignChartItem>();
            var campaigns = await  _campaignDomainService.GetRunningCampaigns(partnerIds);
            foreach (var g in campaigns.GroupBy(_ => _.PartnerId.Value))
            {
                var partner = await _partnerRepository.GetAsync(g.Key);
                if (partner != null)
                {
                    chartItems.Add
                    (
                        new PartnerCampaignChartItem
                        {
                            Label = partner.Name,
                            TotalCampaign = g.Count()
                        }
                    );
                }
            }

            if (chartItems.Count <= 6)
            {
                result.PartnerCampaignChartItems = chartItems;
            }
            else
            {
                result.PartnerCampaignChartItems = chartItems.OrderByDescending(x => x.TotalCampaign).Take(5).ToList();

                var others = chartItems.Where(x => !result.PartnerCampaignChartItems.Contains(x)).ToList();
                result.PartnerCampaignChartItems.Add
                (
                    new PartnerCampaignChartItem
                    {
                        Label = L["PartnerCampaignChart.Others"],
                        TotalCampaign = others.Sum(x => x.TotalCampaign)
                    }
                );
            }

            return result;
        }

        public async Task<PartnerPostTypeChartDto> GetPartnerPostContentTypeChart(DateTime startDate, DateTime endDate)
        {
            var partnerIds = GetPartnerIds();
            var result = new PartnerPostTypeChartDto();
            foreach (var postContentType in (PostContentType[]) Enum.GetValues(typeof(PostContentType)))
            {
                var countPost = await _postRepository.CountAsync(x =>
                    x.CreationTime >= startDate && x.CreationTime <= endDate 
                    && x.PostContentType == postContentType && x.PartnerId.HasValue && partnerIds.Contains(x.PartnerId.Value));
                
                if (countPost > 0)
                {
                    result.PartnerPostTypeChartItems.Add
                    (
                        new PartnerPostTypeChartItem()
                        {
                            Label = L[$"Enum:PostContentType:{postContentType.ToInt()}"],
                            TotalPost = countPost
                        }
                    );
                }
            }

            return result;
        }

        public async Task<GrowthCampaignChartDto> GetGrowthCampaignChartsAsync(GetGrowthCampaignChartsInput input)
        {
            List<Campaign> campaigns;
            var partnerIds = GetPartnerIds();
            if (input.PartnerId.HasValue)
            {
                partnerIds = new List<Guid>() { input.PartnerId.Value };
            }

            if (input.CampaignIds.IsNullOrEmpty())
            {
                campaigns = _campaignRepository.Where(x => x.PartnerId.HasValue && partnerIds.Contains(x.PartnerId.Value)).ToList();
                input.CampaignIds = campaigns.Select(x => x.Id).ToList();
            }
            else
            {
                campaigns = _campaignRepository.Where(x => input.CampaignIds.Contains(x.Id)).ToList();
            }

            return await _statDomainService.GetPartnerGrowthChartStats(input, campaigns, partnerIds);
        }

        [Authorize(ApiPermissions.PartnerModule.PartnerPosts)]
        public async Task<PostDto> GetPostAsync(Guid postId)
        {
            var postNav = await _postRepository.GetAsync(postId);
            return ObjectMapper.Map<Post, PostDto>(postNav);
        }
        
        [Authorize(ApiPermissions.PartnerModule.PartnerPosts)]
        public virtual async Task<PostDto> UpdateNotePost(Guid id, PostUpdateNoteDto input)
        {
            var post = await _postRepository.GetAsync(id);
            post.Note = input.Note;
            
            post = await _postRepository.UpdateAsync(post);
            
            return ObjectMapper.Map<Post, PostDto>(post);
        }

        #endregion
    }
}