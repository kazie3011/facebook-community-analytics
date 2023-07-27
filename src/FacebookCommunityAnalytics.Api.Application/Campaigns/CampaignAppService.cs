using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.Partners;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Services.Emails;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Exceptions;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.Tiktoks;
using Hangfire;
using IdentityServer4.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Volo.Abp.Users;
using Contract = FacebookCommunityAnalytics.Api.Contracts.Contract;

namespace FacebookCommunityAnalytics.Api.Campaigns
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.Campaigns.Default)]
    public class CampaignsAppService : ApiAppService, ICampaignsAppService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IRepository<Partner, Guid> _partnerRepository;
        private readonly ICampaignDomainService _campaignDomainService;
        private readonly IRepository<Contract, Guid> _contractRepository;
        private readonly IStatDomainService _statDomainService;

        public CampaignsAppService(
            ICampaignRepository campaignRepository,
            IRepository<Partner, Guid> partnerRepository,
            ICampaignDomainService campaignDomainService,
            IRepository<Contract, Guid> contractRepository,
            IStatDomainService statDomainService)
        {
            _campaignRepository = campaignRepository;
            _partnerRepository = partnerRepository;
            _campaignDomainService = campaignDomainService;
            _contractRepository = contractRepository;
            _statDomainService = statDomainService;
        }

        public virtual async Task<PagedResultDto<CampaignWithNavigationPropertiesDto>> GetListAsync(GetCampaignsInput input)
        {
            if (CurrentUser is {Id: { }})
            {
                input.CurrentUserEmail = IsCampAndAboveRole() ? null : CurrentUser?.Email;
            }

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
                input.CurrentUserEmail
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
                partnerIds: null,
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

        public virtual async Task<CampaignWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<CampaignWithNavigationProperties, CampaignWithNavigationPropertiesDto>
                (await _campaignRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<CampaignDto> GetByIdOrCode(string idOrCode)
        {
            var campaign = await _campaignDomainService.GetByIdOrCode(idOrCode);
            if (campaign is null) return null;

            var campaignDto = ObjectMapper.Map<Campaign, CampaignDto>(campaign);
            var contracts = _contractRepository.Where(x => x.CampaignId == campaign.Id).ToList();
            if (contracts.IsNotNullOrEmpty())
            {
                campaignDto.Contracts = ObjectMapper.Map<List<Contract>, List<ContractDto>>(contracts);
            }

            return campaignDto;
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetPartnerLookupAsync(LookupRequestDto input)
        {
            var query = _partnerRepository.AsQueryable()
                .WhereIf
                (
                    !string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null && x.Name.Contains(input.Filter)
                );

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Partner>();
            var totalCount = query.Count();

            return new PagedResultDto<LookupDto<Guid?>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Partner>, List<LookupDto<Guid?>>>(lookupData)
            };
        }

        public async Task<List<CampaignPostDto>> GetPosts(Guid campaignId)
        {
            var campaign = await _campaignRepository.GetAsync(x => x.Id == campaignId);

            var isInViewRole = IsCampAndAboveRole();
            if (!isInViewRole && !IsPartnerRole())
            {
                if (campaign.Emails.IsNullOrEmpty() || !campaign.Emails.Contains(CurrentUser.Email)) { throw new ApiException(L[ApiDomainErrorCodes.Campaign.PermissionNotGranted]); }
            }

            var campaignPosts = await _campaignDomainService.GetPosts(campaignId);
            await _campaignDomainService.UpdateCampaignPostCount(campaignId);
            return ObjectMapper.Map<List<CampaignPost>, List<CampaignPostDto>>(campaignPosts);
        }

        public async Task<List<UserAffiliateWithNavigationPropertiesDto>> GetAffiliatesAsync(List<string> shortLinks)
        {
            return await _campaignDomainService.GetAffiliatesAsync(shortLinks);
        }

        [Authorize(ApiPermissions.Campaigns.Create)]
        public virtual async Task<CampaignDto> CreateAsync(CampaignCreateDto input)
        {
            var existing = await _campaignDomainService.GetByName(input.Name);
            if (existing is not null)
            {
                throw new ApiException((L[ApiDomainErrorCodes.Campaign.NameExisted]));
            }

            var campaign = ObjectMapper.Map<CampaignCreateDto, Campaign>(input);
            campaign.TenantId = CurrentTenant.Id;

            if (input.Emails.IsNotNullOrWhiteSpace())
            {
                var emails = input.Emails.SplitEmails().Select(x => x.ToLower()).Where(StringHelper.IsValidEmail).ToList();
                campaign.Emails = string.Join(',', emails);
            }

            if (input.Keywords.IsNotNullOrWhiteSpace())
            {
                var keywords = input.Keywords.SplitKeywords().Select(x => x.ToLower()).ToList();
                campaign.Keywords = string.Join(',', keywords);
            }

            if (input.Hashtags.IsNotNullOrWhiteSpace())
            {
                var hashtags = input.Hashtags.SplitHashtags().Select(x => x.ToLower()).ToList();
                campaign.Hashtags = string.Join(',', hashtags);
            }

            campaign.Status = _campaignDomainService.GetCampaignStatus(input.StartDateTime, input.EndDateTime);
            campaign.Code = campaign.Code?.ToLower().Trim();
            campaign = await _campaignRepository.InsertAsync(campaign, autoSave: true);

            if (campaign.Emails.IsNotNullOrWhiteSpace())
            {
                var emails = campaign.Emails.SplitEmails().Where(StringHelper.IsValidEmail).Distinct().ToList();
                foreach (var email in emails)
                {
                    var password = await _campaignDomainService.RegisterCampaignUsers(email, email, input.Code, campaign);
                    BackgroundJob.Enqueue<ICampaignEmailDomainService>(_ => _.SendCampaignWelcomeEmail(email, password, campaign.Id.ToString(), campaign));
                }
            }

            return ObjectMapper.Map<Campaign, CampaignDto>(campaign);
        }

        [Authorize(ApiPermissions.Campaigns.Edit)]
        public virtual async Task<CampaignDto> UpdateAsync(Guid id, CampaignUpdateDto input)
        {
            var existing = await _campaignDomainService.GetByName(input.Name);
            if (existing is not null && existing.Id != id)
            {
                throw new ApiException((L[ApiDomainErrorCodes.Campaign.NameExisted]));
            }
            
            var campaign = await _campaignRepository.GetAsync(id);
            var oldEmails = campaign.Emails?.SplitEmails() ?? new List<string>();

            ObjectMapper.Map(input, campaign);

            if (input.Emails.IsNotNullOrWhiteSpace())
            {
                var emails = input.Emails.SplitEmails().Select(x => x.ToLower()).Where(StringHelper.IsValidEmail).ToList();
                campaign.Emails = string.Join(',', emails);
            }

            if (input.Keywords.IsNotNullOrWhiteSpace())
            {
                var keywords = input.Keywords.SplitKeywords().Select(x => x.ToLower()).ToList();
                campaign.Keywords = string.Join(',', keywords);
            }

            if (input.Hashtags.IsNotNullOrWhiteSpace())
            {
                var hashtags = input.Hashtags.SplitHashtags().Select(x => x.ToLower()).ToList();
                campaign.Hashtags = string.Join(',', hashtags);
            }

            campaign.Status = _campaignDomainService.GetCampaignStatus(input.StartDateTime, input.EndDateTime);
            campaign.Code = campaign.Code?.ToLower().Trim();
            campaign = await _campaignRepository.UpdateAsync(campaign);

            if (campaign.Emails.IsNotNullOrEmpty())
            {
                var currentEmails = campaign.Emails.SplitEmails();
                foreach (var email in currentEmails.Where(x => !oldEmails.Contains(x)))
                {
                    var password = await _campaignDomainService.RegisterCampaignUsers(email, email, input.Code, campaign);
                    BackgroundJob.Enqueue<ICampaignEmailDomainService>(_ => _.SendCampaignWelcomeEmail(email, password, campaign.Id.ToString(), campaign));
                }
            }

            return ObjectMapper.Map<Campaign, CampaignDto>(campaign);
        }

        [Authorize]
        public async Task UpdateCampaignPrizes(Guid id, CampaignUpdateDto input)
        {
            var campaign = await _campaignRepository.GetAsync(id);
            ObjectMapper.Map(input, campaign);
            await _campaignRepository.UpdateAsync(campaign);
        }

        [Authorize(ApiPermissions.Campaigns.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _campaignRepository.DeleteAsync(id);
        }

        // TODOO Set role, DO NOT USE AUTHORIZE
        [Authorize]
        public async Task<byte[]> ExportCampaign(Guid campaignId)
        {
            return await _campaignDomainService.GetCampaignExcelAsync(campaignId);
        }

        public async Task<List<TiktokWithNavigationPropertiesDto>> GetTikToks(GetTiktoksInputExtend input)
        {
            return ObjectMapper.Map<List<TiktokWithNavigationProperties>, List<TiktokWithNavigationPropertiesDto>>(await _campaignDomainService.GetTikToks(input));
        }
   

        public Task UpdateCampaignTiktok(TiktokCreateUpdateDto input, Guid id)
        {
            return _campaignDomainService.UpdateCampaignTiktok(input, id);
        }

        public Task<List<CampaignDto>> GetCampsByTime(DateTime @from, DateTime to)
        {
            return _campaignDomainService.GetCampsByTime(from, to);
        }

        public Task<PieChartDataSource<double>> GetPostCountGroupsChart(DateTime fromDate, DateTime toDate)
        {
            return _campaignDomainService.GetPostCountGroupsChart(fromDate, toDate);
        }

        public Task<PieChartDataSource<double>> GetReactionGroupsChart(DateTime fromDate, DateTime toDate)
        {
            return _campaignDomainService.GetReactionGroupsChart(fromDate, toDate);
        }

        public Task<List<AuthorStatistic>> GetAuthorStatistic()
        {
            return _campaignDomainService.GetAuthorStatistic();
        }
        

        [Authorize]
        public Task SendCampaignEmail(Guid campaignId)
        {
            Hangfire.BackgroundJob.Enqueue<ICampaignEmailDomainService>(_ => _.Send(campaignId));
            return Task.CompletedTask;
        }

        [Authorize]
        public async Task RemoveCampaignPost(Guid postId)
        {
            await _campaignDomainService.RemoveCampaignPost(postId);
        }

        [Authorize]
        public async Task CreateCampaignPosts(PostCreateDto input)
        {
            await _campaignDomainService.CreateCampaignPosts(input, CurrentUser.GetId(), GetPartnerUserIds());
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
        
        public async Task<CampaignDailyChartResponse> GetCampaignDailyChartStats(Guid campaignId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            return await _statDomainService.GetCampaignDailyChartStats(campaignId,fromDate, toDate);
        }
    }
}