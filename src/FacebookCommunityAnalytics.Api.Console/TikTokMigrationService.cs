using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Tiktoks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace FacebookCommunityAnalytics.Api.Console
{
    
    public class TikTokMigrationService : ITransientDependency
    {
        private readonly IRepository<TikTokMCN> _tikTokMcnRepository;
        private readonly IGroupRepository _groupRepository;

        private readonly IRepository<Tiktok> _tiktoksVideoRepository;
        public TikTokMigrationService(IRepository<TikTokMCN> tikTokMcnRepository, IGroupRepository groupRepository, IRepository<Tiktok> tiktoksVideoRepository)
        {
            _tikTokMcnRepository = tikTokMcnRepository;
            _groupRepository = groupRepository;
            _tiktoksVideoRepository = tiktoksVideoRepository;
        }

        public async Task UpdateMcns()
        {
            var mcns = await _tikTokMcnRepository.GetListAsync();
            foreach (var mcn in mcns)
            {
                if (mcn.HashTag == "gdlfamily")
                {
                    mcn.MCNType = TikTokMCNType.MCNGdl;
                }
                else
                {
                    mcn.MCNType = TikTokMCNType.MCNVietNam;
                }
            }

            await _tikTokMcnRepository.UpdateManyAsync(mcns);
        }
        public async Task UpdateGDLChannel()
        {
            var mcnGdl = await _tikTokMcnRepository.GetAsync(x => x.HashTag == "gdlfamily");
            var channels = await _groupRepository.GetListAsync(x=>x.GroupSourceType == GroupSourceType.Tiktok);
            foreach (var channel in channels)
            {
                channel.GroupOwnershipType = GroupOwnershipType.GDLInternal;
                channel.McnId = mcnGdl.Id;
            }

            await _groupRepository.UpdateManyAsync(channels);
        }

        public async Task AutoCreateChannelMCNVietNam()
        {
            var mcnVietNam =await _tikTokMcnRepository.GetListAsync(x => x.MCNType == TikTokMCNType.MCNVietNam);
            var mcnVietNamIds = mcnVietNam.Select(x => x.Id).ToList();
            var channels = await _groupRepository.GetListAsync(x=>x.GroupSourceType == GroupSourceType.Tiktok &&  x.McnId.HasValue && mcnVietNamIds.Contains(x.McnId.Value) );
            var tikTokVideos = await _tiktoksVideoRepository.GetListAsync(x => !channels.Select(c => c.Fid).Contains(x.ChannelId));
            var channelCreate = new List<Group>();
            foreach (var g in tikTokVideos.GroupBy(x=>x.ChannelId))
            {
                var first = g.FirstOrDefault();
                if (first == null)
                {
                    continue;
                }
                var mcn = mcnVietNam.FirstOrDefault(x => x.HashTag == first.Hashtag);
                if (mcn == null)
                {
                    continue;
                }
                channelCreate.Add(new Group()
                {
                    GroupOwnershipType = GroupOwnershipType.GDLExternal,
                    Fid = g.Key,
                    McnId = mcn?.Id,
                    Title = g.Key,
                    Name = g.Key,
                    GroupCategoryType = GroupCategoryType.Unknown,
                    GroupSourceType = GroupSourceType.Tiktok,
                    IsActive = true
                });
            }

            await _groupRepository.InsertManyAsync(channelCreate);
        }
    }
}