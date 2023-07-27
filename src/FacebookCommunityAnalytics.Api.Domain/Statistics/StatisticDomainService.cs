using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Principal;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Categories;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Integrations.Tiktok;
using FacebookCommunityAnalytics.Api.Organizations;
using FacebookCommunityAnalytics.Api.PartnerModule;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;
using FluentValidation.Results;
using Flurl.Util;
using IdentityServer4.Extensions;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api.Statistics
{
    public interface IStatDomainService : IDomainService
    {
        Task<GeneralStatsResponse> GetGeneralStats(GeneralStatsApiRequest apiRequest);
        Task<GrowthChartDto> GetGrowthChartStats(GeneralStatsApiRequest input);
        Task<GrowthCampaignChartDto> GetPartnerGrowthChartStats(GetGrowthCampaignChartsInput input, List<Campaign> campaigns, List<Guid> partnerIds);
        Task<TiktokGrowthChartDto> GetTiktokChartStats(GeneralStatsApiRequest input);
        Task<MultipleDataSourceChart<double>> GetMCNGDLTikTokChartStats(GeneralStatsApiRequest input);
        Task<MultipleDataSourceChart<long>> GetMCNVietNamTikTokChartStats(GeneralStatsApiRequest input);
        Task<CampaignDailyChartResponse> GetCampaignDailyChartStats(Guid campaignId, DateTimeOffset fromDate, DateTimeOffset toDate);
    }

    public class StatDomainService : BaseDomainService, IStatDomainService
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserAffiliateRepository _userAffiliateRepository;
        private readonly IUserDomainService _userDomainService;
        private readonly IOrganizationDomainService _organizationDomainService;
        private readonly IRepository<GroupStatsHistory, Guid> _groupStatsHistoryRepository;
        private readonly IRepository<Campaign, Guid> _campaignRepository;
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IRepository<TiktokStat, Guid> _tiktokStatRepository;
        private readonly IRepository<TikTokMCN, Guid> _tikTokMcnRepository;
        private readonly ITiktokRepository _tiktokRepository;

        public StatDomainService(
            IPostRepository postRepository,
            IUserDomainService userDomainService,
            IUserAffiliateRepository userAffiliateRepository,
            IOrganizationDomainService organizationDomainService,
            IRepository<GroupStatsHistory, Guid> groupStatsHistoryRepository,
            IRepository<Campaign, Guid> campaignRepository,
            IGroupRepository groupRepository,
            ICategoryRepository categoryRepository,
            IRepository<AppUser, Guid> userRepository,
            IUserInfoRepository userInfoRepository,
            IRepository<TiktokStat, Guid> tiktokStatRepository,
            IRepository<TikTokMCN, Guid> tikTokMcnRepository,
            ITiktokRepository tiktokRepository)
        {
            _postRepository = postRepository;
            _userDomainService = userDomainService;
            _userAffiliateRepository = userAffiliateRepository;
            _organizationDomainService = organizationDomainService;
            _groupStatsHistoryRepository = groupStatsHistoryRepository;
            _campaignRepository = campaignRepository;
            _groupRepository = groupRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _userInfoRepository = userInfoRepository;
            _tiktokStatRepository = tiktokStatRepository;
            _tikTokMcnRepository = tikTokMcnRepository;
            _tiktokRepository = tiktokRepository;
        }

        public async Task<GeneralStatsResponse> GetGeneralStats(GeneralStatsApiRequest apiRequest)
        {
            var allPosts = await _postRepository.GetListAsync(x => x.CreatedDateTime >= apiRequest.FromDateTime && x.CreatedDateTime <= apiRequest.ToDateTime);
            var categories = await _categoryRepository.GetListAsync();
            var groups = await _groupRepository.GetListAsync();
            var users = await _userRepository.GetListAsync();
            var userInfos = await _userInfoRepository.GetListAsync();
            var campaigns = await _campaignRepository.GetListAsync();
            var staffDetails = await _userDomainService.GetUserDetails
            (
                new ApiUserDetailsRequest()
                {
                    GetForPayrollCalculation = true,
                    GetTeamUsers = true,
                    GetSystemUsers = false,
                    GetActiveUsers = true,
                }
            );
            var organizationUnitDtos = (await _organizationDomainService.GetTeams(new GetChildOrganizationUnitRequest())).OrderBy(x => x.DisplayName).Distinct().ToList();

            var postsWithNavs = new List<PostWithNavigationProperties>();
            var posts = new List<Post>();

            foreach (var post in allPosts)
            {
                var group = groups.FirstOrDefault(x => x.Id == post.GroupId);

                if (@group?.GroupOwnershipType != GroupOwnershipType.GDLInternal)
                {
                    continue;
                }

                postsWithNavs.Add
                (
                    new PostWithNavigationProperties
                    {
                        Post = post,
                        Category = categories.FirstOrDefault(x => x.Id == post.CategoryId),
                        Group = @group,
                        AppUser = users.FirstOrDefault(x => x.Id == post.AppUserId),
                        AppUserInfo = userInfos.FirstOrDefault(x => x.AppUserId == post.AppUserId),
                        // Campaign
                        // Partner
                    }
                );

                posts.Add(post);
            }

            var result = new GeneralStatsResponse
            {
                FromDateTime = apiRequest.FromDateTime ?? DateTime.Now,
                ToDateTime = apiRequest.ToDateTime ?? DateTime.Now
            };
            result.TopReactionPosts = postsWithNavs
                .OrderByDescending(x => x.Post.TotalCount)
                .Take(10)
                .Select
                (
                    x =>
                    {
                        var groupName = string.Empty;
                        if (x.Group != null)
                        {
                            groupName = x.Group.Title.IsNotNullOrEmpty() ? x.Group.Title : x.Group.Fid;
                        }

                        var rs = new PostStatisticDto
                        {
                            Fid = x.Post.Fid,
                            Url = x.Post.Url,
                            AuthorName = (x.AppUser != null && x.AppUserInfo != null) ? $"{x.AppUser?.UserName}({x.AppUserInfo?.Code})" : string.Empty,
                            CommentCount = x.Post.CommentCount,
                            LikeCount = x.Post.LikeCount,
                            ShareCount = x.Post.ShareCount,
                            TotalCount = x.Post.TotalCount,
                            PostContentType = x.Post.PostContentType,
                            GroupName = groupName
                        };
                        return rs;
                    }
                )
                .ToList();

            result.Total_Post = posts.Count;
            result.Total_User = posts.Where(x => x.AppUserId.HasValue).DistinctBy(x => x.AppUserId).Count();

            result.TopReactionAffiliatePosts = postsWithNavs.Where(_ => _.Post.PostContentType == PostContentType.Affiliate)
                .OrderByDescending(x => x.Post.TotalCount)
                .Take(10)
                .Select
                (
                    x =>
                    {
                        var rs = new PostStatisticDto
                        {
                            Fid = x.Post.Fid,
                            Url = x.Post.Url,
                            AuthorName = (x.AppUser != null && x.AppUserInfo != null) ? $"{x.AppUser?.UserName}({x.AppUserInfo?.Code})" : string.Empty,
                            CommentCount = x.Post.CommentCount,
                            LikeCount = x.Post.LikeCount,
                            ShareCount = x.Post.ShareCount,
                            TotalCount = x.Post.TotalCount,
                            PostContentType = x.Post.PostContentType,
                            GroupName = x.Group.Name
                        };
                        return rs;
                    }
                )
                .ToList();

            if (apiRequest.FromDateTime.HasValue && apiRequest.ToDateTime.HasValue)
            {
                var dateRange = apiRequest.FromDateTime.Value.To(apiRequest.ToDateTime.Value).ToList();

                foreach (var date in dateRange)
                {
                    var currentPosts = posts.Where(p => p.CreatedDateTime != null && p.CreatedDateTime.Value.Date == date.Date).ToList();
                    //Post By Count
                    foreach (var orgUnit in organizationUnitDtos)
                    {
                        var staffIds = staffDetails.Where(x => x.Team.Id == orgUnit.Id).ToList().Select(x => x.User.Id);
                        var postsOfTeam = currentPosts.Where(x => x.AppUserId.HasValue && staffIds.Contains(x.AppUserId.Value));
                        result.DataChartCountPosts.Add
                        (
                            new DataChartItemDto<int, string>()
                            {
                                Display = date.ToString("dd/MM"),
                                Type = orgUnit.DisplayName,
                                Value = postsOfTeam.Count()
                            }
                        );
                    }

                    //Post By PostContentType
                    foreach (var enumValue in Enum.GetValues(typeof(PostContentType)))
                    {
                        var typePost = (PostContentType) enumValue;
                        if (typePost == PostContentType.Unknown) continue;
                        var postsByType = currentPosts.Where(x => x.PostContentType == typePost);

                        result.DataChartCountPostsByType.Add
                        (
                            new DataChartItemDto<int, PostContentType>()
                            {
                                Display = date.ToString("dd/MM"),
                                Type = typePost,
                                Value = postsByType.Count()
                            }
                        );
                    }

                    //Post By CampaignType
                    foreach (var enumValue in Enum.GetValues(typeof(CampaignType)))
                    {
                        var campaignType = (CampaignType) enumValue;
                        if (campaignType == CampaignType.Unknown) continue;
                        var campaignIds = campaigns.Where(x => x.CampaignType == campaignType).Select(x => x.Id).Distinct().ToList();
                        var postsByType = currentPosts.Where(x => x.CampaignId.HasValue && campaignIds.Contains(x.CampaignId.Value));
                        result.DataChartCountPostsByCampaignType.Add
                        (
                            new DataChartItemDto<int, CampaignType>()
                            {
                                Display = date.ToString("dd/MM"),
                                Type = campaignType,
                                Value = postsByType.Count()
                            }
                        );
                    }
                }
            }

            var groupPosts = posts.GroupBy(_ => _.PostContentType).ToList();
            foreach (var g in groupPosts)
            {
                var item = new GeneralStatsResponse.GeneralStatsResponseItem
                {
                    Type = g.Key.ToString(),
                };
                var p = g.Where(p => p.PostContentType == g.Key).ToList();
                item.TotalPost = p.Count;
                item.TotalUser = p.Where(_ => _.AppUserId.HasValue).DistinctBy(_ => _.AppUserId).Count();

                var totalReaction = p.Sum(post => post.TotalCount);
                if (item.TotalUser > 0)
                {
                    item.AvgPostPerUser = item.TotalPost > 0 ? item.TotalPost / item.TotalUser : 0;
                    item.AvgReactionPerUser = totalReaction > 0 ? totalReaction / item.TotalUser : 0;
                }

                if (item.TotalPost > 0) item.AvgReactionPerPost = totalReaction > 0 ? totalReaction / item.TotalPost : 0;

                result.Stats.Add(item);
            }

            var userAffiliates = await _userAffiliateRepository.GetListAsync
            (
                createdAtMin: apiRequest.FromDateTime,
                createdAtMax: apiRequest.ToDateTime
            );

            var gdlAffs = userAffiliates.Where
                    (_ => _.AffiliateUrl.IsNotNullOrEmpty() && (_.AffiliateUrl.Contains(GlobalConsts.BaseAffiliateDomain) || _.AffiliateUrl.Contains(GlobalConsts.GDLDomain)))
                .ToList();
            var hpdAffs = userAffiliates.Where(_ => _.AffiliateUrl.IsNotNullOrEmpty() && _.AffiliateUrl.Contains(GlobalConsts.HPDDomain)).ToList();
            var yinAffs = userAffiliates.Where(_ => _.AffiliateUrl.IsNotNullOrEmpty() && _.AffiliateUrl.Contains(GlobalConsts.YANDomain)).ToList();

            var affDictionary = new Dictionary<string, List<UserAffiliate>>
            {
                {GlobalConsts.BaseAffiliateDomain, gdlAffs},
                {GlobalConsts.HPDDomain, hpdAffs},
                {GlobalConsts.YANDomain, yinAffs},
            };

            foreach (var item in affDictionary)
            {
                result.Affiliates.Add
                (
                    new GeneralStatsResponse.AffiliateInfo
                    {
                        Name = item.Key,
                        Click = item.Value.Where(_ => _.AffConversionModel != null).Sum(_ => _.AffConversionModel.ClickCount),
                        Conversion = item.Value.Where(_ => _.AffConversionModel != null).Sum(_ => _.AffConversionModel.ConversionCount),
                        Amount = item.Value.Where(_ => _.AffConversionModel != null).Sum(_ => _.AffConversionModel.ConversionAmount),
                        Commission = item.Value.Where(_ => _.AffConversionModel != null).Sum(_ => _.AffConversionModel.CommissionAmount),
                    }
                );
            }

            result.Total_Group = groups.Count(g => g.IsActive && g.GroupSourceType == GroupSourceType.Group);
            result.Total_Page = groups.Count(g => g.IsActive && g.GroupSourceType == GroupSourceType.Page);

            return result;
        }

        public async Task<GrowthChartDto> GetGrowthChartStats(GeneralStatsApiRequest input)
        {
            if (!input.FromDateTime.HasValue || !input.ToDateTime.HasValue) { return new GrowthChartDto(); }

            var gdlGroups = await _groupRepository.GetListAsync(x => x.GroupOwnershipType == GroupOwnershipType.GDLInternal);
            var ids = gdlGroups.Select(x => x.Fid).ToList();
            var groupStatsHistories = _groupStatsHistoryRepository
                .WhereIf(ids.IsNotNullOrEmpty(), x => ids.Contains(x.GroupFid))
                .Where(x => x.CreatedAt.HasValue)
                .Where(x => x.GroupSourceType != GroupSourceType.Tiktok)
                .Where(x => x.CreatedAt >= input.FromDateTime)
                .Where(x => x.CreatedAt <= input.ToDateTime)
                .OrderBy(x => x.CreatedAt)
                .ToList();
            var growthChartStats = new GrowthChartDto {ChartLabels = input.FromDateTime.Value.To(input.ToDateTime.Value).Select(x => x.Date.ToString("dd-MM")).ToList()};

            foreach (var dateTime in input.FromDateTime.Value.To(input.ToDateTime.Value))
            {
                var groupStatsHistoriesPart = groupStatsHistories.Where(x => x.CreatedAt.Value.Date == dateTime.Date).ToList();
                var value = groupStatsHistoriesPart?.Sum(x => x.TotalInteractions);
                growthChartStats.TotalInteractionsLineCharts.Add
                (
                    new DataChartItemDto<double?, string>()
                    {
                        Display = dateTime.ToString("dd-MM"),
                        Type = "TotalInteractions",
                        Value = value == 0 ? null : value
                    }
                );
                growthChartStats.GrowthNumberBarCharts.Add
                (
                    new DataChartItemDto<double, string>()
                    {
                        Display = dateTime.ToString("dd-MM"),
                        Type = "GrowthNumbers",
                        Value = groupStatsHistoriesPart?.Sum(x => x.GrowthNumber) ?? 0
                    }
                );
            }

            return growthChartStats;
        }

        public async Task<GrowthCampaignChartDto> GetPartnerGrowthChartStats(GetGrowthCampaignChartsInput input, List<Campaign> campaigns, List<Guid> partnerIds)
        {
            if (!input.FromDateTime.HasValue || !input.ToDateTime.HasValue || campaigns.IsNullOrEmpty()) { return new GrowthCampaignChartDto(); }

            if (input.ToDateTime.Value.Date == DateTime.Now.Date)
            {
                input.ToDateTime = input.ToDateTime.Value.AddDays(-1);
            }

            var growthChartStats = new GrowthCampaignChartDto
            {
                //ChartLabels = input.FromDateTime.Value.To(input.ToDateTime.Value).Select(x => x.Date.ToString("dd-MM")).ToList()
                TotalCampaign = campaigns.Count,
                TotalPartner = partnerIds.Count,
            };

            var posts = _postRepository.Where
                (
                    x => x.CreatedDateTime >= input.FromDateTime
                         && x.CreatedDateTime <= input.ToDateTime
                )
                .OrderByDescending(o => o.TotalCount)
                .ToList();

            growthChartStats.TotalPostFb = posts.Count(x => x.PostSourceType == PostSourceType.Group || x.PostSourceType == PostSourceType.Page);
            growthChartStats.TotalPostTiktok = posts.Count(x => x.PostSourceType == PostSourceType.Video);

            foreach (var post in posts.Take(10))
            {
                var postStatisticDto = new PostStatisticDto
                {
                    Fid = post.Fid,
                    Url = post.Url,
                    CommentCount = post.CommentCount,
                    ShareCount = post.ShareCount,
                    LikeCount = post.LikeCount,
                    TotalCount = post.TotalCount,
                    PostContentType = post.PostContentType
                };

                if (post.GroupId.HasValue)
                {
                    var group = _groupRepository.FirstOrDefault(x => x.Id == post.GroupId.Value);
                    postStatisticDto.GroupName = group?.Name;
                }

                if (post.AppUserId.HasValue)
                {
                    var user = _userRepository.FirstOrDefault(x => x.Id == post.AppUserId.Value);
                    postStatisticDto.AuthorName = user?.UserName;
                }

                growthChartStats.TopReactionPosts.Add(postStatisticDto);
            }

            foreach (var gr in posts.Where(x => x.GroupId.HasValue).GroupBy(x => x.GroupId.Value))
            {
                var group = _groupRepository.FirstOrDefault(x => x.Id == gr.Key);
                if (group == null)
                {
                    continue;
                }

                var groupPosts = gr.ToList();
                if (groupPosts.Count > 0)
                {
                    var totalPost = groupPosts.Count;
                    var totalReaction = groupPosts.Sum(x => x.TotalCount);
                    var avgReactionPerPost = totalReaction / totalPost;

                    growthChartStats.Stats.Add
                    (
                        new GrowthCampaignChartDto.PartnerGeneralStatsResponseItem()
                        {
                            Group = group.Title.IsNotNullOrEmpty() ? group.Title : group.Fid,
                            TotalPost = totalPost,
                            TotalReactions = totalReaction,
                            AvgReactionPerPost = avgReactionPerPost
                        }
                    );
                }
            }

            return growthChartStats;
        }

        public async Task<TiktokGrowthChartDto> GetTiktokChartStats(GeneralStatsApiRequest input)
        {
            if (!input.FromDateTime.HasValue || !input.ToDateTime.HasValue) { return new TiktokGrowthChartDto(); }

            var tiktokStarts = new TiktokGrowthChartDto {ChartLabels = input.FromDateTime.Value.To(input.ToDateTime.Value).Select(x => x.Date.ToString("dd-MM")).ToList()};
            var groupStatsHistories = _groupStatsHistoryRepository
                .Where(x => x.GroupSourceType == input.GroupSourceType)
                .Where(x => x.CreatedAt >= input.FromDateTime)
                .Where(x => x.CreatedAt <= input.ToDateTime)
                .OrderBy(x => x.CreatedAt)
                .ToList();

            var tiktokStatsHitories = _tiktokStatRepository
                .Where(x => x.Date >= input.FromDateTime)
                .Where(x => x.Date <= input.ToDateTime)
                .OrderBy(x => x.Date)
                .ToList();

            var preTiktokFollower = 0;
            foreach (var dateTime in input.FromDateTime.Value.To(input.ToDateTime.Value))
            {
                var tiktokSta = tiktokStatsHitories.FirstOrDefault(x => x.Hashtag == GlobalConfiguration.ChartConfig.TikTokChartHashTag && x.Date == dateTime);
                tiktokStarts.TotalViewLineCharts.Add
                (
                    new DataChartItemDto<double?, string>()
                    {
                        Display = dateTime.ToString("dd-MM"),
                        Type = "TiktokTotalView",
                        Value = tiktokSta?.Count == 0 ? null : tiktokSta?.Count,
                    }
                );

                var groupStatsHistoriesPart = groupStatsHistories.Where(x => x.CreatedAt != null && x.CreatedAt.Value.Date == dateTime.Date).ToList();

                var follower = groupStatsHistoriesPart?.Sum(x => x.GroupMembers) ?? 0;
                tiktokStarts.TiktokFollowerGrowths.Add
                (
                    new TiktokFollowerGrowth
                    {
                        Date = dateTime.ToString("d"),
                        Follower = follower,
                        FollowerChange = preTiktokFollower != 0 && follower != 0 ? follower - preTiktokFollower : 0
                    }
                );
                preTiktokFollower = follower;
            }

            return tiktokStarts;
        }

        public async Task<MultipleDataSourceChart<double>> GetMCNGDLTikTokChartStats(GeneralStatsApiRequest input)
        {
            input.TikTokMcnType = TikTokMCNType.MCNGdl;
            return await GetMCNTikTokChart(input);
        }

        public async Task<MultipleDataSourceChart<long>> GetMCNVietNamTikTokChartStats(GeneralStatsApiRequest input)
        {
            input.TikTokMcnType = TikTokMCNType.MCNVietNam;

            if (!input.FromDateTime.HasValue || !input.ToDateTime.HasValue) { return new MultipleDataSourceChart<long>(); }

            var mcns = await _tikTokMcnRepository.GetListAsync(x => x.MCNType == input.TikTokMcnType);
            var hashtags = mcns.Select(x => x.HashTag).Distinct();
            if (input.TikTokMcnIds.IsNotNullOrEmpty()) mcns = mcns.Where(_ => input.TikTokMcnIds.Contains(_.Id)).ToList();

            var reportMonths = GetReportMonths(input.FromDateTime.Value, input.ToDateTime.Value).ToList();
            var monthStarts = reportMonths.Select(y => y.monthStart);
            var growthChartStats = new MultipleDataSourceChart<long>() {ChartLabels = reportMonths.Select(_ => _.monthStart.ToString("MM-yyyy")).ToList()};
            var tiktokStatsResource = _tiktokStatRepository.Where(x => monthStarts.Contains(x.Date) && hashtags.Contains(x.Hashtag)).ToList();
            if (tiktokStatsResource.IsNullOrEmpty()) return growthChartStats;
            var tiktokStatGroups = tiktokStatsResource.GroupBy(x => x.Hashtag).OrderByDescending(x => x.Sum(y => y.Count)).ToList();

            if (input.Count > 0) tiktokStatGroups = tiktokStatGroups.Take(input.Count).ToList();

            var topMcns = tiktokStatGroups.Select(x => x.Key).ToList();

            foreach (var mcn in mcns)
            {
                if (!topMcns.Contains(mcn.HashTag)) continue;
                var tiktokStats = tiktokStatGroups.FirstOrDefault(x => x.Key == mcn.HashTag);

                var chartItems = reportMonths.Select
                    (
                        reportMonth => new DataChartItemDto<long, string>
                        {
                            Display = reportMonth.monthStart.ToString("MM-yyyy"),
                            Type = mcn.Name,
                            Value = tiktokStats?.FirstOrDefault(x => x.Date == reportMonth.monthStart)?.Count ?? 0
                        }
                    )
                    .ToList();
                growthChartStats.MultipleDataCharts.Add(chartItems);
            }

            return growthChartStats;
        }

        public async Task<CampaignDailyChartResponse> GetCampaignDailyChartStats(Guid campaignId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            var posts = await _postRepository.GetListWithNavigationPropertiesExtendAsync(campaignId: campaignId);
            var tiktoks = await _tiktokRepository.GetListWithNavigationPropertiesExtendAsync(campaignId: campaignId);
            var result = new CampaignDailyChartResponse();
            var timeFrames = GetDailyTimeFrames(fromDate, toDate);
            foreach (var date in timeFrames)
            {
                foreach (var type in Enum.GetValues(typeof(PostContentType)).Cast<PostContentType>())
                {
                    if (!(type == PostContentType.Unknown || type == PostContentType.D2C ))
                    {
                        result.Data.Add
                        (
                            new DataChartItemDto<int, string>()
                            {
                                Display = date.Display,
                                Type = type.ToString(),
                                Value = posts.Count
                                (
                                    x => x.Post.CreatedDateTime != null
                                         && x.Post.PostContentType == type
                                         && x.Post.CreatedDateTime.Value >= date.FromDateTime
                                         && x.Post.CreatedDateTime.Value <= date.ToDateTime
                                )
                            }
                        );
                    }
                }

                result.Data.Add
                (
                    new DataChartItemDto<int, string>()
                    {
                        Display = date.Display,
                        Type = GlobalConsts.TikTok,
                        Value = tiktoks.Count(x => x.Tiktok.CreatedDateTime != null && x.Tiktok.CreatedDateTime.Value >= date.FromDateTime && x.Tiktok.CreatedDateTime.Value <= date.ToDateTime)
                    }
                );
            }

            return result;
        }

        private async Task<MultipleDataSourceChart<double>> GetMCNTikTokChart(GeneralStatsApiRequest input)
        {
            if (!input.FromDateTime.HasValue || !input.ToDateTime.HasValue) { return new MultipleDataSourceChart<double>(); }

            var mcns = await _tikTokMcnRepository.GetListAsync(x => x.MCNType == input.TikTokMcnType);
            var mcnIds = mcns.Select(x => x.Id);
            var gdlGroups = await _groupRepository.GetListAsync(x => x.McnId.HasValue && mcnIds.Contains(x.McnId.Value));
            var ids = gdlGroups.Select(x => x.Fid).ToList();
            var groupStatsHistories = _groupStatsHistoryRepository
                .WhereIf(ids.IsNotNullOrEmpty(), x => ids.Contains(x.GroupFid))
                .Where(x => x.CreatedAt.HasValue)
                .Where(x => x.GroupSourceType == GroupSourceType.Tiktok)
                .Where(x => x.CreatedAt >= input.FromDateTime)
                .Where(x => x.CreatedAt <= input.ToDateTime)
                .OrderBy(x => x.CreatedAt)
                .ToList();
            var growthChartStats = new MultipleDataSourceChart<double>() {ChartLabels = input.FromDateTime.Value.To(input.ToDateTime.Value).Select(x => x.Date.ToString("dd-MM")).ToList()};

            var dataLineCharts = new List<DataChartItemDto<double, string>>();
            var dataBarCharts = new List<DataChartItemDto<double, string>>();
            foreach (var dateTime in input.FromDateTime.Value.To(input.ToDateTime.Value))
            {
                var groupStatsHistoriesPart = groupStatsHistories.Where(x => x.CreatedAt.Value.Date == dateTime.Date).ToList();
                var value = (double) groupStatsHistoriesPart.Sum(x => x.TotalInteractions);
                dataLineCharts.Add
                (
                    new DataChartItemDto<double, string>()
                    {
                        Display = dateTime.ToString("dd-MM"),
                        Type = "TotalViews",
                        Value = value
                    }
                );
                dataBarCharts.Add
                (
                    new DataChartItemDto<double, string>()
                    {
                        Display = dateTime.ToString("dd-MM"),
                        Type = "TotalCreator",
                        Value = groupStatsHistoriesPart?.Sum(x => x.GrowthNumber) ?? 0
                    }
                );
            }

            growthChartStats.MultipleDataCharts.Add(dataLineCharts);
            growthChartStats.MultipleDataCharts.Add(dataBarCharts);

            return growthChartStats;
        }

        private static IEnumerable<(DateTime monthStart, DateTime monthEnd)> GetReportMonths(DateTime timeFrom, DateTime timeTo)
        {
            var maxMonthStart = DateTime.SpecifyKind(new DateTime(timeTo.Year, timeTo.Month, 1), DateTimeKind.Utc);
            var results = new List<(DateTime monthStart, DateTime monthEnd)>();

            while (true)
            {
                var monthStart = DateTime.SpecifyKind(new DateTime(timeFrom.Year, timeFrom.Month, 1), DateTimeKind.Utc);

                if (monthStart > maxMonthStart)
                    break;

                var monthEnd = monthStart.AddMonths(1).AddSeconds(-1);

                results.Add(new ValueTuple<DateTime, DateTime>(monthStart, monthEnd));
                timeFrom = monthEnd.AddDays(1).Date;
            }

            return results.OrderBy(x => x.monthStart);
        }
    }
}