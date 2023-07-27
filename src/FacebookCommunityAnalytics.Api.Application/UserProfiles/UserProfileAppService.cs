using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserCompensations;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace FacebookCommunityAnalytics.Api.UserProfiles
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.UserInfos.Default)]
    public class UserProfileAppService  : ApiAppService, IUserProfileAppService
    {
        private readonly IOrganizationDomainService _organizationDomainService;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IUserDomainService _userDomainService;
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IPostRepository _postRepository;
        private readonly IPostDomainService _postDomainService;
        private readonly IStaffEvaluationRepository _staffEvaluationRepository;
        private readonly IStaffEvaluationDomainService _staffEvaluationDomainService;
        private readonly IUserCompensationRepository _userCompensationRepository;
        private readonly IUserCompensationDomainService _userCompensationDomainService;
        private readonly IUserAffiliateRepository _userAffiliateRepository;
        private readonly IUserAffiliateDomainService _userAffiliateDomainService;
        private readonly IRepository<ContractTransaction, Guid> _contractTransactionRepository;
        private readonly ITiktokRepository _tiktokRepository;
        private readonly IGroupRepository _groupRepository;

        public UserProfileAppService(IContractRepository contractRepository,
            IUserAffiliateRepository userAffiliateRepository,
            IOrganizationDomainService organizationDomainService,
            IPostRepository postRepository,
            IPostDomainService postDomainService,
            IStaffEvaluationDomainService staffEvaluationDomainService,
            IUserCompensationRepository userCompensationRepository,
            IUserCompensationDomainService userCompensationDomainService,
            IUserAffiliateDomainService userAffiliateDomainService,
            IStaffEvaluationRepository staffEvaluationRepository,
            IUserInfoRepository userInfoRepository,
            IRepository<AppUser, Guid> userRepository,
            IUserDomainService userDomainService,
            IRepository<ContractTransaction, Guid> contractTransactionRepository,
            ITiktokRepository tiktokRepository,
            IGroupRepository groupRepository)
        {
            _contractRepository = contractRepository;
            _userAffiliateRepository = userAffiliateRepository;
            _organizationDomainService = organizationDomainService;
            _postRepository = postRepository;
            _postDomainService = postDomainService;
            _staffEvaluationDomainService = staffEvaluationDomainService;
            _userCompensationRepository = userCompensationRepository;
            _userCompensationDomainService = userCompensationDomainService;
            _userAffiliateDomainService = userAffiliateDomainService;
            _staffEvaluationRepository = staffEvaluationRepository;
            _userInfoRepository = userInfoRepository;
            _userRepository = userRepository;
            _userDomainService = userDomainService;
            _contractTransactionRepository = contractTransactionRepository;
            _tiktokRepository = tiktokRepository;
            _groupRepository = groupRepository;
        }

        public async Task<PagedResultDto<ContractWithNavigationPropertiesDto>> GetContractsPagedResult(GetContractsInput input)
        {
            input.MaxResultCount = int.MaxValue;
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
        
        public async Task<PagedResultDto<UserAffiliateWithNavigationPropertiesDto>> GetUserAffiliateWithNavigationProperties(GetUserAffiliatesInputExtend input)
        {
            input.MaxResultCount = int.MaxValue;
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

        public async Task<PagedResultDto<PostWithNavigationPropertiesDto>> GetPostsPagedResult(GetPostsInputExtend input)
        {
            input.MaxResultCount = int.MaxValue;
            if (IsManagerRole())
            {
            }
            else if (IsLeaderRole())
            {
                if (input.AppUserId != null)
                {
                    input.AppUserIds = await _userDomainService.GetManagedUserIds(CurrentUser.GetId());
                }
            }
            else if (IsStaffRole())
            {
                input.AppUserId = CurrentUser.GetId();
            }

            if (input.Url.IsNotNullOrWhiteSpace())
            {
                input.Url = FacebookHelper.GetCleanUrl(input.Url).ToString();
            }

            if (input.FilterText.IsNotNullOrWhiteSpace() && input.FilterText.Contains("http"))
            {
                input.FilterText = FacebookHelper.GetCleanUrlString(input.FilterText);
            }

            if (IsPartnerRole() && CurrentUser.Id.HasValue)
            {
                input.AppUserIds = new[] { CurrentUser.Id.Value };
            }

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
        
        public Task<List<PostDetailExportRow>> GetEvaluationPostDetailExportRow(GetStaffEvaluationsInput input)
        {
            return _postDomainService.GetEvaluationPostDetailExportRow(input);
        }
        
        public async Task<StaffEvaluationWithNavigationPropertiesDto> GetStaffEvaluation(Guid userId, int month, int year)
        {
            return await _staffEvaluationDomainService.GetStaffEvaluationByUser(userId, month, year);
        }

        public async Task<List<StaffEvaluationWithNavigationPropertiesDto>> GetStaffEvaluations(Guid userId,int fromYear, int fromMonth,int toYear,int toMonth)
        {
            return await _staffEvaluationDomainService.GetStaffEvaluationsByUser(userId, fromYear, fromMonth, toYear, toMonth);
        }
        public async Task<UserCompensationNavigationPropertiesDto> GetUserCompensationByUser(Guid userId, int month, int year)
        {
            var userCompensation = await _userCompensationRepository.GetWithNavigationPropertiesByUserAsync(userId, month, year);
            
            return ObjectMapper.Map<UserCompensationNavigationProperties, UserCompensationNavigationPropertiesDto>(userCompensation);
        }
        
        public async Task<List<CompensationAffiliateDto>> GetAffiliateConversions(DateTime fromDate, DateTime toDate, Guid userId)
        {
            return await _userCompensationDomainService.GetAffiliateConversions(fromDate, toDate, userId);
        }
        
        public async Task<UserProfileDto> GetUserProfileAsync(Guid userId)
        {
            var userProfileResult = new UserProfileDto();
            
            var request = new ApiUserDetailsRequest();
            request.UserIds.Add(userId);
            var userDetails = await _userDomainService.GetUserDetails(request);
            
            var userDetail = userDetails.FirstOrDefault();
            if (userDetail == null)
            {
                return new UserProfileDto();
            }
            if (userDetail.Info != null)
            {
                userProfileResult.UserInfo = ObjectMapper.Map<UserInfo, UserInfoDto>(userDetail.Info);
            }
            
            if (userDetail.User != null)
            {
                userProfileResult.AppUser = ObjectMapper.Map<AppUser, AppUserDto>(userDetail.User);
            }

            userProfileResult.Team = ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(userDetail.Team);
            return userProfileResult;
        }
        
        public async Task<PagedResultDto<LookupDto<Guid?>>> GetUserLookupAsync(LookupRequestDto input)
        {
            return await _userDomainService.GetUserLookupAsync(input);
        }

        public async Task<UserProfileChartResponse> GetChartStats(UserProfileChartRequest request)
        {
            var result = new UserProfileChartResponse();

            var team = await _organizationDomainService.GetTeam(request.TeamId);
            if (team == null) return result;

            if (GlobalConfiguration.TeamTypeMapping.Seeding.Contains(team.DisplayName)
                || GlobalConfiguration.TeamTypeMapping.Affiliate.Contains(team.DisplayName)
                || GlobalConfiguration.TeamTypeMapping.Content.Contains(team.DisplayName))
            {
                var posts = await _postRepository.GetListExtendAsync(appUserId: request.UserId, createdDateTimeMin: request.FromDateTime, createdDateTimeMax: request.ToDateTime);
                foreach (var date in request.FromDateTime.To(request.ToDateTime))
                {
                    foreach (var enumValue in Enum.GetValues(typeof(PostContentType)))
                    {
                        var typePost = (PostContentType)enumValue;
                        if (typePost == PostContentType.Unknown) continue;
                        var postsByType = posts.Where(_ => _.PostContentType == typePost && _.CreatedDateTime >= date && _.CreatedDateTime < date.Add(new TimeSpan(23, 59, 59))).ToList();
                        result.CountPostsByTypeChartData.Add
                        (
                            new DataChartItemDto<int, PostContentType>()
                            {
                                Display = date.ToString("dd/MM"),
                                Type = typePost,
                                Value = postsByType.Count()
                            }
                        );
                    }
                }
            }

            if (GlobalConfiguration.TeamTypeMapping.Sale.Contains(team.DisplayName))
            {
                var contracts = await _contractRepository.GetListExtendAsync(salePersonId: request.UserId);
                var transactions = await _contractTransactionRepository.GetListAsync();
                transactions = transactions.Where(_ => contracts.Select(c => c.Id).Contains(_.ContractId)).ToList();
                foreach (var date in request.FromDateTime.To(request.ToDateTime))
                {
                    result.SaleChartData.Add
                    (
                        new DataChartItemDto<decimal, string>()
                        {
                            Display = date.ToString("dd/MM"),
                            Type = "payment",
                            Value = transactions.Where(_ => _.PaymentDueDate >= date && _.PaymentDueDate < date.Add(new TimeSpan(23, 59, 59))).Sum(_ => _.PartialPaymentValue)
                        }
                    );
                }
            }

            if (GlobalConfiguration.TeamTypeMapping.Tiktok.Contains(team.DisplayName))
            {
                var tiktokVideos = await _tiktokRepository.GetListAsync(x => x.CreationTime >= request.FromDateTime && x.CreationTime < request.ToDateTime);
                var tiktokChannels = await _groupRepository.GetListAsync(groupSourceType: GroupSourceType.Tiktok, isActive: true);
                tiktokChannels = tiktokChannels.Where(_ => _.ModeratorIds.Contains(request.UserId)).ToList();

                foreach (var date in request.FromDateTime.To(request.ToDateTime))
                {
                    var tiktokCount = 0;
                    foreach (var channel in tiktokChannels)
                    {
                        tiktokCount += tiktokVideos.Count(_ => (_.ChannelId == channel.Name || _.ChannelId == channel.Fid) && _.CreatedDateTime >= date && _.CreatedDateTime < date.Add(new TimeSpan(23, 59, 59)));
                    }
                    result.TikTokChartData.Add
                    (
                        new DataChartItemDto<int, string>()
                        {
                            Display = date.ToString("dd/MM"),
                            Type = "tiktok video",
                            Value = tiktokCount
                        }
                    );
                }
            }
            return result;
        }
    }
}