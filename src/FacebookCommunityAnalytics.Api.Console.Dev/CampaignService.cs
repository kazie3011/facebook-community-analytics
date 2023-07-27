using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Campaigns;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Posts;
using Volo.Abp.DependencyInjection;

namespace FacebookCommunityAnalytics.Api.Console.Dev
{
    public class CampaignService : LoggerService, ITransientDependency
    {
        public IPostRepository        PostRepository        { get; set; }
        public ICampaignRepository    CampaignRepository    { get; set; }
        public IGroupRepository       GroupRepository       { get; set; }
        public ICampaignDomainService CampaignDomainService { get; set; }

        public async Task RemapCampaignPostsByHashtags(string campaignCode, params string[] hashtags)
        {
            if (hashtags.IsNullOrEmpty()) return;
            if (campaignCode.IsNullOrWhiteSpace()) return;
            var tags = hashtags.Select(x => x.ToLower().Trim()).Distinct().ToList();
            campaignCode = campaignCode.ToLower().Trim();
            var oldPosts = await PostRepository.GetListAsync(createdDateTimeMin: DateTime.Now.AddMonths(-6));
            var posts    = oldPosts.Where(x => x.Hashtag.IsNotNullOrEmpty() && tags.Contains(x.Hashtag.ToLower().Trim())).ToList();
            if (posts.IsNullOrEmpty()) return;
            Trace($"RemapCampaignPostsByHashtags - Found {posts}");
            var camp = await CampaignRepository.FindAsync(x => x.Code.ToLower().Trim() == campaignCode);
            if (camp is null) return;
            foreach (var post in posts)
            {
                post.CampaignId = camp.Id;
            }

            await PostRepository.UpdateManyAsync(posts);
        }

        public async Task RemapCampaignPostsByKeywords(string campaignCode, params string[] keywords)
        {
            if (keywords.IsNullOrEmpty()) return;
            if (campaignCode.IsNullOrWhiteSpace()) return;
            var words = keywords.Select(x => x.ToLower().Trim()).Distinct().ToList();
            campaignCode = campaignCode.ToLower().Trim();
            var oldPosts = await PostRepository.GetListAsync(createdDateTimeMin: DateTime.Now.AddMonths(-6));
            var posts    = oldPosts.Where(x => x.Content.IsNotNullOrEmpty() && words.Intersect(x.Content.ToLower().Trim().SplitClean(' ')).Any()).ToList();
            if (posts.IsNullOrEmpty()) return;
            Trace($"RemapCampaignPostsByKeywords - Found {posts}");
            var camp = await CampaignRepository.FindAsync(x => x.Code.ToLower().Trim() == campaignCode);
            if (camp is null) return;
            foreach (var post in posts)
            {
                post.CampaignId = camp.Id;
            }

            await PostRepository.UpdateManyAsync(posts);
        }

        public async Task UpdateCampaignStatus(string campaignCode)
        {
            await CampaignDomainService.UpdateStatus(campaignCode);
        }

        public async Task UpdateCampaignStatuses()
        {
            await CampaignDomainService.UpdateStatuses();
        }

        public async Task UpdateCampaignPostCount()
        {
            var campaigns = await CampaignRepository.GetListAsync();
            foreach (var campaign in campaigns)
            {
                await CampaignDomainService.UpdateCampaignPostCount(campaign.Id);
            }
        }

        public async Task RemapCampaignPosts()
        {
            var campaigns = await CampaignRepository.GetListAsync();
            var posts     = await PostRepository.GetListExtendAsync(campaignIds: campaigns.Select(_ => _.Id));
            posts     = posts.Where(_ => _.PostSourceType is PostSourceType.Group or PostSourceType.Page).ToList();
            campaigns = campaigns.Where(campaign => campaign.Hashtags.IsNotNullOrWhiteSpace()).ToList();
            var groups           = await GroupRepository.GetListAsync(groupSourceType: GroupSourceType.Page);
            var campaignHashTags = new Dictionary<Campaign, IList<string>>();
            foreach (var campaign in campaigns)
            {
                var keyWords = campaign.Hashtags.SplitHashtags().Select(s => s.ToLower()).ToList();
                campaignHashTags.Add(campaign, keyWords);
            }

            var campaignKeyWords = new Dictionary<Campaign, IList<string>>();
            foreach (var campaign in campaigns)
            {
                var keyWords = campaign.Keywords.SplitKeywords().Select(s => s.ToLower()).ToList();
                campaignKeyWords.Add(campaign, keyWords);
            }

            var campaignHashtags = campaignHashTags.SelectMany(pair => pair.Value).Distinct().ToList();
            var campKeywords     = campaignKeyWords.SelectMany(pair => pair.Value).Distinct().ToList();
            foreach (var post in posts)
            {
                try
                {
                    if (!post.IsPostContentTypeManual)
                    {
                        var postHashtags = new List<string>();
                        if (post.Hashtag is not null)
                        {
                            postHashtags = post.Hashtag.Split('#', StringSplitOptions.RemoveEmptyEntries).Select(_ => _.ToLower().Trim()).ToList();
                        }

                        if (!post.AppUserId.HasValue
                         && postHashtags.IsNotNullOrEmpty()
                         && (postHashtags.Intersect(campaignHashtags).Any() || campKeywords.Any(keyword => post.Content.ToLower().Contains(keyword))))
                        {
                            post.PostContentType = PostContentType.Contest;
                        }

                        var groupTitles = groups.Where(_ => _.Title.IsNotNullOrEmpty()).Select(_ => _.Title.Trim().Trim('!').ToLower()).ToList();
                        if (groupTitles.IsNotNullOrEmpty() && post.CreatedBy.IsNotNullOrEmpty())
                        {
                            var createdBy  = post.CreatedBy.ToLower().Trim().Trim('!');
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
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e);
                    throw;
                }
            }

            if (posts.IsNotNullOrEmpty()) await PostRepository.UpdateManyAsync(posts, true);
        }
    }
}