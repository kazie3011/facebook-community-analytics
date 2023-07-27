using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Integrations.Shopiness;
using FacebookCommunityAnalytics.Api.Integrations.Shopiness.Models;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AffiliateConversions;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Exports;
using FacebookCommunityAnalytics.Api.Integrations.Tiki.Affiliates;
using FacebookCommunityAnalytics.Api.Integrations.Tiki.TikiAffiliates;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api.Services
{
    public interface IUserAffiliateDomainService : IDomainService
    {
        UserAffiliate InitUserAffiliateCreation(UserAffiliate input);

        Task InitUserAffiliates(DateTime? fromDateTime, DateTime? toDateTime);
        Task<UserAffDetailDto> GetUserAffDetails(UserAffDetailApiRequest apiRequest);
        Task<ShortlinkDto> GenerateShortlink(GenerateShortlinkApiRequest apiRequest);
        Task<UserAffSummaryApiResponse> GetUserAffiliateSummary(UserAffSummaryApiRequest request);
        Task<byte[]> ExportUserAffiliate(GetUserAffiliatesInputExtend input);
        Task<List<Guid>> UserIds(GetUserAffiliatesInputExtend input, Guid? userId);
    }

    public class UserAffiliateDomainService : BaseDomainService, IUserAffiliateDomainService
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IUserAffiliateRepository _userAffiliateRepository;
        private readonly IAffiliateConversionRepository _affiliateConversionRepository;

        private readonly IShopinessDomainService _shopinessDomainService;
        private readonly ITikiAffiliateDomainService _tikiAffiliateDomainService;

        private readonly IExportDomainService _exportDomainService;
        private readonly IOrganizationDomainService _organizationDomainService;
        private readonly IUserDomainService _userDomainService;
        private readonly IRepository<AppUser, Guid> _appUserRepository;

        public UserAffiliateDomainService(
            IShopinessDomainService shopinessDomainService,
            IUserAffiliateRepository userAffiliateRepository,
            IPostRepository postRepository,
            IExportDomainService exportDomainService,
            IOrganizationDomainService organizationDomainService,
            IUserDomainService userDomainService,
            IUserInfoRepository userInfoRepository,
            ITikiAffiliateDomainService tikiAffiliateDomainService,
            IAffiliateConversionRepository affiliateConversionRepository,
            IRepository<AppUser, Guid> appUserRepository)
        {
            _shopinessDomainService = shopinessDomainService;
            _userAffiliateRepository = userAffiliateRepository;
            _postRepository = postRepository;
            _exportDomainService = exportDomainService;
            _organizationDomainService = organizationDomainService;
            _userDomainService = userDomainService;
            _userInfoRepository = userInfoRepository;
            _tikiAffiliateDomainService = tikiAffiliateDomainService;
            _affiliateConversionRepository = affiliateConversionRepository;
            _appUserRepository = appUserRepository;
        }

        #region Init

        public async Task InitUserAffiliates(DateTime? fromDateTime, DateTime? toDateTime)
        {
            var postNavs = await _postRepository
                .GetListWithNavigationPropertiesExtendAsync
                (
                    submissionDateTimeMin: fromDateTime,
                    submissionDateTimeMax: toDateTime,
                    postContentType: PostContentType.Affiliate
                );
            if (postNavs.IsNullOrEmpty()) return;
            postNavs = postNavs.Where(p => p.Post.Shortlinks.IsNotNullOrEmpty() && p.AppUser != null).ToList();

            Console.WriteLine($"===========================================InitUserAffiliates for {postNavs.Count} posts");
            
            // 1. get the SHORTLINK OWNER
            ConcurrentBag<UserAffiliateModel> affiliateModels = new();

            Parallel.ForEach
            (
                postNavs,
                postNav =>
                {
                    if (postNav.Post.Shortlinks.IsNotNullOrEmpty())
                    {
                        var shortUrls = postNav.Post.Shortlinks;
                        foreach (var shortUrl in shortUrls)
                        {
                            var affiliateOwnershipType = AffiliateType(shortUrl);
                            var item = new UserAffiliateModel
                            {
                                Shortlink = shortUrl,
                                AffiliateOwnershipType = affiliateOwnershipType,
                                AppUserId = postNav.AppUser.Id,
                                CreatedAt = postNav.Post.SubmissionDateTime ?? DateTime.UtcNow,
                                GroupId = postNav.Group?.Id
                            };

                            if (affiliateModels.All(_ => _.Shortlink != shortUrl))
                            {
                                affiliateModels.Add(item); 
                                Console.WriteLine($"===========================================InitUserAffiliates - building shortlink model: {shortUrl}");
                            }
                        }
                    }
                }
            );
            Console.WriteLine($"===========================================InitUserAffiliates - FOUND for {affiliateModels.Count} shortlinks");

            var updateAffiliates = new List<UserAffiliate>();
            var userAffiliateModels = affiliateModels.DistinctBy(_ => _.Shortlink.Trim()).ToList();

            var newUserAffs = new List<UserAffiliate>();

            // 2. map the OWNER (APPUSER) to current affiliates
            foreach (var batch in userAffiliateModels.Partition(1000))
            {
                var currentPartition = batch.ToList();
                var shortUrls = currentPartition.Select(_ => _.Shortlink.Trim()).ToList();

                var existingAffiliates = await _userAffiliateRepository.Get(shortUrls);
                foreach (var userAff in currentPartition)
                {
                    var existing = existingAffiliates.FirstOrDefault(_ => _.AffiliateOwnershipType == userAff.AffiliateOwnershipType && UrlHelper.GetShortKey(_.AffiliateUrl) == UrlHelper.GetShortKey(userAff.Shortlink.Trim()));
                    if (existing == null)
                    {
                        var newAff = new UserAffiliate
                        {
                            AppUserId = userAff.AppUserId,
                            MarketplaceType = MarketplaceType.Shopee,
                            AffiliateUrl = userAff.Shortlink,
                            CreatedAt = userAff.CreatedAt,
                            GroupId = userAff.GroupId
                        };
                        newUserAffs.Add(InitUserAffiliateCreation(newAff));

                        Console.WriteLine($"===========================================InitUserAffiliates - NEW USER AFFILIATE: {userAff.Shortlink}");
                    }
                    else
                    {
                        existing.AppUserId = userAff.AppUserId;
                        existing.GroupId = userAff.GroupId;
                        updateAffiliates.Add(existing);
                        Console.WriteLine($"===========================================InitUserAffiliates - UPDATE USER AFFILIATE: {existing.AffiliateUrl}");
                    }
                }
            }
 
            if (updateAffiliates.IsNotNullOrEmpty())
            {
                foreach (var batch in updateAffiliates.Partition(100)) { await _userAffiliateRepository.UpdateManyAsync(batch); }
            }

            if (newUserAffs.IsNotNullOrEmpty())
            {
                foreach (var batch in newUserAffs.Partition(100)) { await _userAffiliateRepository.InsertManyAsync(batch); }
            }
        }

        private static AffiliateOwnershipType AffiliateType(string shortUrl)
        {
            var affiliateOwnershipType = AffiliateOwnershipType.Unknown;
            if (shortUrl.Contains(GlobalConsts.GDLDomain) || shortUrl.Contains(GlobalConsts.BaseAffiliateDomain))
            {
                affiliateOwnershipType = AffiliateOwnershipType.GDL;
            }
            else if (shortUrl.Contains(GlobalConsts.HPDDomain))
            {
                affiliateOwnershipType = AffiliateOwnershipType.HappyDay;
            }
            else if (shortUrl.Contains(GlobalConsts.YANDomain))
            {
                affiliateOwnershipType = AffiliateOwnershipType.YAN;
            }

            return affiliateOwnershipType;
        }

        public UserAffiliate InitUserAffiliateCreation(UserAffiliate input)
        {
            if (input.PartnerId == Guid.Empty) input.PartnerId = null;
            if (input.CampaignId == Guid.Empty) input.CampaignId = null;
            if (input.GroupId == Guid.Empty) input.GroupId = null;

            if (input.AffiliateUrl.Contains(GlobalConsts.BaseAffiliateDomain) || input.AffiliateUrl.Contains(GlobalConsts.GDLDomain)) { input.AffiliateOwnershipType = AffiliateOwnershipType.GDL; }
            else if (input.AffiliateUrl.Contains(GlobalConsts.HPDDomain)) { input.AffiliateOwnershipType = AffiliateOwnershipType.HappyDay; }
            else if (input.AffiliateUrl.Contains(GlobalConsts.YANDomain)) { input.AffiliateOwnershipType = AffiliateOwnershipType.YAN; }
            else { input.AffiliateOwnershipType = AffiliateOwnershipType.Unknown; }

            return input;
        }

        public async Task<ShortlinkDto> GenerateShortlink(GenerateShortlinkApiRequest apiRequest)
        {
            var shortlinkDto = new ShortlinkDto();

            switch (apiRequest.AffiliateProviderType)
            {
                case AffiliateProviderType.Unknown: break;

                case AffiliateProviderType.Shopiness:
                {
                    var trackingResp = await _shopinessDomainService.CreateTracking
                    (
                        new TrackingRequest
                        {
                            link = apiRequest.Link,
                            subId1 = apiRequest.UserCode,
                            subId2 = apiRequest.CommunityFid.IsNullOrEmpty() ? string.Empty : apiRequest.CommunityFid,
                            subId3 = apiRequest.CampaignId.IsNullOrEmpty() ? string.Empty : apiRequest.CampaignId,
                            IsGdl = !apiRequest.IsHappyDay
                        }
                    );
                    if (trackingResp != null)
                    {
                        shortlinkDto.Link = apiRequest.Link;
                        shortlinkDto.Shortlink = trackingResp.CustomLink;
                    }

                    break;
                }

                case AffiliateProviderType.Tiki:
                {
                    var response = await _tikiAffiliateDomainService.GenerateShortlink
                    (
                        new TikiAffGetShortlink.Request
                        {
                            CommunityFid = apiRequest.CommunityFid,
                            PartnerId = apiRequest.PartnerId,
                            CampaignId = apiRequest.CampaignId,
                            UserCode = apiRequest.UserCode,
                            Link = apiRequest.Link,
                            Shortlink = apiRequest.Shortlink
                        }
                    );
                    if (response.success)
                    {
                        shortlinkDto.Link = response.Link;
                        shortlinkDto.Shortlink = response.Shortlink;
                    }

                    break;
                }

                default: throw new ArgumentOutOfRangeException();
            }

            return shortlinkDto;
        }

        #endregion

        #region GET

        public async Task<UserAffDetailDto> GetUserAffDetails(UserAffDetailApiRequest apiRequest)
        {
            var userinfo = await _userInfoRepository.FindAsync(_ => _.Code == apiRequest.UserCode);
            if (userinfo != null)
            {
                // var dateTimeRange = _apiConfigurationDomainService.GetDefaultPayrollDateTime();
                // var fromDateTime = dateTimeRange.Key;
                // var toDateTime = dateTimeRange.Value;

                var posts = await _postRepository.GetListExtendAsync
                (
                    createdDateTimeMin: apiRequest.FromDateTime,
                    createdDateTimeMax: apiRequest.ToDateTime,
                    appUserId: userinfo.AppUserId,
                    postContentType: PostContentType.Affiliate
                );

                var affs = await _userAffiliateRepository.GetListAsync
                (
                    appUserId: userinfo.AppUserId,
                    createdAtMin: apiRequest.FromDateTime,
                    createdAtMax: apiRequest.ToDateTime
                );

                var shortUrlsFromPosts = posts.Where(_ => _.Shortlinks.IsNotNullOrEmpty()).SelectMany(_ => _.Shortlinks).Distinct().ToList();
                var shortUrlsFromShopiness = affs.Select(_ => _.AffiliateUrl).Distinct().ToList();
                return new UserAffDetailDto
                {
                    UserCode = userinfo.Code,
                    PostCount = posts.Count,
                    Urls = posts.Select(_ => _.Url).ToList(),
                    ShortUrlsFromPosts = shortUrlsFromPosts,
                    ShortUrlsFromPostsCount = shortUrlsFromPosts.Count,
                    ShortUrlsFromShopiness = shortUrlsFromShopiness,
                    ShortUrlsFromShopinessCount = shortUrlsFromShopiness.Count,
                    ClickCount = affs.Sum(_ => _.AffConversionModel.ClickCount),
                    ConversionCount = affs.Sum(_ => _.AffConversionModel.ConversionCount)
                };
            }

            return new UserAffDetailDto();
        }

        public async Task<UserAffSummaryApiResponse> GetUserAffiliateSummary(UserAffSummaryApiRequest request)
        {
            var response = new UserAffSummaryApiResponse();
            var dateTimeMin = request.FromDateTime?.Ticks ?? 0;
            var dateTimeMax = request.ToDateTime?.Ticks ?? 0;
            var affConversions = await _affiliateConversionRepository.GetListExtendAsync(dateTimeMin, dateTimeMax);
            var orgUnits = await _organizationDomainService.GetTeams(new GetChildOrganizationUnitRequest { IsGDLNode = true });
            var userDetails = await _userDomainService.GetUserDetails(new ApiUserDetailsRequest()
            {
                GetSystemUsers = false,
                GetActiveUsers = true,
            });

            var userAffs = await _userAffiliateRepository.GetListAsync();

            foreach (var g in userAffs.GroupBy(_ => _.AppUserId))
            {
                var userDetail = userDetails.FirstOrDefault(_ => _.User.Id == g.Key);
                if (userDetail == null) continue;
                
                Debug.WriteLine($"----------------{userDetail.User.UserName}------------");
                var userOrg = userDetail.Identity.OrganizationUnits.FirstOrDefault();
                var org = orgUnits.FirstOrDefault(_ => _.Id == userOrg?.OrganizationUnitId);

                var shortKeys = g.Select(s => UrlHelper.GetShortKey(s.AffiliateUrl)).ToList();
                var conversions = affConversions.Where(_ =>  _.ShortKey.IsIn(shortKeys)).ToList();
                var i = new UserAffSummaryApiResponse.Item
                {
                    UserCode = userDetail.Info.Code,
                    UserDisplayName = $"{userDetail.User.UserName} ({userDetail.Info.Code})",
                    Team = org?.DisplayName,
                    //Click = g.Sum(x => x.UserAffiliateConversion.ClickCount),
                    Conversion = conversions.Count,
                    Amount = conversions.Sum(x => x.SaleAmount),
                    Commission = conversions.Sum(x => x.Payout + x.PayoutBonus)
                };
                
                response.Items.Add(i);
            }

            // response.Items = response.Items.OrderByDescending(_ => _.Amount).Take(100).ToList();
            response.Items = response.Items.Where(x => x.Conversion > 0).OrderByDescending(_ => _.Conversion).ToList();
            //response.Click = response.Items.Sum(_ => _.Click);
            response.Conversion = response.Items.Sum(_ => _.Conversion);
            response.Amount = response.Items.Sum(_ => _.Amount);
            response.Commission = response.Items.Sum(_ => _.Commission);

            return response;
        }

        #endregion

        #region UPDATE + EXPORT

        public async Task<byte[]> ExportUserAffiliate(GetUserAffiliatesInputExtend input)
        {
            var affiliates = await _userAffiliateRepository.GetUserAffiliateWithNavigationProperties
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
                input.AppUserId
            );

            return _exportDomainService.GenerateUserAffiliateExcelBytes(affiliates, "Short Link");
        }

        #endregion
        public async Task<List<Guid>> UserIds(GetUserAffiliatesInputExtend input, Guid? userId)
        {
            List<Guid> userIds = null;
            Guid? currentUserId = null;
            switch (input.ConversionOwnerFilter)
            {
                case ConversionOwnerFilter.Own:
                    input.OrgUnitId = null;
                    break;
                case ConversionOwnerFilter.NotOwn:
                    currentUserId = userId;
                    var allUsers = await _appUserRepository.GetListAsync();
                    userIds = allUsers.Select(_ => _.Id).ToList();
                    break;
                case ConversionOwnerFilter.NoSelect:
                    break;
            }

            if (input.OrgUnitId.HasValue)
            {
                var users = await _userDomainService.GetTeamMembers(input.OrgUnitId.Value);
                userIds = users.Select(_ => _.Id).ToList();
            }

            if (currentUserId != null) { userIds = userIds.Where(_ => _ != currentUserId).ToList(); }

            return userIds;
        }

        public class UserAffiliateModel
        {
            public string Shortlink { get; set; }
            public AffiliateOwnershipType AffiliateOwnershipType { get; set; }
            public DateTime? CreatedAt { get; set; }
            public Guid AppUserId { get; set; }
            public Guid? GroupId { get; set; }
        }
    }
}