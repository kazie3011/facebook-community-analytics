using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.AccountProxies;
using FacebookCommunityAnalytics.Api.Accounts;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Crawl;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.TrendingDetails;
using FacebookCommunityAnalytics.Api.UncrawledPosts;
using Flurl;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Emailing.Smtp;

namespace FacebookCommunityAnalytics.Api.Services
{
    public interface ICrawlDomainService : IDomainService
    {
        Task<UncrawledPostsApiResponse> GetUnavailablePosts(UncrawledPostsApiRequest apiRequest);
        Task<UncrawledPostsApiResponse> GetUncrawledPosts(UncrawledPostsApiRequest apiRequest);
        Task<UncrawledCampaignPostsApiResponse> GetUncrawledCampaignPosts(UncrawledCampaignPostsApiRequest apiRequest);
        Task SaveCrawlResult(SaveCrawlResultApiRequest apiRequest);
        Task<SaveAutoCrawlResultApiResponse> SaveAutoCrawlResult(SaveAutoCrawlResultApiRequest apiRequest);
        Task<int> InitUncrawledPosts(DateTime fromDate, DateTime toDate, bool initNotAvailablePosts = false);
        Task<int> InitCampaignPosts();
        Task<int> InitCampaignPosts(List<string> campaignCodes);
        Task<GetUncrawledGroupUserApiResponse> GetUncrawledGroupUsers(GetUncrawledGroupUserApiRequest apiRequest);
        Task<GetUncrawledGroupApiResponse> GetUncrawledGroups(GetUncrawledGroupApiRequest apiRequest);
        Task<List<AccountProxyWithNavigationProperties>> GetAccountProxies(GetCrawlAccountProxiesRequest request);
        Task InitGroupUserCrawlTeams(List<string> source);
        Task RebindAccountProxies();
        Task UnlockCrawlAccounts(UnlockCrawlAccountRequest unlockCrawlAccountRequest);
        Task<List<AccountDto>> GetAccounts(GetAccountsRequest request);
        Task<int> InitUncrawledPartnerPosts(List<Guid> partnerIds = null, bool initNotAvailablePosts = false);
    }

    public class CrawlDomainService : BaseCrawlDomainService, ICrawlDomainService
    {
        private readonly ISmtpEmailSender _smtpEmailSender;

        private readonly IPostRepository _postRepository;
        private readonly IUncrawledPostRepository _uncrawledPostRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IAccountProxyRepository _accountProxyRepository;
        private readonly IAccountRepository _accountRepository;

        private readonly IUserDomainService _userDomainService;
        private readonly IAccountProxiesDomainService _accountProxiesDomainService;
        private readonly ICampaignDomainService _campaignDomainService;
        private readonly IRepository<TrendingDetail, Guid> _trendingDetailsRepository;

        public CrawlDomainService(
            IPostRepository postRepository,
            IUncrawledPostRepository uncrawledPostRepository,
            IGroupRepository groupRepository,
            IUserDomainService userDomainService,
            ICampaignRepository campaignRepository,
            IAccountProxyRepository accountProxyRepository,
            IAccountProxiesDomainService accountProxiesDomainService,
            IAccountRepository accountRepository,
            ISmtpEmailSender smtpEmailSender,
            ICampaignDomainService campaignDomainService,
            IRepository<TrendingDetail, Guid> trendingDetailsRepository)
        {
            _postRepository = postRepository;
            _uncrawledPostRepository = uncrawledPostRepository;
            _groupRepository = groupRepository;
            _userDomainService = userDomainService;
            _campaignRepository = campaignRepository;
            _accountProxyRepository = accountProxyRepository;
            _accountProxiesDomainService = accountProxiesDomainService;
            _accountRepository = accountRepository;
            _smtpEmailSender = smtpEmailSender;
            _campaignDomainService = campaignDomainService;
            _trendingDetailsRepository = trendingDetailsRepository;
        }

        #region AUTO CRAWL

        private PostCopyrightType ExtractCopyrightType(List<string> hashtags)
        {
            // default remake
            var copyrightType = PostCopyrightType.Remake;

            if (hashtags.IsNullOrEmpty()) return copyrightType;

            var exclusive = new[] {"#bygdl"};
            var remake = new[] {"#rmk", "#remake"};
            var copy = new[] {"#st", "#cre"};
            var via = new[] {"#via"};

            foreach (var hashtag in hashtags)
            {
                if (hashtag.IsIn(exclusive)) { return PostCopyrightType.Exclusive; }

                if (hashtag.IsIn(remake)) { return PostCopyrightType.Remake; }

                if (hashtag.IsIn(copy)) { return PostCopyrightType.Copy; }

                if (hashtag.IsIn(via)) { return PostCopyrightType.VIA; }
            }

            return copyrightType;
        }

        private PostContentType ExtractContentType(List<string> shortLinks)
        {
            var links = GetShortUrls(shortLinks);

            return links.IsNotNullOrEmpty() ? PostContentType.Affiliate : PostContentType.Seeding;
        }

        // private async Task<AppUser> ExtractUser(List<string> hashtags)
        // {
        //     if (hashtags.IsNullOrEmpty()) return null;
        //
        //     hashtags = hashtags.Where(_ => _.IsNotNullOrWhiteSpace()).Select(_ => _.ToLower().Trim()).ToList();
        //
        //     // #stXXX or #atXXX
        //     // #st = copy
        //     var userHashtag = hashtags.FirstOrDefault(_ => (_ != "#st" && _.StartsWith("#st")) || _.StartsWith("#at"));
        //     if (userHashtag.IsNullOrEmpty()) return null;
        //
        //     var userCode = userHashtag.Replace("#st", "").Replace("#at", "");
        //     if (userCode.ToIntOrDefault() <= 0) return null;
        //
        //     var userInfo = await _userDomainService.GetByCode(userCode);
        //     if (userInfo == null) { return null; }
        //
        //     return userInfo.AppUser;
        // }

        private List<string> CleanUpHashtags(List<string> hashtags)
        {
            if (hashtags.IsNullOrEmpty()) return new List<string>();

            return hashtags.Where(_ => _.IsNotNullOrWhiteSpace())
                .Select(_ => _.Replace("#", "").ToLower().Trim().Trim(':').Trim('.'))
                .ToList();
        }

        #endregion

        #region Init And Save

        public async Task InitGroupUserCrawlTeams(List<string> source)
        {
            foreach (var teamGroupItem in source)
            {
                var groupFid = teamGroupItem.Split("-").LastOrDefault()?.Trim();
                if (groupFid.IsNotNullOrEmpty())
                {
                    var group = await _groupRepository.FirstOrDefaultAsync(x => x.Fid == groupFid);
                    if (group != null)
                    {
                        var teams = teamGroupItem.Split("-").FirstOrDefault()?.Replace(" ", string.Empty).Split(",").ToList();
                        group.CrawlInfo = new GroupCrawlInfo {SeedingTeams = teams};
                        await _groupRepository.UpdateAsync(group);
                    }
                }
            }
        }

        public async Task<int> InitUncrawledPosts(DateTime fromDate, DateTime toDate, bool initNotAvailablePosts = false)
        {
            var crawlConfiguration = GlobalConfiguration.CrawlConfiguration;
            // ALWAYS INIT NEW POSTS
            var posts = new List<Post>();
            var newPosts = _postRepository
                .Where(p => p.SubmissionDateTime.HasValue && p.SubmissionDateTime.Value >= fromDate)
                .Where(p => p.CreatedDateTime == null && !p.IsNotAvailable || p.LastCrawledDateTime == null)
                .ToList();
            var toCrawlPosts = await _postRepository.GetUncrawledPosts(crawlConfiguration.IntervalHours, fromDate, toDate);

            if (initNotAvailablePosts)
            {
                var notAvailablePosts = await _postRepository.GetNotAvailablePosts(crawlConfiguration.IntervalHours, fromDate, toDate);
                posts.AddRange(notAvailablePosts);
            }
            posts = posts.Union(newPosts.Union(toCrawlPosts))
                .DistinctBy(p => p.Fid)
                .ToList();

            var current = await _uncrawledPostRepository.GetListAsync();
            var currentUrls = current.Select(c => c.Url).ToList();
            var uncrawledPosts = posts.Where(_ => !_.Url.IsIn(currentUrls))
                .Select
                (
                    post => new UncrawledPost
                    {
                        Url = post.Url,
                        PostSourceType = post.PostSourceType,
                        UpdatedAt = post.CreatedDateTime == null ? DateTime.UtcNow.AddYears(-1) : post.LastCrawledDateTime ?? DateTime.UtcNow.AddYears(-1)
                    }
                )
                .ToList();

            foreach (var partition in uncrawledPosts.Partition(100)) { await _uncrawledPostRepository.InsertManyAsync(partition, true); }

            /*
             * SEND EMAIL TO DEV to inform current status of uncrawled posts
             */
            // var subject = $"InitUncrawledPosts finish at {DateTime.Now} - Count {uncrawledPosts.Count}";
            // if (GlobalConfiguration.PartnerConfiguration.IsPartnerTool)
            // {
            //     subject = subject + " - Partner Tool";
            // }
            // var from = GlobalConfiguration.EmailConfiguration.AdminEmail;
            // var recipients = GlobalConfiguration.EmailConfiguration.Recipients;
            // if (recipients.IsNotNullOrEmpty())
            // {
            //     var to = recipients.FirstOrDefault();
            //     var ccs = recipients.Skip(1).ToList();
            //
            //     if (to != null)
            //     {
            //         var message = new MailMessage(@from, to, subject, subject) {IsBodyHtml = true};
            //         foreach (var cc in ccs)
            //         {
            //             message.CC.Add(cc);
            //         }
            //
            //         await SendUsingNetworkCredentials(message);
            //     }
            // }
            
            return uncrawledPosts.Count;
        }

        public async Task<int> InitUncrawledPartnerPosts(List<Guid> partnerIds, bool initNotAvailablePosts = false)
        {
            var crawlConfiguration = GlobalConfiguration.CrawlConfiguration;
            var now = DateTime.UtcNow;
            var (fromDate, toTime) = GetPayrollDateTime(now.Year, now.Month);

            // ALWAYS INIT NEW POSTS
            var posts = new List<Post>();
            var newPosts = _postRepository
                .Where(p => p.SubmissionDateTime.HasValue && p.SubmissionDateTime.Value >= fromDate)
                .Where(p => p.CreatedDateTime == null && !p.IsNotAvailable || p.LastCrawledDateTime == null)
                .ToList();
            var toCrawlPosts = await _postRepository.GetUncrawledPosts(crawlConfiguration.IntervalHours, fromDate, toTime);
            if (initNotAvailablePosts)
            {
                var notAvailablePosts = await _postRepository.GetNotAvailablePosts(crawlConfiguration.IntervalHours, fromDate, toTime);
                posts.AddRange(notAvailablePosts);
            }

            posts = posts.Union(newPosts.Union(toCrawlPosts))
                .WhereIf(partnerIds.IsNotNullOrEmpty(), p => p.PartnerId.HasValue && p.PartnerId.Value.IsIn(partnerIds))
                .DistinctBy(p => p.Fid)
                .ToList();
            
            var current = await _uncrawledPostRepository.GetListAsync();
            var currentUrls = current.Select(c => c.Url).ToList();
            var uncrawledPosts = posts.Where(_ => !_.Url.IsIn(currentUrls))
                .Select
                (
                    post => new UncrawledPost
                    {
                        Url = post.Url,
                        PostSourceType = post.PostSourceType,
                        UpdatedAt = post.CreatedDateTime == null ? DateTime.UtcNow.AddYears(-1) : post.LastCrawledDateTime ?? DateTime.UtcNow.AddYears(-1)
                    }
                )
                .ToList();

            foreach (var partition in uncrawledPosts.Partition(100)) { await _uncrawledPostRepository.InsertManyAsync(partition, true); }

            /*
             * SEND EMAIL TO DEV to inform current status of uncrawled posts
             */
            // var subject = $"InitUncrawledPosts - PARTNER - finish at {DateTime.Now} - Count {uncrawledPosts.Count}";
            // if (GlobalConfiguration.PartnerConfiguration.IsPartnerTool)
            // {
            //     subject = subject + " - Partner Tool";
            // }
            // var from = GlobalConfiguration.EmailConfiguration.AdminEmail;
            // var recipients = GlobalConfiguration.EmailConfiguration.Recipients;
            // if (recipients.IsNotNullOrEmpty())
            // {
            //     var to = recipients.FirstOrDefault();
            //     var ccs = recipients.Skip(1).ToList();
            //
            //     if (to != null)
            //     {
            //         var message = new MailMessage(@from, to, subject, subject) {IsBodyHtml = true};
            //         foreach (var cc in ccs)
            //         {
            //             message.CC.Add(cc);
            //         }
            //
            //         await SendUsingNetworkCredentials(message);
            //     }
            // }
            
            return uncrawledPosts.Count;
        }

        private async Task<int> DoInitCampaignPosts(List<Campaign> campaigns)
        {
            if (campaigns.IsNullOrEmpty()) { return 0; }

            var campaignIds = campaigns.Select(c => c.Id).Distinct().ToList();

            var crawlConfiguration = GlobalConfiguration.CrawlConfiguration;
            var lastCrawledDatetime = GlobalConfiguration.PartnerConfiguration.IsPartnerTool
                ? DateTime.UtcNow.AddHours(-crawlConfiguration.IntervalCampaigns)
                : DateTime.UtcNow.AddHours(-crawlConfiguration.IntervalHours);

            var posts = await _postRepository.GetListExtendAsync(campaignIds: campaignIds, lastCrawledDateTimeMax: lastCrawledDatetime);
            if (posts == null) return 0;

            foreach (var post in posts)
            {
                var newUncrawlPosts = new UncrawledPost
                {
                    Url = post.Url,
                    PostSourceType = post.PostSourceType,
                    UpdatedAt = DateTime.UtcNow.AddYears(-10)
                };

                var uncrawlPosts = await _uncrawledPostRepository.GetListAsync(u => u.Url == post.Url);
                if (uncrawlPosts.IsNotNullOrEmpty())
                {
                    foreach (var uncrawledPost in uncrawlPosts) { await _uncrawledPostRepository.HardDeleteAsync(uncrawledPost); }
                }

                await _uncrawledPostRepository.InsertAsync(newUncrawlPosts);
            }

            return posts.Count;
        }

        public async Task<int> InitCampaignPosts(List<string> campaignCodes)
        {
            var campaigns = await _campaignDomainService.GetListByIdOrCode(campaignCodes);
            return await DoInitCampaignPosts(campaigns);
        }

        public async Task<int> InitCampaignPosts()
        {
            var campaigns = await _campaignRepository.GetListAsync
            (
                c => c.Status == CampaignStatus.Started || c.Status == CampaignStatus.Hold
            );
            return await DoInitCampaignPosts(campaigns);
        }

        public async Task<SaveAutoCrawlResultApiResponse> SaveAutoCrawlResult(SaveAutoCrawlResultApiRequest apiRequest)
        {
            return await DoSaveAutoCrawlResult(apiRequest);
        }
        
        // private async Task<AppUser> ExtractUser(List<string> hashtags)
        // {
        //     if (hashtags.IsNullOrEmpty()) return null;
        //
        //     hashtags = hashtags.Where(_ => _.IsNotNullOrWhiteSpace()).Select(_ => _.ToLower().Trim()).ToList();
        //
        //     // #stXXX or #atXXX
        //     // #st = copy
        //     var userHashtag = hashtags.FirstOrDefault(_ => (_ != "#st" && _.StartsWith("#st")) || _.StartsWith("#at"));
        //     if (userHashtag.IsNullOrEmpty()) return null;
        //
        //     var userCode = userHashtag.Replace("#st", "").Replace("#at", "");
        //     if (userCode.ToIntOrDefault() <= 0) return null;
        //
        //     var userInfo = await _userDomainService.GetByCode(userCode);
        //     if (userInfo == null) { return null; }
        //
        //     return userInfo.AppUser;
        // }

        private async Task<SaveAutoCrawlResultApiResponse> DoSaveAutoCrawlResult(SaveAutoCrawlResultApiRequest apiRequest)
        {
            if (apiRequest == null) return new SaveAutoCrawlResultApiResponse();
            
            var newPosts = new ConcurrentBag<Post>();
            var updatePosts = new ConcurrentBag<Post>();
            var groups = await _groupRepository.GetListAsync();

            if (apiRequest.Items.IsNotNullOrEmpty())
            {
                var crawledItems = apiRequest.Items.DistinctBy(x => x.Url).ToList();
                foreach (var crawledItem in crawledItems)
                {
                    crawledItem.Url = FacebookHelper.GetCleanUrl(crawledItem.Url);
                }
                var existingPosts = await _postRepository.GetAsync(crawledItems.Select(_ => _.Url).ToList());
                
                var campaignHashTags = await GetCampaignHashtags();
                var campaignKeywords = await GetCampaignKeywords();
                var appUserDic = await _userDomainService.GetSeedingUserDic(crawledItems.Select(post => post.CreateFuid).Distinct().ToList());

                Parallel.ForEach
                (
                    crawledItems,
                    autoCrawlPost =>
                    {
                        PostContentType postContentType;

                        var appUser = appUserDic.Where(entry => entry.Key == autoCrawlPost.CreateFuid.Trim()).Select(entry => entry.Value).FirstOrDefault();
                        if (appUser != null)
                        {
                            postContentType = ExtractContentType(autoCrawlPost.Urls);
                        }
                        else
                        {
                            var postHashtags = CleanUpHashtags(autoCrawlPost.HashTags);
                            var campaignHashtags = campaignHashTags.SelectMany(pair => pair.Value).Distinct().ToList();
                            var campKeywords = campaignKeywords.SelectMany(pair => pair.Value).Distinct();

                            if (postHashtags.Intersect(campaignHashtags).Any() || campKeywords.Any(keyword => autoCrawlPost.Content.ToLower().Contains(keyword)))
                            {
                                postContentType = PostContentType.Contest;
                            }
                            else
                            {
                                return;
                            }
                            
                            var groupTitles = groups.Where(_ => _.Title.IsNotNullOrEmpty()).Select(_ => _.Title.Trim().Trim('!').ToLower()).ToList();
                            if (groupTitles.IsNotNullOrEmpty() && autoCrawlPost.CreatedBy.IsNotNullOrEmpty())
                            {
                                var createdBy = autoCrawlPost.CreatedBy.ToLower().Trim().Trim('!');
                                if (groupTitles.Contains(createdBy))
                                {
                                    postContentType = PostContentType.Seeding;
                                }
                            }
                        }

                        var p = existingPosts.FirstOrDefault(_ => _.Url == autoCrawlPost.Url);
                        var isNew = p is null;
                        if (isNew)
                        {
                            p = new Post
                            {
                                Status = PostStatus.AutoCrawledNew
                            };
                        }
                        else
                        {
                            p.Status = PostStatus.AutoCrawledUpdate;
                        }

                        if (!p.IsCampaignManual)
                        {
                            var campaignId = GetCampaignId(autoCrawlPost, campaignHashTags, campaignKeywords);
                            p.CampaignId = campaignId;
                        }
                        
                        if (!p.IsPostContentTypeManual)
                        {
                            p.PostContentType = postContentType;
                        }

                        p.GroupId = groups.FirstOrDefault(g => g.Fid == autoCrawlPost.GroupFid)?.Id;
                        p.AppUserId = appUser?.Id;
                        
                        p.Content = autoCrawlPost.Content.IsNotNullOrWhiteSpace() ? autoCrawlPost.Content : p.Content;
                        p.Note = InitNote(isNew, apiRequest.CrawlType);
                        p.PostSourceType = autoCrawlPost.PostSourceType;
                        p.PostCopyrightType = ExtractCopyrightType(autoCrawlPost.HashTags);

                        p.Url = FacebookHelper.GetCleanUrl(autoCrawlPost.Url).ToString();
                        p.Fid = FacebookHelper.GetGroupPostFid(p.Url);
                        p.Shortlinks = GetShortUrls(autoCrawlPost.Urls);
                        p.Hashtag = string.Join(" ", autoCrawlPost.HashTags);

                        p.LikeCount = autoCrawlPost.LikeCount;
                        p.CommentCount = autoCrawlPost.CommentCount;
                        p.ShareCount = autoCrawlPost.ShareCount;
                        p.TotalCount = autoCrawlPost.LikeCount + autoCrawlPost.CommentCount + autoCrawlPost.ShareCount;

                        p.CreatedBy = autoCrawlPost.CreatedBy;
                        p.CreatedFuid = autoCrawlPost.CreateFuid;
                        p.CreatedDateTime = autoCrawlPost.CreatedAt;
                        p.SubmissionDateTime = DateTime.UtcNow;
                        p.LastCrawledDateTime = DateTime.UtcNow;

                        p.IsNotAvailable = false;
                        p.IsValid = true;

                        if (isNew) { newPosts.Add(p); }
                        else { updatePosts.Add(p); }
                    }
                );
            }

            try
            {
                if (newPosts.IsNotNullOrEmpty()) await _postRepository.InsertManyAsync(newPosts.ToList());
                if (updatePosts.IsNotNullOrEmpty()) await _postRepository.UpdateManyAsync(updatePosts.ToList());

                var group = groups.FirstOrDefault(_ => _.Fid == apiRequest.GroupFid);
                switch (apiRequest.CrawlType)
                {
                    case CrawlType.PagePosts:
                    case CrawlType.GroupPosts:
                    {
                        if (group != null)
                        {
                            group.CrawlInfo ??= new GroupCrawlInfo();
                            group.CrawlInfo.GroupPostLastCrawledDateTime = DateTime.UtcNow;
                            await _groupRepository.UpdateAsync(group);
                        }

                        break;
                    }

                    case CrawlType.PageInfos:
                    case CrawlType.GroupInfos:
                    {
                        if (group != null)
                        {
                            group.CrawlInfo ??= new GroupCrawlInfo();
                            group.CrawlInfo.GroupInfoLastCrawledDateTime = DateTime.UtcNow;
                            await _groupRepository.UpdateAsync(group);
                        }

                        break;
                    }
                }
            }
            catch (Exception) { return new SaveAutoCrawlResultApiResponse {Success = false}; }

            return new SaveAutoCrawlResultApiResponse {Success = true};
        }

        private Guid? GetCampaignId(AutoCrawlPost autoCrawlPost, Dictionary<Campaign, IList<string>> campaignHashTags, Dictionary<Campaign, IList<string>> campaignKeywords)
        {
            Guid? campaignId = null;

            var postHashtags = CleanUpHashtags(autoCrawlPost.HashTags);
            if (postHashtags.Intersect(campaignHashTags.SelectMany(pair => pair.Value).Distinct()).Any())
            {
                var campaigns = campaignHashTags.Where(pair => pair.Value.Any(s => postHashtags.Contains(s))).Select(pair => pair.Key).ToList();
                campaigns = campaigns.OrderByDescending(campaign => campaign.LastModificationTime).ToList();
                campaignId = campaigns.FirstOrDefault()?.Id;
            }
            else if (campaignKeywords.SelectMany(pair => pair.Value).Distinct().Any(s => autoCrawlPost.Content.ToLower().Contains(s)))
            {
                var campaigns = campaignKeywords.Where(pair => pair.Value.Any(s => autoCrawlPost.Content.ToLower().Contains(s))).Select(pair => pair.Key).ToList();
                campaigns = campaigns.OrderByDescending(campaign => campaign.LastModificationTime).ToList();
                campaignId = campaigns.FirstOrDefault()?.Id;
            }

            return campaignId;
        }

        private async Task<Dictionary<Campaign, IList<string>>> GetCampaignHashtags()
        {
            var campaigns = await _campaignRepository.GetListAsync
            (
                c => c.Status == CampaignStatus.Started
            );

            campaigns = campaigns.Where(campaign => campaign.Hashtags.IsNotNullOrWhiteSpace()).ToList();
            var dataReturn = new Dictionary<Campaign, IList<string>>();
            foreach (var campaign in campaigns)
            {
                var keyWords = campaign.Hashtags.SplitHashtags().Select(s => s.ToLower()).ToList();
                dataReturn.Add(campaign, keyWords);
            }

            return dataReturn;
        }

        private async Task<Dictionary<Campaign, IList<string>>> GetCampaignKeywords()
        {
            var campaigns = await _campaignRepository.GetListAsync
            (
                c => c.Status == CampaignStatus.Started
            );

            campaigns = campaigns.Where(campaign => campaign.Keywords.IsNotNullOrWhiteSpace()).ToList();

            var dataReturn = new Dictionary<Campaign, IList<string>>();
            foreach (var campaign in campaigns)
            {
                var keyWords = campaign.Keywords.SplitKeywords().Select(s => s.ToLower()).ToList();
                dataReturn.Add(campaign, keyWords);
            }

            return dataReturn;
        }

        public async Task<UncrawledCampaignPostsApiResponse> GetUncrawledCampaignPosts(UncrawledCampaignPostsApiRequest apiRequest)
        {
            var campaigns = await _campaignRepository.GetListAsync
            (
                c => c.Status == CampaignStatus.Started
            );
            if (campaigns.IsNullOrEmpty())
            {
                return new UncrawledCampaignPostsApiResponse
                {
                    Count = 0,
                };
            }

            var campaignIds = campaigns.Select(c => c.Id).ToList();
            var crawlConfiguration = GlobalConfiguration.CrawlConfiguration;
            var posts = await _postRepository.GetListExtendAsync
                (campaignIds: campaignIds, lastCrawledDateTimeMax: DateTime.UtcNow.AddHours(-crawlConfiguration.IntervalCampaigns), maxResultCount: 100);
            if (posts.Count == 0)
            {
                return new UncrawledCampaignPostsApiResponse
                {
                    Count = 0,
                };
            }

            var dto = new UncrawledCampaignPostsApiResponse
            {
                Count = posts.Count,
                Items = posts.Select
                    (
                        _ => new UncrawledItemDto
                        {
                            PostSourceType = _.PostSourceType,
                            Url = _.Url,
                            UpdatedAt = _.CreatedDateTime == null ? DateTime.UtcNow.AddYears(-1) : _.LastCrawledDateTime ?? DateTime.UtcNow.AddYears(-1)
                        }
                    )
                    .ToList()
            };

            int accountProxyCount = (int) Math.Ceiling((double) posts.Count / 10);
            var accountProxies = await GetAccountProxies(new GetCrawlAccountProxiesRequest {AccountType = apiRequest.AccountType});
            accountProxies = accountProxies.Where
                    (properties => !properties.AccountProxy.IsCrawling)
                .OrderBy(properties => properties.AccountProxy.CrawledAt)
                .Take(accountProxyCount)
                .ToList();

            if (accountProxies.Any())
            {
                foreach (var accountProxy in accountProxies.Select(properties => properties.AccountProxy))
                {
                    accountProxy.IsCrawling = true;
                    accountProxy.CrawledAt = DateTime.UtcNow;
                }

                await _accountProxyRepository.UpdateManyAsync(accountProxies.Select(properties => properties.AccountProxy));
                var dtoAccountProxies = ObjectMapper.Map<List<AccountProxyWithNavigationProperties>, List<AccountProxyWithNavigationPropertiesDto>>(accountProxies);
                dto.AccountProxies = dtoAccountProxies;
            }
            else
            {
                dto.Count = 0;
            }

            return dto;
        }

        public async Task SaveCrawlResult(SaveCrawlResultApiRequest apiRequest)
        {
            await DoSaveCrawledPosts(apiRequest);
        }

        private async Task DoSaveCrawledPosts(SaveCrawlResultApiRequest result)
        {
            if (result.Items.IsNullOrEmpty()) return;

            var crawledPosts = result.Items.DistinctBy(_ => _.Url).ToList();
            var defaultUrls = crawledPosts.Select(_ => _.Url).ToList();
            var cleanUrls = crawledPosts.Select(_ => FacebookHelper.GetCleanUrl(_.Url).ToString()).ToList();
            var urls = defaultUrls.Concat(cleanUrls).ToArray();
            var posts = await _postRepository.GetAsync(urls);
            var campaignHashTags = await GetCampaignHashtags();
            var campaignKeywords = await GetCampaignKeywords();
            var groups = await _groupRepository.GetListAsync();

            foreach (var crawledPost in crawledPosts)
            {
                var post = posts.FirstOrDefault(_ => _.Url == crawledPost.Url || _.Url == FacebookHelper.GetCleanUrl(crawledPost.Url));
                if (post is null) continue;

                post.IsNotAvailable = crawledPost.IsNotAvailable;
                post.LastCrawledDateTime = DateTime.UtcNow;
                if (crawledPost.IsNotAvailable) continue;

                post.LikeCount = crawledPost.LikeCount;
                post.CommentCount = crawledPost.CommentCount;
                post.ShareCount = crawledPost.ShareCount;
                post.TotalCount = crawledPost.LikeCount + crawledPost.CommentCount + crawledPost.ShareCount;

                post.Content = crawledPost.Content.IsNotNullOrWhiteSpace() ? crawledPost.Content : post.Content;
                if (crawledPost.CreatedBy.IsNotNullOrWhiteSpace()) post.CreatedBy = crawledPost.CreatedBy;
                if (crawledPost.CreateFuid.IsNotNullOrWhiteSpace()) post.CreatedFuid = crawledPost.CreateFuid;

                post.Shortlinks = GetShortUrls(crawledPost.Urls);
                var postHashtags = CleanUpHashtags(crawledPost.HashTags);
                var campaignHashtags = campaignHashTags.SelectMany(pair => pair.Value).Distinct().ToList();
                var campKeywords = campaignKeywords.SelectMany(pair => pair.Value).Distinct();

                if (!post.IsPostContentTypeManual && !post.AppUserId.HasValue)
                {
                    post.PostContentType = ExtractContentType(crawledPost.Urls);
                    if (postHashtags.Intersect(campaignHashtags).Any() || campKeywords.Any(keyword => crawledPost.Content.ToLower().Contains(keyword)))
                    {
                        post.PostContentType = PostContentType.Contest;
                    }
                    
                    var groupTitles = groups.Where(_ => _.Title.IsNotNullOrEmpty()).Select(_ => _.Title.Trim().Trim('!').ToLower()).ToList();
                    if (groupTitles.IsNotNullOrEmpty() && post.CreatedBy.IsNotNullOrEmpty())
                    {
                        var createdBy = post.CreatedBy.ToLower().Trim().Trim('!');
                        if (groupTitles.Contains(createdBy))
                        {
                            post.PostContentType = PostContentType.Seeding;
                        }
                    }
                }
                
                if (!post.IsPostContentTypeManual && post.AppUserId.HasValue && post.PostContentType == PostContentType.Contest)
                {
                    post.PostContentType = PostContentType.Seeding;
                }

                if (crawledPost.CreatedAt.IsNotNullOrWhiteSpace())
                {
                    var canParse = DateTime.TryParse(crawledPost.CreatedAt, null, DateTimeStyles.AdjustToUniversal, out var createdAt);
                    if (canParse && createdAt != DateTime.MinValue) { post.CreatedDateTime = createdAt; }
                }

                post.Note = InitNote(false, CrawlType.GroupSelectivePosts);
            }

            if (posts.IsNotNullOrEmpty()) await _postRepository.UpdateManyAsync(posts, true);
        }

        #endregion

        #region Get

        public virtual async Task<UncrawledPostsApiResponse> GetUncrawledPosts(UncrawledPostsApiRequest apiRequest)
        {
            return await DoGetUncrawledPosts(apiRequest);
        }

        private async Task<UncrawledPostsApiResponse> DoGetUncrawledPosts(UncrawledPostsApiRequest apiRequest)
        {
            var count = await _uncrawledPostRepository.GetCountAsync();
            var uncrawledPosts = await _uncrawledPostRepository.GetListAsync
            (
                maxResultCount: GlobalConfiguration.CrawlConfiguration.BatchSize,
                postSourceType: apiRequest?.PostSourceType,
                sorting: "UpdatedAt asc"
            );
            var dto = new UncrawledPostsApiResponse
            {
                Count = count,
                Items = uncrawledPosts.Select
                    (
                        _ => new UncrawledItemDto
                        {
                            PostSourceType = _.PostSourceType,
                            Url = _.Url,
                            UpdatedAt = _.UpdatedAt
                        }
                    )
                    .ToList()
            };

            foreach (var uncrawledPost in uncrawledPosts) { await _uncrawledPostRepository.HardDeleteAsync(uncrawledPost); }

            var accountProxies = await GetAccountProxies(new GetCrawlAccountProxiesRequest {AccountType = apiRequest.AccountType});
            accountProxies = accountProxies.Where
                    (properties => !properties.AccountProxy.IsCrawling)
                .OrderBy(properties => properties.AccountProxy.CrawledAt)
                .Take(GlobalConfiguration.CrawlConfiguration.BatchSize / 10)
                .ToList();

            if (accountProxies.Any())
            {
                foreach (var accountProxy in accountProxies.Select(properties => properties.AccountProxy))
                {
                    accountProxy.IsCrawling = true;
                    accountProxy.CrawledAt = DateTime.UtcNow;
                }

                await _accountProxyRepository.UpdateManyAsync(accountProxies.Select(properties => properties.AccountProxy));

                var dtoAccountProxies = ObjectMapper.Map<List<AccountProxyWithNavigationProperties>, List<AccountProxyWithNavigationPropertiesDto>>(accountProxies);
                dto.AccountProxies = dtoAccountProxies;
            }
            else
            {
                dto.Count = 0;
                dto.Items = new List<UncrawledItemDto>();
            }

            return dto;
        }

        public async Task<GetUncrawledGroupUserApiResponse> GetUncrawledGroupUsers(GetUncrawledGroupUserApiRequest apiRequest)
        {
            return await DoGetUncrawledGroupUsers(apiRequest);
        }

        private async Task<GetUncrawledGroupUserApiResponse> DoGetUncrawledGroupUsers(GetUncrawledGroupUserApiRequest apiRequest)
        {
            var response = new GetUncrawledGroupUserApiResponse();

            var groups = await _groupRepository.GetListAsync(g => g.IsActive && g.GroupSourceType == GroupSourceType.Group);
            var list = groups.Where(g => g.Fid.IsNotNullOrEmpty());
            foreach (var g in list)
            {
                if (g.CrawlInfo == null || g.CrawlInfo.SeedingTeams.IsNullOrEmpty()) continue;

                var userInfos = await _userDomainService.GetUsersByOrgNames(g.CrawlInfo.SeedingTeams);
                var crawlItems = new List<UncrawledGroupUserCrawlUrlItem>();
                foreach (var info in userInfos)
                {
                    var items = info.Accounts.Where(acc => acc.IsActive && !Flurl.Url.IsValid(acc.Fid))
                        .Select
                        (
                            acc => new UncrawledGroupUserCrawlUrlItem
                            {
                                Url = $"https://www.facebook.com/groups/{g.Fid}/user/{acc.Fid}",
                                GroupName = g.Name,
                                GroupFid = g.Fid,
                                Usercode = info.Code,
                                UserId = info.AppUserId ?? Guid.Empty,
                                Fuid = acc.Fid
                            }
                        )
                        .ToList();

                    crawlItems.AddRange(items);
                }

                var groupUserCrawlItem = new UncrawledGroupUserItem
                {
                    GroupFid = g.Fid,
                    GroupName = g.Name,
                    UrlItems = crawlItems.DistinctBy(_ => _.Url).ToList()
                };

                response.Items.Add(groupUserCrawlItem);
            }

            response.Count = response.Items.Count;
            return response;
        }

        public async Task<GetUncrawledGroupApiResponse> GetUncrawledGroups(GetUncrawledGroupApiRequest apiRequest)
        {
            return await DoGetUncrawledGroups(apiRequest);
        }

        private async Task<GetUncrawledGroupApiResponse> DoGetUncrawledGroups(GetUncrawledGroupApiRequest apiRequest)
        {
            var groups = await _groupRepository.GetListAsync(_ => _.GroupSourceType == apiRequest.GroupSourceType && _.IsActive);

            var uncrawleds = new List<Group>();
            foreach (var g in groups)
            {
                if (
                    apiRequest.IgnoreTime
                    || g.CrawlInfo?.GroupPostLastCrawledDateTime == null
                    || g.CrawlInfo.GroupPostLastCrawledDateTime.Value < DateTime.UtcNow.AddHours(-2))
                {
                    uncrawleds.Add(g);
                }
            }

            return new GetUncrawledGroupApiResponse
            {
                Count = uncrawleds.Count,
                Groups = ObjectMapper.Map<List<Group>, List<GroupDto>>(uncrawleds.OrderBy(_ => _.CrawlInfo.GroupPostLastCrawledDateTime).ToList())
            };
        }

        public async Task<UncrawledPostsApiResponse> GetUnavailablePosts(UncrawledPostsApiRequest apiRequest)
        {
            if (apiRequest.FromDateTime == null && apiRequest.ToDateTime == null)
            {
                var now = DateTime.UtcNow;
                var (fromTime, toTime) = GetPayrollDateTime(now.Year, now.Month);
                apiRequest.FromDateTime = fromTime;
                apiRequest.ToDateTime = toTime;
            }

            var posts = await _postRepository.GetListAsync
            (
                isNotAvailable: true,
                createdDateTimeMin: apiRequest.FromDateTime,
                createdDateTimeMax: apiRequest.ToDateTime,
                postSourceType: apiRequest.PostSourceType
            );

            return new UncrawledPostsApiResponse
            {
                Count = posts.Count,
                Items = posts.Select
                    (
                        _ => new UncrawledItemDto
                        {
                            PostSourceType = _.PostSourceType,
                            Url = _.Url
                        }
                    )
                    .ToList()
            };
        }

        private string InitNote(bool isNew, CrawlType crawlType)
        {
            return crawlType switch
            {
                _ => $"{crawlType.ToString()} - {(isNew ? "new" : "update")}",
            };
        }

        private List<string> GetShortUrls(List<string> shortUrls)
        {
            var shortLinks = new List<string>();
            if (shortUrls.IsNotNullOrEmpty())
            {
                var validShortUrls = shortUrls
                    .Where(_ => _.IsNotNullOrEmpty())
                    .Select(_ => _.Trim())
                    .Distinct()
                    .ToList();

                foreach (var shortUrl in validShortUrls)
                {
                    shortLinks.AddRange(ParseShortUrls(shortUrl));
                }
            }

            return shortLinks.Where(_ => _.IsNotNullOrEmpty()).ToList();
        }

        /// <summary>
        /// TODOO Long test this
        /// </summary>
        /// <param name="shortUrl"></param>
        /// <returns></returns>
        private static List<string> ParseShortUrls(string shortUrl)
        {
            var cleanShortUrls = new List<string>();
            if (shortUrl.Contains(GlobalConsts.BaseAffiliateDomain)
                || shortUrl.Contains(GlobalConsts.GDLDomain)
                || shortUrl.Contains(GlobalConsts.HPDDomain)
                || shortUrl.Contains(GlobalConsts.YANDomain))
            {
                var shortLinks = shortUrl.Split(new[] {"https:"}, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var shortlink in shortLinks.Select(_ => $"https:{_}"))
                {
                    var shortlinkUrl = new Url(shortlink);

                    var root = shortlinkUrl.Root;
                    var shortKey = shortlinkUrl.Path;

                    if (shortKey.Length > GlobalConsts.ShopinessShortkeyLength + 1)
                    {
                        shortKey = shortKey.Remove(GlobalConsts.ShopinessShortkeyLength + 1);
                    }

                    cleanShortUrls.Add(Url.Combine(root, shortKey));
                }
            }

            return cleanShortUrls;
        }

        public async Task<List<AccountProxyWithNavigationProperties>> GetAccountProxies(GetCrawlAccountProxiesRequest request)
        {
            var items = await _accountProxyRepository.GetListWithNavigationPropertiesAsync();

            var acc = new List<AccountProxyWithNavigationProperties>();
            switch (request.AccountType)
            {
                case AccountType.All:
                    acc = items;
                    break;

                default:
                    acc = items.Where(_ => _.Account?.AccountType == request.AccountType).ToList();
                    break;
            }

            return acc;
        }

        #endregion

        /// <summary>
        /// Rebinding the accounts with the proxies
        /// </summary>
        public async Task RebindAccountProxies()
        {
            await _accountProxiesDomainService.RebindAccountProxies();
        }

        /// <summary>
        /// The account will be locked when being used for crawling, so unlock the account if needed.
        /// This method should be triggered from Crawler Service
        /// </summary>
        /// <param name="unlockCrawlAccountRequest"></param>
        public async Task UnlockCrawlAccounts(UnlockCrawlAccountRequest unlockCrawlAccountRequest)
        {
            if (unlockCrawlAccountRequest.AccountProxyIds.IsNullOrEmpty()) return;
            var accountProxies = _accountProxyRepository.Where(proxy => unlockCrawlAccountRequest.AccountProxyIds.Contains(proxy.Id)).ToList();
            foreach (var accountProxy in accountProxies)
            {
                accountProxy.IsCrawling = false;

                // TODOO VuD: remove this logic
                if (unlockCrawlAccountRequest.AccountType is AccountType.NETFacebookPagePost or AccountType.NETFacebookGroupPost)
                {
                    accountProxy.CrawledAt = DateTime.UtcNow;
                }
            }

            await _accountProxyRepository.UpdateManyAsync(accountProxies);
        }

        public async Task<List<AccountDto>> GetAccounts(GetAccountsRequest request)
        {
            var items = await _accountRepository.GetListAsync(isActive: true);
            items = items.Where(account => account.AccountStatus == request.AccountStatus).ToList();

            return ObjectMapper.Map<List<Account>, List<AccountDto>>(items);
        }
    }
}