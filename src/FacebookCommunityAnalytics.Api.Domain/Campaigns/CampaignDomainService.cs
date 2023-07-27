using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AffiliateConversions;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Exceptions;
using FacebookCommunityAnalytics.Api.Exports;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.Statistics;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.UserAffiliates;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;
using Hangfire;
using Humanizer;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.Campaigns
{
    public interface ICampaignDomainService : IDomainService
    {
        Task<Campaign> GetByIdOrCode(string idOrCode);
        Task<Campaign> GetByName(string name);
        Task<List<Campaign>> GetListByIdOrCode(List<string> idOrCode);
        Task<List<Campaign>> GetRunningCampaigns(List<Guid> partnerIds);
        Task<List<CampaignPost>> GetPosts(Guid campaignId);
        Task<List<UserAffiliateWithNavigationPropertiesDto>> GetAffiliatesAsync(List<string> shortLinks);
        Task<byte[]> GetCampaignExcelAsync(Guid campaignId);
        Task CreateCampaignPosts(PostCreateDto input, Guid userId, List<Guid> partnerUserIds);
        Task RemoveCampaignPost(Guid postId);
        Task UpdateStatuses();
        Task UpdateStatus(string campaignCode);
        Task<string> RegisterCampaignUsers(string email, string name, string surname, Campaign campaign);
        CampaignStatus GetCampaignStatus(DateTime? startDateTime, DateTime? endDateTime);
        Task<List<TiktokWithNavigationProperties>> GetTikToks(GetTiktoksInputExtend input);
        Task UpdateCampaignTiktok(TiktokCreateUpdateDto input, Guid id);
        Task UpdateCampaignPostCount(Guid campaignId);
        Task<PieChartDataSource<double>> GetPostCountGroupsChart(DateTime fromDate, DateTime toDate);
        Task<PieChartDataSource<double>> GetReactionGroupsChart(DateTime fromDate, DateTime toDate);
        Task<List<AuthorStatistic>> GetAuthorStatistic();
        Task<List<CampaignDto>> GetCampsByTime(DateTime from, DateTime to);
    }

    public class CampaignDomainService : BaseDomainService, ICampaignDomainService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IPostRepository _postRepository;
        private readonly IPostDomainService _postDomainService;
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IdentityUserManager _identityUserManager;
        private readonly IGroupRepository _groupRepository;
        private readonly IUserAffiliateRepository _userAffiliateRepository;
        private readonly ITiktokRepository _tiktokRepository;
        private readonly IExportDomainService _exportDomainService;
        private readonly IContractRepository _contractRepository;
        private readonly IAffiliateConversionRepository _affiliateConversionRepository;

        public CampaignDomainService(
            ICampaignRepository campaignRepository,
            IPostRepository postRepository,
            IGroupRepository groupRepository,
            IUserAffiliateRepository userAffiliateRepository,
            IExportDomainService exportDomainService,
            IRepository<AppUser, Guid> userRepository,
            IdentityUserManager identityUserManager,
            IUserInfoRepository userInfoRepository,
            IPostDomainService postDomainService,
            ITiktokRepository tiktokRepository,
            IContractRepository contractRepository,
            IAffiliateConversionRepository affiliateConversionRepository)
        {
            _campaignRepository = campaignRepository;
            _postRepository = postRepository;
            _groupRepository = groupRepository;
            _userAffiliateRepository = userAffiliateRepository;
            _exportDomainService = exportDomainService;
            _userRepository = userRepository;
            _identityUserManager = identityUserManager;
            _userInfoRepository = userInfoRepository;
            _postDomainService = postDomainService;
            _tiktokRepository = tiktokRepository;
            _contractRepository = contractRepository;
            _affiliateConversionRepository = affiliateConversionRepository;
        }

        public async Task<Campaign> GetByName(string name)
        {
            if (name.IsNullOrWhiteSpace()) throw new ApiException(L[ApiDomainErrorCodes.Campaign.InvalidName]);
            name = name.ToLower().Trim();

            var queryable = await _campaignRepository.GetQueryableAsync();
            var existing = queryable.FirstOrDefault(c => c.Name.ToLower() == name);

            return existing;
        }

        public async Task<Campaign> GetByIdOrCode(string idOrCode)
        {
            Campaign campaign;
            var isGuid = Guid.TryParse(idOrCode, out var guid);
            if (isGuid)
            {
                campaign = await _campaignRepository.FindAsync(guid);
            }
            else
            {
                campaign = await _campaignRepository.FirstOrDefaultAsync(c => c.Code == idOrCode.Trim().ToLower());
            }

            return campaign;
        }

        public async Task<List<Campaign>> GetListByIdOrCode(List<string> idsOrCodes)
        {
            var campaigns = new List<Campaign>();
            foreach (var idOrCode in idsOrCodes)
            {
                campaigns.Add(await GetByIdOrCode(idOrCode));
            }

            return campaigns;
        }

        public async Task<List<Campaign>> GetRunningCampaigns(List<Guid> partnerIds)
        {
            var campaignStatusFilters = new List<CampaignStatus>()
            {
                CampaignStatus.Started,
                CampaignStatus.Hold
            };
            if (partnerIds.IsNotNullOrEmpty())
            {
                return await _campaignRepository.GetListAsync(x => campaignStatusFilters.Contains(x.Status) && x.PartnerId.HasValue && partnerIds.Contains(x.PartnerId.Value));
            }
            else
            {
                return await _campaignRepository.GetListAsync(x => campaignStatusFilters.Contains(x.Status));
            }
        }

        public async Task<List<CampaignPost>> GetPosts(Guid campaignId)
        {
            var posts = await _postRepository.GetListExtendAsync(campaignId: campaignId);
            var groups = await _groupRepository.GetListAsync();

            var campaignPosts = ObjectMapper.Map<List<Post>, List<CampaignPost>>(posts);

            foreach (var item in campaignPosts)
            {
                var g = groups.FirstOrDefault(_ => _.Id == item.GroupId);
                if (g != null)
                {
                    var name = g.Title.IsNotNullOrEmpty() ? g.Title : g.Name;
                    item.GroupName =$"{name} ({g.GroupSourceType})";
                }

                var appUserId = item.AppUserId;
                if (appUserId is not null)
                {
                    var user = await _userRepository.GetAsync(appUserId.Value);
                    item.Username = user.UserName;
                }
            }

            return campaignPosts;
        }
      
        public async Task<List<UserAffiliateWithNavigationPropertiesDto>> GetAffiliatesAsync(List<string> shortLinks)
        {
            var shortKeys = shortLinks.Select(UrlHelper.GetShortKey);
            var conversions = await _affiliateConversionRepository.GetListExtendAsync(shortKeys: shortKeys);
            var items = await _userAffiliateRepository.GetUserAffiliateWithNavigationProperties(shortLinks: shortLinks);
            foreach (var item in items)
            {
                var itemConversions = conversions.Where(_ => _.ShortKey == UrlHelper.GetShortKey(item.UserAffiliate.AffiliateUrl)).ToList();
                item.UserAffiliate.AffConversionModel.ConversionCount = itemConversions.Count;
                item.UserAffiliate.AffConversionModel.ConversionAmount = itemConversions.Sum(_ => _.SaleAmount);
            }
            return ObjectMapper
                .Map<List<UserAffiliateWithNavigationProperties>,
                    List<UserAffiliateWithNavigationPropertiesDto>>(items);
        }

        //Todoo Long: get conversions
        public async Task<byte[]> GetCampaignExcelAsync(Guid campaignId)
        {
            var campaignPosts = await _postRepository.GetListWithNavigationPropertiesExtendAsync(campaignId: campaignId);
            campaignPosts = campaignPosts.Where(_ => _.Post.TotalCount != 0).OrderByDescending(_ => _.Post.TotalCount).ToList();
            var tiktokVideos = await _tiktokRepository.GetListWithNavigationPropertiesExtendAsync(campaignId: campaignId);
            var userAffiliates = await _userAffiliateRepository.GetUserAffiliateWithNavigationProperties(campaignId: campaignId);
            // var affConversions = await _affiliateConversionRepository.GetListAsync();
            // foreach (var aff in userAffiliates)
            // {
            //     var conversions = affConversions.Where(_ => _.ShortKey == UrlHelper.GetShortKey(aff.UserAffiliate.AffiliateUrl)).ToList();
            //     aff.UserAffiliate.AffConversionModel.ConversionAmount = conversions.Count;
            //     aff.UserAffiliate.AffConversionModel.CommissionAmount = conversions.Sum(_ => _.PayoutBonus);
            //     aff.UserAffiliate.AffConversionModel.ConversionAmount = conversions.Sum(_ => _.SaleAmount);
            // }
            return _exportDomainService.GenerateCampaignDetailExcelBytes
            (
                new GetCampaignDetailExportRequest()
                {
                    Posts = ObjectMapper.Map<List<PostWithNavigationProperties>, List<PostExportRow>>(campaignPosts),
                    Affiliates = ObjectMapper.Map<List<UserAffiliateWithNavigationProperties>, List<UserAffiliateExportRow>>(userAffiliates),
                    TikToks = ObjectMapper.Map<List<TiktokWithNavigationProperties>, List<TiktokExportRow>>(tiktokVideos)
                },
                null
            );
        }

        public CampaignStatus GetCampaignStatus(DateTime? startDate, DateTime? endDate)
        {
            var now = DateTime.UtcNow;
            CampaignStatus status;

            if (startDate <= now)
            {
                if (endDate == null)
                {
                    status = CampaignStatus.Started;
                }
                else
                {
                    if (endDate.Value.TimeOfDay == TimeSpan.Zero)
                    {
                        status = endDate <= now.AddDays(1) ? CampaignStatus.Ended : CampaignStatus.Started;
                    }
                    else
                    {
                        status = endDate <= now ? CampaignStatus.Ended : CampaignStatus.Started;
                    }
                }
            }
            else
            {
                status = CampaignStatus.Unknown;
            }

            return status;
        }

        /// <summary>
        /// If status is updated, do update the campaign(s).
        /// In case the new status is hold or ended, we need to trigger last crawling for the campaign posts
        /// </summary>
        public async Task UpdateStatuses()
        {
            var campaignsToUpdate = new List<Campaign>();

            foreach (var campaign in await _campaignRepository.GetListAsync())
            {
                var newStatus = GetCampaignStatus(campaign.StartDateTime, campaign.EndDateTime);
                var oldStatus = campaign.Status;
                if (newStatus == oldStatus) continue;

                campaign.Status = newStatus;
                await TriggerCampaignPostCrawling(newStatus, oldStatus, campaign.Code);
                campaignsToUpdate.Add(campaign);
            }

            if (campaignsToUpdate.IsNotNullOrEmpty())
            {
                await _campaignRepository.UpdateManyAsync(campaignsToUpdate);
            }
        }

        /// <summary>
        /// If status is updated, do update the campaign(s).
        /// In case the new status is hold or ended, we need to trigger last crawling for the campaign posts
        /// </summary>
        public async Task UpdateStatus(string campaignCode)
        {
            if (campaignCode.IsNullOrWhiteSpace()) return;

            var campaign = await _campaignRepository.FindAsync(x => x.Code == campaignCode);
            if (campaign is null) return;

            var newStatus = GetCampaignStatus(campaign.StartDateTime, campaign.EndDateTime);
            var oldStatus = campaign.Status;
            if (newStatus != oldStatus)
            {
                campaign.Status = newStatus;
                await TriggerCampaignPostCrawling(newStatus, oldStatus, campaign.Code);
                await _campaignRepository.UpdateAsync(campaign);
            }
        }

        /// <summary>
        /// If status is change and the new status is hold or ended, we need to trigger last crawling for the campaign posts
        /// </summary>
        private Task TriggerCampaignPostCrawling(CampaignStatus newStatus, CampaignStatus oldStatus, string campaignCode)
        {
            if (newStatus == oldStatus) return Task.CompletedTask;

            switch (newStatus)
            {
                case CampaignStatus.Unknown: break;
                case CampaignStatus.Draft: break;
                case CampaignStatus.Started: break;

                case CampaignStatus.Hold:
                case CampaignStatus.Ended:
                    BackgroundJob.Enqueue<ICrawlDomainService>(_ => _.InitCampaignPosts(new List<string> {campaignCode}));
                    break;
                case CampaignStatus.Archived: break;
                default: throw new ArgumentOutOfRangeException();
            }

            return Task.CompletedTask;
        }

        public async Task<string> RegisterCampaignUsers(string email, string name, string surname, Campaign campaign)
        {
            var users = await _userRepository.ToListAsync();
            var existingUser = users.FirstOrDefault(_ => _.Email.ToLower().Equals(email) || _.UserName.ToLower().Equals(email));

            if (existingUser != null) { return string.Empty; }

            var identityUser = new IdentityUser(Guid.Empty, email, email)
            {
                Name = name,
                Surname = surname,
            };

            var password = $"GDL{StringHelper.RandomStringAll(8).FirstLetterToUpper().Transform(To.LowerCase, To.TitleCase)}";
            await _identityUserManager.CreateAsync(identityUser, password);
            if (identityUser.Id != Guid.Empty)
            {
                await _identityUserManager.SetRolesAsync
                (
                    identityUser,
                    new List<string>
                    {
                        RoleConsts.CampaignViewer,
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

            return password;
        }

        public async Task RemoveCampaignPost(Guid postId)
        {
            var entity = await _postRepository.FirstOrDefaultAsync(x => x.Id == postId);
            if (entity != null)
            {
                entity.CampaignId = null;
                entity.IsCampaignManual = false;
                await _postRepository.UpdateAsync(entity);
            }
        }

        public async Task CreateCampaignPosts(PostCreateDto input, Guid userId, List<Guid> partnerUserIds)
        {
            if (input.CampaignId == null) return;

            var urls = input.Url.Split("\n").Where(_ => _.IsNotNullOrWhiteSpace()).Select(_ => _.Trim()).Distinct();
            var createPosts = new List<Post>();

            foreach (var url in urls.Where(FacebookHelper.IsValidGroupPostUrl))
            {
                var cleanUrl = FacebookHelper.GetCleanUrl(url).ToString();
                var campaign = await _campaignRepository.GetAsync((Guid) input.CampaignId);

                var existingPosts = await _postRepository.GetListExtendAsync(url: cleanUrl);
                if (existingPosts.IsNotNullOrEmpty())
                {
                    existingPosts = existingPosts.Select
                        (
                            _ =>
                            {
                                if (_.CampaignId != input.CampaignId)
                                {
                                    _.IsCampaignManual = true;
                                }
                                if (_.PostContentType != input.PostContentType)
                                {
                                    _.IsPostContentTypeManual = true;
                                }
                                _.CampaignId = input.CampaignId;
                                _.PostContentType = input.PostContentType;
                                _.PartnerId = campaign.PartnerId;
                                return _;
                            }
                        )
                        .ToList();
                    await _postRepository.UpdateManyAsync(existingPosts);
                }
                else
                {
                    var post = ObjectMapper.Map<PostCreateDto, Post>(input);
                    post.Url = cleanUrl;
                    post = await _postDomainService.InitPostCreation(post, userId, partnerUserIds);
                    if (post == null) continue;
                    post.PartnerId = campaign.PartnerId;
                    post.IsCampaignManual = true;
                    post.IsPostContentTypeManual = true;
                    createPosts.Add(post);
                }
            }

            if (createPosts.IsNotNullOrEmpty()) await _postRepository.InsertManyAsync(createPosts, autoSave: true);
        }

        public async Task<List<TiktokWithNavigationProperties>> GetTikToks(GetTiktoksInputExtend input)
        {
            return await _tiktokRepository.GetListWithNavigationPropertiesExtendAsync(filterText: input.Search, videoId: input.VideoId, campaignId: input.CampaignId);
        }

        public async Task UpdateCampaignTiktok(TiktokCreateUpdateDto input, Guid id)
        {
            var tikTok = await _tiktokRepository.GetAsync(id);
            var updateTikTok = ObjectMapper.Map(input, tikTok);
            await _tiktokRepository.UpdateAsync(updateTikTok);
        }

        public async Task UpdateCampaignPostCount(Guid campaignId)
        {
            var campaign = await _campaignRepository.GetAsync(campaignId);
            var posts = await _postRepository.GetListExtendAsync(campaignId: campaignId);
            var tiktokCount = await _tiktokRepository.GetCountExtendAsync(campaignId: campaignId);

            campaign.FacebookCount = posts.Count;
            campaign.TikTokCount = (int)tiktokCount;
            campaign.TotalLike = posts.Sum(_ => _.LikeCount);
            campaign.TotalShare = posts.Sum(_ => _.ShareCount);
            campaign.TotalComment = posts.Sum(_ => _.CommentCount);
            campaign.TotalReaction = posts.Sum(_ => _.TotalCount);
            
            await _campaignRepository.UpdateAsync(campaign);
        }

        public async Task<PieChartDataSource<double>> GetPostCountGroupsChart(DateTime fromDate, DateTime toDate)
        {
            var result = new PieChartDataSource<double>();
            var campaigns = await _campaignRepository.GetListAsync(_ => (_.StartDateTime >= fromDate && _.StartDateTime <= toDate) || (_.EndDateTime >= toDate && _.EndDateTime <= toDate) || (_.StartDateTime < fromDate && _.EndDateTime > toDate));
            var groups = await _groupRepository.GetListAsync();
            var posts = await _postRepository.GetListExtendAsync(campaignIds: campaigns.Select(_ => _.Id), groupIds: groups.Select(_ => _.Id));
            var postsGroupBy = posts.GroupBy(_ => _.GroupId).ToList();
            var postCount = (double)posts.Count;
            foreach (var item in postsGroupBy.OrderByDescending(_ => _.Count()))
            {
                result.Items.Add(new PieChartItem<double>()
                {
                    Label = groups.FirstOrDefault(_ => _.Id == item.Key)?.Title,
                    Value = item.Count(),
                    ValuePercent = item.Any() ? Math.Round((item.Count() * 100) / postCount, 1) : 0
                });
            }
            return result;
        }

        public async Task<PieChartDataSource<double>> GetReactionGroupsChart(DateTime fromDate, DateTime toDate)
        {
            var result = new PieChartDataSource<double>();
            var campaigns = await _campaignRepository.GetListAsync(_ => (_.StartDateTime >= fromDate && _.StartDateTime <= toDate) 
                                                                        || (_.EndDateTime >= fromDate && _.EndDateTime <= toDate) 
                                                                        || (_.StartDateTime < fromDate && _.EndDateTime > toDate));
            var groups = await _groupRepository.GetListAsync();
            var posts = await _postRepository.GetListExtendAsync(campaignIds: campaigns.Select(_ => _.Id), groupIds: groups.Select(_ => _.Id));
            var postsGroupBy = posts.GroupBy(_ => _.GroupId).ToList();
            var postTotalReaction = (double)posts.Sum(_ => _.TotalCount);
            foreach (var item in postsGroupBy.OrderByDescending(_ => _.Sum(p =>p.TotalCount)))
            {
                result.Items.Add(new PieChartItem<double>()
                {
                    Label = groups.FirstOrDefault(_ => _.Id == item.Key)?.Title,
                    Value = item.Sum(_ => _.TotalCount),
                    ValuePercent = item.Sum(_ => _.TotalCount) > 0 ? Math.Round(((item.Sum(_ => _.TotalCount) * 100) / postTotalReaction ), 1) : 0 
                });
            }
            return result;
        }

        public async Task<List<AuthorStatistic>> GetAuthorStatistic()
        {
            var result = new List<AuthorStatistic>();
            var campaigns = await _campaignRepository.GetListAsync();
            var posts = await _postRepository.GetListExtendAsync(campaignIds: campaigns.Select(_ => _.Id));
            var postGroupBy = posts.Where(_ => _.CreatedBy.IsNotNullOrEmpty()).GroupBy(_ => _.CreatedBy).ToList();
            foreach (var item in postGroupBy)
            {
                var postCountEvaRate = posts.Count != 0 ? (decimal)item.Count() / posts.Count : 0;
                var reactionEvaRate = posts.Sum(_ => _.TotalCount) != 0 ? (decimal)item.Sum(_ => _.TotalCount) / posts.Sum(_ => _.TotalCount) : 0;
                var campaignEvaRate = campaigns.Count != 0 ? (decimal)item.GroupBy(_ => _.CampaignId).Count() / campaigns.Count : 0;
                
                result.Add
                (
                    new AuthorStatistic()
                    {
                        Author = item.Key,
                        PostCount = item.Count(),
                        TotalReaction = item.Sum(_ => _.TotalCount),
                        CampaignCount = item.GroupBy(_ => _.CampaignId).Count(),
                        EvaluationRate = (postCountEvaRate + reactionEvaRate + campaignEvaRate) / 3
                    }
                );
            }
            return result.OrderByDescending(_ => _.EvaluationRate).ToList();
        }
        
        public async Task<List<CampaignDto>> GetCampsByTime(DateTime from, DateTime to)
        {
            var campaigns = ObjectMapper.Map<List<Campaign>, List<CampaignDto>>(await _campaignRepository.GetListAsync
            (
                x => (x.StartDateTime < from && x.EndDateTime > to)
                     || (x.StartDateTime >= from && x.StartDateTime <= to)
                     || (x.EndDateTime >= from && x.EndDateTime <= to)
            ));

            var campaignPosts = await _postRepository.GetListExtendAsync(campaignIds: campaigns.Select(x => x.Id));
            var campaignTikTokVideos = await _tiktokRepository.GetListExtendAsync(campaignIds: campaigns.Select(x => x.Id));
            var shortLinks = campaignPosts.Where(_ => _.PostContentType == PostContentType.Affiliate).SelectMany(_ => _.Shortlinks).ToList();
            var shortKeys = shortLinks.Select(UrlHelper.GetShortKey);
            var conversions = await _affiliateConversionRepository.GetListExtendAsync(shortKeys: shortKeys);
            var userAffiliates = await _userAffiliateRepository.GetListAsync(shortLinks: shortLinks);
            foreach (var item in userAffiliates)
            {
                var itemConversions = conversions.Where(_ => _.ShortKey == UrlHelper.GetShortKey(item.AffiliateUrl)).ToList();
                item.AffConversionModel.ConversionCount = itemConversions.Count;
                item.AffConversionModel.ConversionAmount = itemConversions.Sum(_ => _.SaleAmount);
            }
            foreach (var item in campaigns)
            {
                var posts = campaignPosts.Where(x => x.CampaignId == item.Id).ToList();
                var videos = campaignTikTokVideos.Where(x => x.CampaignId == item.Id).ToList();
                var itemShortLinks = posts.Where(_ => _.PostContentType == PostContentType.Affiliate).SelectMany(_ => _.Shortlinks).ToList();
                var affiliates = userAffiliates.Where(_ => itemShortLinks.Contains(_.AffiliateUrl)).ToList();
                CalculateQuantityKPI(item, posts, videos);
                CalculateQualityKPI(item, posts, videos, affiliates);
            }
            return campaigns;
        }

        private void CalculateQuantityKPI(CampaignDto campaign, List<Post> campaignPosts, List<Tiktok> campaignTikTokVideos)
        {
            var kpiPercents = new List<decimal>();
            var seedingPostCount = campaignPosts.Count(x => x.PostContentType == PostContentType.Seeding);
            var contestPostCount = campaignPosts.Count(x => x.PostContentType == PostContentType.Contest);
            var affiliatePostCount = campaignPosts.Count(x => x.PostContentType == PostContentType.Affiliate);
            var tikTokPostCount = campaignTikTokVideos.Count();

            if (seedingPostCount > 0)
            {
                campaign.QuantityKPIDescription += L["Campaign.QuantityKPIDescription.Seeding", seedingPostCount, campaign.Target.Seeding_TotalPost] + "|";
                if (campaign.Target.Seeding_TotalPost > 0)
                {
                    kpiPercents.Add(seedingPostCount / (decimal)campaign.Target.Seeding_TotalPost);
                }
            }

            if (contestPostCount > 0)
            {
                campaign.QuantityKPIDescription += L["Campaign.QuantityKPIDescription.Contest", contestPostCount, campaign.Target.Contest_TotalPost] + "|";

                if (campaign.Target.Contest_TotalPost > 0)
                {
                    kpiPercents.Add(contestPostCount / (decimal)campaign.Target.Contest_TotalPost);
                }
            }

            if (affiliatePostCount > 0)
            {
                campaign.QuantityKPIDescription += L["Campaign.QuantityKPIDescription.Affiliate", affiliatePostCount, campaign.Target.Affiliate_TotalPost] + "|";
                if (campaign.Target.Affiliate_TotalPost > 0)
                {
                    kpiPercents.Add(affiliatePostCount / (decimal)campaign.Target.Affiliate_TotalPost);
                }
            }

            if (tikTokPostCount > 0)
            {
                campaign.QuantityKPIDescription += L["Campaign.QuantityKPIDescription.TikTok", tikTokPostCount, campaign.Target.TikTok_TotalVideo] + "|";
                if (campaign.Target.TikTok_TotalVideo > 0)
                {
                    kpiPercents.Add(tikTokPostCount / (decimal)campaign.Target.TikTok_TotalVideo);
                }
            }
            campaign.QuantityKPI = kpiPercents.IsNotNullOrEmpty() ? (int)(Math.Round((kpiPercents.Sum() / (decimal)kpiPercents.Count), 2) * 100) : 0;
            campaign.QuantityKPIDescription = campaign.QuantityKPIDescription.IsNullOrEmpty() ? GlobalConsts.NotApplicable : campaign.QuantityKPIDescription;
            
        }

        public void CalculateQualityKPI(CampaignDto campaign, List<Post> campaignPosts, List<Tiktok> campaignTiktokVideos, List<UserAffiliate> affiliates)
        {
            var kpiPercents = new List<decimal>();

            var seedingReaction = campaignPosts.Where(x => x.PostContentType == PostContentType.Seeding).Sum(x => x.TotalCount);
            var contestReaction = campaignPosts.Where(x => x.PostContentType == PostContentType.Contest).Sum(x => x.TotalCount);
            var affiliateClickCount = affiliates.Sum(_ => _.AffConversionModel.ClickCount);
            var affiliateConversionCount = affiliates.Sum(_ => _.AffConversionModel.ConversionCount);
            var tikTokReactionCount = campaignTiktokVideos.Sum(x => x.ViewCount);
            

            if (seedingReaction > 0)
            {
                campaign.QualityKPIDescription += L["Campaign.QualityKPIDescription.Seeding", seedingReaction, campaign.Target.Seeding_TotalReaction] + "|";
                if (campaign.Target.Seeding_TotalReaction > 0)
                {
                    kpiPercents.Add(seedingReaction / (decimal)campaign.Target.Seeding_TotalReaction);
                }
            }

            if (contestReaction > 0)
            {
                campaign.QualityKPIDescription += L["Campaign.QualityKPIDescription.Contest", contestReaction, campaign.Target.Contest_TotalReaction]+ "|";
                if (campaign.Target.Contest_TotalReaction > 0)
                {
                    kpiPercents.Add(contestReaction / (decimal)campaign.Target.Contest_TotalReaction);
                }
            }

            if (affiliateClickCount > 0)
            {
                campaign.QualityKPIDescription += L["Campaign.QualityKPIDescription.Affiliate_TotalClick", affiliateClickCount, campaign.Target.Affiliate_TotalClick] + "|";
                if (campaign.Target.Affiliate_TotalClick > 0)
                {
                    kpiPercents.Add(affiliateClickCount / (decimal)campaign.Target.Affiliate_TotalClick);
                }
            }
            
            if (affiliateConversionCount > 0)
            {
                campaign.QualityKPIDescription += L["Campaign.QualityKPIDescription.Affiliate_TotalConversion", affiliateConversionCount, campaign.Target.Affiliate_TotalConversion] + "|";
                if (campaign.Target.Affiliate_TotalConversion > 0)
                {
                    kpiPercents.Add(affiliateConversionCount / (decimal)campaign.Target.Affiliate_TotalConversion);
                }
            }

            if (tikTokReactionCount > 0)
            {
                campaign.QualityKPIDescription += L["Campaign.QualityKPIDescription.TikTok", tikTokReactionCount, campaign.Target.TikTok_TotalView] + "|";
                if (campaign.Target.TikTok_TotalView > 0)
                {
                    kpiPercents.Add(tikTokReactionCount / (decimal)campaign.Target.TikTok_TotalView);
                }
            }
            
            campaign.QualityKPI = kpiPercents.IsNotNullOrEmpty() ? (int)(Math.Round((kpiPercents.Sum() / (decimal)kpiPercents.Count), 2) * 100) : 0;
            campaign.QualityKPIDescription = campaign.QualityKPIDescription.IsNullOrEmpty() ? GlobalConsts.NotApplicable : campaign.QualityKPIDescription;
        }
    }
}