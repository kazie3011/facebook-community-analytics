using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using FacebookCommunityAnalytics.Api.Crawl;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Integrations.Tiktok;
using FacebookCommunityAnalytics.Api.Posts;
using FacebookCommunityAnalytics.Api.Tiktoks;
using FacebookCommunityAnalytics.Api.TrendingDetails;
using Flurl;
using JetBrains.Annotations;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Services
{
    public interface ITiktokCrawlDomainService
    {
        Task<SaveChannelStatApiResponse> DoSaveChannelStats(SaveChannelStatApiRequest  apiRequest);
        Task<SaveChannelVideoResponse>   DoSaveVideos(SaveChannelVideoRequest          apiRequest);
        Task                             DoSaveThumbnailVideos(SaveChannelVideoRequest apiRequest);
        Task<SaveTiktokStatApiResponse>  DoSaveTiktokStat(SaveTiktokStatApiRequest     apiRequest);
        Task<GetTiktokHashTagsApiResponse>     GetTiktokHashTags();
        Task                             UpdateTiktokVideosState(UpdateTiktokVideosStateRequest updateTiktokVideosStateRequest);
        Task                             DoSaveMCNChannelVideoStats(CrawlMCNVideoInput          apiRequest);
        Task<List<MCNVietNamChannelDto>> SaveMCNVietNamChannel(MCNVietNamChannelApiRequest      request);
        Task<List<TrendingDetailDto>>    SaveTrendingDetails(TrendingDetailApiRequest           request);
    }

    public class TiktokCrawlDomainService : BaseCrawlDomainService, ITiktokCrawlDomainService
    {
        private readonly IGroupRepository                     _groupRepository;
        private readonly ITiktokRepository                    _tiktokRepository;
        private readonly IRepository<GroupStatsHistory, Guid> _groupStatsHistoryRepository;
        private readonly IRepository<TiktokStat, Guid>        _tiktokStatRepository;
        private readonly ICampaignRepository                  _campaignRepository;
        private readonly IRepository<TikTokMCN>               _tikTokMcnRepository;
        private readonly IRepository<TiktokVideoStat>         _tiktokVideoStatsRepository;
        private readonly IRepository<MCNVietNamChannel>       _mcnVietNamChannelsRepository;
        private readonly IRepository<TrendingDetail>          _trendingDetailRepository;

        public TiktokCrawlDomainService(
            IGroupRepository                     groupRepository,
            ITiktokRepository                    tiktokRepository,
            IRepository<GroupStatsHistory, Guid> groupStatsHistoryRepository,
            IRepository<TiktokStat, Guid>        tiktokStatRepository,
            ICampaignRepository                  campaignRepository,
            IRepository<TikTokMCN>               tikTokMcnRepository,
            IRepository<TiktokVideoStat>         tiktokVideoStatsRepository,
            IRepository<MCNVietNamChannel>       mcnVietNamChannelsRepository,
            IRepository<TrendingDetail>          trendingDetailRepository)
        {
            _groupRepository              = groupRepository;
            _tiktokRepository             = tiktokRepository;
            _groupStatsHistoryRepository  = groupStatsHistoryRepository;
            _tiktokStatRepository         = tiktokStatRepository;
            _campaignRepository           = campaignRepository;
            _tikTokMcnRepository          = tikTokMcnRepository;
            _tiktokVideoStatsRepository   = tiktokVideoStatsRepository;
            _mcnVietNamChannelsRepository = mcnVietNamChannelsRepository;
            _trendingDetailRepository     = trendingDetailRepository;
        }

        public async Task<SaveChannelStatApiResponse> DoSaveChannelStats(SaveChannelStatApiRequest apiRequest)
        {
            var response = new SaveChannelStatApiResponse { Success = true };
            var group    = await _groupRepository.FindAsync(x => x.Fid == apiRequest.ChannelId);
            if (group == null)
            {
                response.Success      = false;
                response.ErrorMessage = L[ApiDomainErrorCodes.TiktokCrawl.NoChannelFound];
            }
            else
            {
                group.Title                                  = apiRequest.Title;
                group.Stats.GroupMembers                     = apiRequest.Followers;
                group.Stats.Reactions                        = apiRequest.Likes;
                group.Description                            = apiRequest.Description;
                group.ThumbnailImage                         = DoSaveChannelImage(group.Id, apiRequest.ThumbnailImage);
                group.CrawlInfo.GroupInfoLastCrawledDateTime = apiRequest.UpdatedAt;
                await _groupRepository.UpdateAsync(group);

                // Save History
                await SaveGroupStatsHistory(apiRequest, @group);
            }

            return response;
        }

        private string DoSaveChannelImage(Guid channelId, string base64String)
        {
            var fileName        = $"{channelId}.jpg";
            var rootPathChannel = GlobalConfiguration.MediaConfiguration.TiktokRootPathChannel;
            if (!Directory.Exists(rootPathChannel))
            {
                Directory.CreateDirectory(rootPathChannel);
            }

            var fullPathImage = rootPathChannel.EndsWith("\\") ? $"{rootPathChannel}{fileName}" : $"{rootPathChannel}\\{fileName}";
            FileHelper.SaveByteArrayAsImage(fullPathImage, base64String);
            return fileName;
        }

        private async Task SaveGroupStatsHistory(SaveChannelStatApiRequest apiRequest, Group @group)
        {
            var groups      = await _groupRepository.GetListAsync(g => g.IsActive);
            var currentDate = DateTime.UtcNow.Date;
            var groupStatsHistory = await _groupStatsHistoryRepository.FirstOrDefaultAsync
                                        (history => history.GroupFid == apiRequest.ChannelId && history.CreatedAt != null && history.CreatedAt.Value == currentDate);
            if (groupStatsHistory != null)
            {
                groupStatsHistory.LastModificationTime = DateTime.UtcNow;
                groupStatsHistory.TotalInteractions    = apiRequest.Likes;
                groupStatsHistory.GroupMembers         = apiRequest.Followers;
                await _groupStatsHistoryRepository.UpdateAsync(groupStatsHistory);
            }
            else
            {
                groupStatsHistory = new GroupStatsHistory
                {
                    CreatedAt         = currentDate,
                    GroupId           = groups.FirstOrDefault(_ => _.Fid == apiRequest.ChannelId)?.Id,
                    GroupFid          = apiRequest.ChannelId,
                    GroupMembers      = apiRequest.Followers,
                    GroupSourceType   = @group.GroupSourceType,
                    TotalInteractions = apiRequest.Likes
                };
                await _groupStatsHistoryRepository.InsertAsync(groupStatsHistory);
            }
        }

        public async Task<SaveChannelVideoResponse> DoSaveVideos(SaveChannelVideoRequest apiRequest)
        {
            if (apiRequest == null) { return new SaveChannelVideoResponse { ErrorMessage = L[ApiDomainErrorCodes.TiktokCrawl.EmptyData] }; }

            var groups = await _groupRepository.GetListAsync();
            var group  = groups.FirstOrDefault(x => x.Fid == apiRequest.ChannelId);
            if (group == null) { return new SaveChannelVideoResponse { ErrorMessage = L[ApiDomainErrorCodes.TiktokCrawl.EmptyData] }; }

            await SaveTikTokVideos(apiRequest.Videos, @group.Fid);
            group.CrawlInfo.GroupPostLastCrawledDateTime = DateTime.UtcNow;
            await _groupRepository.UpdateAsync(group);
            return new SaveChannelVideoResponse();
        }

        public async Task DoSaveThumbnailVideos(SaveChannelVideoRequest apiRequest)
        {
            var videos        = apiRequest.Videos.DistinctBy(x => x.VideoId).ToList();
            var existingPosts = await _tiktokRepository.GetAsync(videos.Select(_ => _.VideoId).ToList());
            var updatePosts   = new List<Tiktok>();
            foreach (var post in existingPosts)
            {
                var video = videos.FirstOrDefault(x => x.VideoId == post.VideoId);
                if (video != null)
                {
                    post.ThumbnailImage = DoSaveVideoImage(post.Id, video.ThumbnailImage);
                    updatePosts.Add(post);
                }
            }

            await _tiktokRepository.UpdateManyAsync(updatePosts);
        }

        public async Task SaveTikTokVideos(List<TiktokVideoDto> tiktokVideoDtos, string channelId)
        {
            var updatePosts      = new List<Tiktok>();
            var videos           = tiktokVideoDtos.DistinctBy(x => x.VideoId).ToList();
            var existingPosts    = await _tiktokRepository.GetAsync(videos.Select(_ => _.VideoId).ToList());
            var campaignHashTags = await GetCampaignHashtags();
            var campaignKeywords = await GetCampaignKeywords();
            var tiktokGroups     = await _groupRepository.GetListAsync(groupSourceType: GroupSourceType.Tiktok, isActive: true);
            var newPosts         = new List<Tiktok>();
            foreach (var video in videos)
            {
                var p     = existingPosts.FirstOrDefault(_ => _.VideoId == video.VideoId);
                var isNew = p == null;
                if (isNew)
                {
                    p = new Tiktok
                    {
                        Url       = video.VideoUrl,
                        VideoId   = video.VideoId,
                        IsNew     = true,
                        ChannelId = channelId
                    };
                }

                var campaignId = GetCampaignId(video, campaignHashTags, campaignKeywords);
                if (campaignId.HasValue)
                {
                    p.CampaignId = campaignId;
                }

                p.LikeCount    = video.Like;
                p.CommentCount = video.Comment;
                p.ShareCount   = video.Share;
                p.TotalCount   = p.LikeCount + p.CommentCount + p.ShareCount;
                p.ViewCount    = video.ViewCount;
                p.Hashtag      = string.Join(" ", video.HashTags);
                p.Content      = video.Content;
                // check channel Id or video contains path of group Url
                // Example TikTok Group https://www.tiktok.com/@sunnahh__?
                // Example TikTok Video https://www.tiktok.com/@sunnahh__/video/7038875922086382850
                p.GroupId             = tiktokGroups.FirstOrDefault(_ => _.Fid == channelId)?.Id ?? tiktokGroups.FirstOrDefault(_ => video.VideoUrl.Contains((new Url(_.Url.Replace("?", "")).Path)))?.Id;
                p.CreatedDateTime     = video.CreatedAt;
                p.LastCrawledDateTime = DateTime.UtcNow;
                if (isNew)
                {
                    newPosts.Add(p);
                }
                else { updatePosts.Add(p); }
            }

            if (newPosts.IsNotNullOrEmpty()) await _tiktokRepository.InsertManyAsync(newPosts);
            if (updatePosts.IsNotNullOrEmpty()) await _tiktokRepository.UpdateManyAsync(updatePosts);
        }

        private string DoSaveVideoImage(Guid videoId, string base64String)
        {
            var fileName        = $"{videoId}.jpg";
            var rootPathChannel = GlobalConfiguration.MediaConfiguration.TiktokRootPathVideo;
            if (!Directory.Exists(rootPathChannel))
            {
                Directory.CreateDirectory(rootPathChannel);
            }

            var fullPathImage = rootPathChannel.EndsWith("\\") ? $"{rootPathChannel}{fileName}" : $"{rootPathChannel}\\{fileName}";
            FileHelper.SaveByteArrayAsImage(fullPathImage, base64String);
            return fileName;
        }

        private async Task<Dictionary<Campaign, IList<string>>> GetCampaignHashtags()
        {
            var campaigns = await _campaignRepository.GetListAsync(c => c.Status == CampaignStatus.Started);
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
            var campaigns = await _campaignRepository.GetListAsync(c => c.Status == CampaignStatus.Started);
            campaigns = campaigns.Where(campaign => campaign.Keywords.IsNotNullOrWhiteSpace()).ToList();
            var dataReturn = new Dictionary<Campaign, IList<string>>();
            foreach (var campaign in campaigns)
            {
                var keyWords = campaign.Keywords.SplitKeywords().Select(s => s.ToLower()).ToList();
                dataReturn.Add(campaign, keyWords);
            }

            return dataReturn;
        }

        private List<string> CleanUpHashtags(List<string> hashtags)
        {
            if (hashtags.IsNullOrEmpty()) return new List<string>();
            return hashtags.Where(_ => _.IsNotNullOrWhiteSpace()).Select(_ => _.ToLower().Trim().Trim(':').Trim('.')).ToList();
        }

        private Guid? GetCampaignId(TiktokVideoDto tiktokVideoDto, Dictionary<Campaign, IList<string>> campaignHashTags, Dictionary<Campaign, IList<string>> campaignKeywords)
        {
            Guid? campaignId   = null;
            var   postHashtags = CleanUpHashtags(tiktokVideoDto.HashTags).Select(s => s.Replace("#", "").Trim().ToLower()).ToList();
            if (postHashtags.Intersect(campaignHashTags.SelectMany(pair => pair.Value).Distinct()).Any())
            {
                var campaigns = campaignHashTags.Where(pair => pair.Value.Any(s => postHashtags.Contains(s))).Select(pair => pair.Key).ToList();
                campaigns  = campaigns.OrderByDescending(campaign => campaign.LastModificationTime).ToList();
                campaignId = campaigns.FirstOrDefault()?.Id;
            }
            else if (campaignKeywords.SelectMany(pair => pair.Value).Distinct().Any(s => tiktokVideoDto.Content.ToLower().Contains(s)))
            {
                var campaigns = campaignKeywords.Where(pair => pair.Value.Any(s => tiktokVideoDto.Content.ToLower().Contains(s))).Select(pair => pair.Key).ToList();
                campaigns  = campaigns.OrderByDescending(campaign => campaign.LastModificationTime).ToList();
                campaignId = campaigns.FirstOrDefault()?.Id;
            }

            return campaignId;
        }

        public async Task<SaveTiktokStatApiResponse> DoSaveTiktokStat(SaveTiktokStatApiRequest apiRequest)
        {
            var currentDateTime = DateTime.UtcNow;
            var tiktokStat      = await _tiktokStatRepository.FirstOrDefaultAsync(stat => stat.Hashtag == apiRequest.Hashtag && stat.Date == currentDateTime.Date);
            if (tiktokStat != null)
            {
                tiktokStat.Count                = apiRequest.Count;
                tiktokStat.LastModificationTime = currentDateTime;
                await _tiktokStatRepository.UpdateAsync(tiktokStat);
            }
            else
            {
                tiktokStat = new TiktokStat
                {
                    Count                = apiRequest.Count,
                    Date                 = currentDateTime.Date,
                    Hashtag              = apiRequest.Hashtag,
                    LastModificationTime = currentDateTime
                };
                await _tiktokStatRepository.InsertAsync(tiktokStat);
            }

            return new SaveTiktokStatApiResponse { Success = true };
        }

        public async Task<GetTiktokHashTagsApiResponse> GetTiktokHashTags()
        {
            var mcnTikTokHashtags = (await _tikTokMcnRepository.GetListAsync()).Select(_ => _.HashTag).ToList();
            return new GetTiktokHashTagsApiResponse
            {
                Success  = true,
                HashTags = mcnTikTokHashtags
            };
        }

        public async Task UpdateTiktokVideosState(UpdateTiktokVideosStateRequest updateTiktokVideosStateRequest)
        {
            var videos = await _tiktokRepository.GetByVideoIdsAsync(updateTiktokVideosStateRequest.VideoIds);
            foreach (var video in videos)
            {
                video.IsNew = updateTiktokVideosStateRequest.IsNew;
            }

            await _tiktokRepository.UpdateManyAsync(videos);
        }

        public async Task DoSaveMCNChannelVideoStats(CrawlMCNVideoInput apiRequest)
        {
            if (apiRequest.Hashtag.IsNullOrEmpty() || apiRequest.ChannelVideos.IsNullOrEmpty())
            {
                return;
            }

            var hashTag = apiRequest.Hashtag.Trim();
            var mcn     = await _tikTokMcnRepository.GetAsync(x => x.HashTag.Equals(hashTag));
            if (mcn != null)
            {
                foreach (var channelVideo in apiRequest.ChannelVideos)
                {
                    if (channelVideo.ChannelStat == null || channelVideo.ChannelStat.ChannelId.IsNullOrEmpty()) continue;
                    var channel = (await _groupRepository.GetQueryableAsync()).FirstOrDefault(x => x.Fid == channelVideo.ChannelStat.ChannelId);
                    if (channel == null)
                    {
                        //TODO HuyNN: Need confirm insert new channel

                        // var newChannel = await _groupRepository.InsertAsync(new Group()
                        // {
                        //     Fid = channelVideo.ChannelStat.ChannelId,
                        //     Title = channelVideo.ChannelStat.Title,
                        //     Description = channelVideo.ChannelStat.Description,
                        //     Name = channelVideo.ChannelStat.ChannelId,
                        //     Url = BuildTikTokUrl(channelVideo.ChannelStat.ChannelId),
                        //     McnId = mcn.Id,
                        //     Stats = new GroupStats()
                        //     {
                        //         GroupMembers   = channelVideo.ChannelStat.Followers,
                        //         Reactions = channelVideo.ChannelStat.Likes,
                        //     },
                        //     
                        // }, true);
                        //
                        // if (newChannel.Id == Guid.Empty)
                        // {
                        //     continue;
                        // }
                        //
                        // newChannel.ThumbnailImage = DoSaveChannelImage(newChannel.Id,channelVideo.ChannelStat.ThumbnailImage);
                        //
                        // newChannel.CrawlInfo.GroupInfoLastCrawledDateTime = channelVideo.ChannelStat.UpdatedAt;
                        //
                        // await _groupRepository.UpdateAsync(newChannel);
                        //
                        // // Save History
                        // await SaveGroupStatsHistory(channelVideo.ChannelStat, newChannel);
                    }
                    else
                    {
                        await DoSaveChannelStats(channelVideo.ChannelStat);
                    }

                    var saveVideoStatsApiRequest = new SaveChannelVideoRequest()
                    {
                        ChannelId = channelVideo.ChannelStat.ChannelId,
                        Videos    = channelVideo.TiktokVideos,
                    };
                    Hangfire.BackgroundJob.Enqueue(() => SaveTikTokVideos(channelVideo.TiktokVideos, channelVideo.ChannelStat.ChannelId));
                    Hangfire.BackgroundJob.Enqueue(() => DoSaveThumbnailVideos(saveVideoStatsApiRequest));
                    Hangfire.BackgroundJob.Enqueue(() => SaveTikTokVideoStats(mcn.HashTag, channelVideo.ChannelStat.ChannelId, channelVideo.TiktokVideos));
                }
            }
        }

        public async Task SaveTikTokVideoStats(string mcnHashtag, string channelId, List<TiktokVideoDto> tiktokVideoDtos)
        {
            if (tiktokVideoDtos.IsNullOrEmpty())
            {
                return;
            }

            var videoStatsInsert = tiktokVideoDtos.Select
                                                   (
                                                    x => new TiktokVideoStat()
                                                    {
                                                        Date      = x.CreatedAt,
                                                        Hashtag   = mcnHashtag,
                                                        ChannelId = channelId,
                                                        Like      = x.Like,
                                                        Share     = x.Share,
                                                        Comment   = x.Comment,
                                                        ViewCount = x.ViewCount,
                                                        VideoId   = x.VideoId
                                                    }
                                                   )
                                                  .ToList();
            if (videoStatsInsert.IsNotNullOrEmpty())
            {
                await _tiktokVideoStatsRepository.InsertManyAsync(videoStatsInsert);
            }
        }

        private string BuildTikTokUrl(string channelId)
        {
            return $"https://www.tiktok.com/@{channelId}";
        }

        public async Task<List<MCNVietNamChannelDto>> SaveMCNVietNamChannel(MCNVietNamChannelApiRequest request)
        {
            var mcnChannels = request.MCNVietNamChannels.Select
                                          (
                                           _ =>
                                           {
                                               _.CreatedDateTime ??= DateTime.UtcNow;
                                               return _;
                                           }
                                          )
                                     .ToList();
            var newMCNChannels = ObjectMapper.Map<List<MCNVietNamChannelDto>, List<MCNVietNamChannel>>(mcnChannels);
            foreach (var batch in newMCNChannels.Partition(100))
            {
                await _mcnVietNamChannelsRepository.InsertManyAsync(batch);
            }

            return mcnChannels;
        }

        public async Task<List<TrendingDetailDto>> SaveTrendingDetails(TrendingDetailApiRequest request)
        {
            var trendings = await _trendingDetailRepository.GetListAsync();
            var trendingDetails = request.TrendingDetails.Select
                                              (
                                               _ =>
                                               {
                                                   _.CreatedDateTime = _.CreatedDateTime?.Date ?? DateTime.UtcNow.Date;
                                                   return _;
                                               }
                                              )
                                         .ToList();
            var results = (from item in trendingDetails
                           let trending = trendings.FirstOrDefault(_ => item.CreatedDateTime != null && _.Description == item.Description && _.CreatedDateTime.Date == item.CreatedDateTime.Value.Date)
                           where trending is null
                           select item).ToList();
            var newTrendingDetails = ObjectMapper.Map<List<TrendingDetailDto>, List<TrendingDetail>>(results);
            foreach (var batch in newTrendingDetails.Partition(100))
            {
                await _trendingDetailRepository.InsertManyAsync(batch);
            }

            return results;
        }
    }
}