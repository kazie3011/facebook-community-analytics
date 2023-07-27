using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;
using FacebookCommunityAnalytics.Api.Integrations.Tiktok;
using FacebookCommunityAnalytics.Api.Medias;
using FacebookCommunityAnalytics.Api.Permissions;
using FacebookCommunityAnalytics.Api.Services;
using FacebookCommunityAnalytics.Api.TikTokMCNs;
using FacebookCommunityAnalytics.Api.Tiktoks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Timing;

namespace FacebookCommunityAnalytics.Api.Groups
{
    [RemoteService(IsEnabled = false)]
    [Authorize(ApiPermissions.Groups.Default)]
    public class GroupExtendAppService : ApiAppService, IGroupExtendAppService
    {
        private readonly IGroupDomainService _groupDomainService;
        private readonly IGroupRepository _groupRepository;
        private readonly IRepository<TikTokMCN, Guid> _tikTokMcnRepository;

        public GroupExtendAppService(
            IGroupDomainService groupDomainService,
            IGroupRepository groupRepository,
            IRepository<TikTokMCN, Guid> tikTokMcnRepository)
        {
            _groupDomainService = groupDomainService;
            _groupRepository = groupRepository;
            _tikTokMcnRepository = tikTokMcnRepository;
        }

        public async Task<List<GroupDto>> CreateGroupsAsync(List<GroupCreateDto> groupCreateDtos)
        {
            var groups = ObjectMapper.Map<List<GroupCreateDto>, List<Group>>(groupCreateDtos);
            foreach (var group in groups) { group.TenantId = CurrentTenant.Id; }

            return ObjectMapper.Map<List<Group>, List<GroupDto>>(await _groupDomainService.CreateGroups(groups));
        }

        public async Task<List<GroupDto>> GetListAsync()
        {
            var groups = await _groupRepository.GetListAsync();
            var groupDtos = ObjectMapper.Map<List<Group>, List<GroupDto>>(groups.OrderBy(x => x.Name).ToList());

            return groupDtos;
        }

        public async Task<GroupDto> GetAsync(GetGroupApiRequest request)
        {
            if (request.GroupFid.IsNullOrWhiteSpace()) return null;

            var group = await _groupDomainService.GetAsync(request);
            return ObjectMapper.Map<Group, GroupDto>(group);
        }

        public async Task<List<GroupDto>> GetManyAsync(GetGroupApiRequest request)
        {
            var groups = await _groupDomainService.GetManyAsync(request);
            return ObjectMapper.Map<List<Group>, List<GroupDto>>(groups);
        }

        public Task UpdateStats(GroupStatsRequest request)
        {
            return _groupDomainService.UpdateGroupStats(request);
        }

        public Task CleanGroupStats()
        {
            return _groupDomainService.CleanGroupStats();
        }

        //TikTok
        public virtual async Task<PagedResultDto<GroupDto>> GetListTikTokAsync(GetGroupsInput input)
        {
            input.GroupSourceType = GroupSourceType.Tiktok;
            var tiktokMcnIds = new List<Guid>();
            if (input.TikTokMcnType.HasValue)
            {
                if (input.TikTokMcnType is TikTokMCNType.MCNGdl or TikTokMCNType.MCNVietNam)
                {
                    tiktokMcnIds = (await _tikTokMcnRepository.GetListAsync(x => x.MCNType == input.TikTokMcnType.Value)).Select(x => x.Id).ToList();
                }
            }

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
                input.GroupCategoryType,
                input.ContractStatus,
                tiktokMcnIds
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
                tiktokMcnIds,
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

        public async Task CreateTikTokChannelAsync(TikTokChannelCreateDto input)
        {
            var mcn = await _tikTokMcnRepository.GetAsync(input.McnId);
            var group = new Group()
            {
                Fid = input.Name,
                Name = input.Name,
                Description = input.Description,
                Url = input.Url,
                GroupCategoryType = GroupCategoryType.Unknown,
                McnId = input.McnId,
                GroupOwnershipType = mcn.MCNType == TikTokMCNType.MCNGdl ? GroupOwnershipType.GDLInternal : GroupOwnershipType.GDLExternal
            };

            await _groupRepository.InsertAsync(group);
        }

        public Task<List<TikTokChannelKpiModel>> GetTikTokChannelKpiModels(GetTikTokChannelKpiRequest request)
        {
            return _groupDomainService.GetTikTokChannelKpiModels(request);
        }

        public async Task<FileResultDto> GetChannelImage(Guid channelId)
        {
            var channel = await _groupRepository.GetAsync(channelId);
            if (channel != null && channel.ThumbnailImage.IsNotNullOrEmpty())
            {
                var extension = Path.GetExtension(channel.ThumbnailImage);

                var contentType = $"image/{extension.Replace(".", string.Empty)}";
                var rootPathChannel = GlobalConfiguration.MediaConfiguration.TiktokRootPathChannel;

                string fullPathImage;
                if (rootPathChannel.EndsWith("\\"))
                {
                    fullPathImage = $"{rootPathChannel}{channel.ThumbnailImage}";
                }
                else
                {
                    fullPathImage = $"{rootPathChannel}\\{channel.ThumbnailImage}";
                }

                if (File.Exists(fullPathImage))
                {
                    Rectangle section = new Rectangle(new Point(0, 0), new Size(130, 130));

                    Bitmap CroppedImage = CropImage(new Bitmap(fullPathImage), section);
                    await using var ms = new MemoryStream();
                    CroppedImage.Save(ms, ImageFormat.Jpeg);
                    var fileData = ms.ToArray();
                    //var fileData = await System.IO.File.ReadAllBytesAsync(fullPathImage);

                    return new FileResultDto()
                    {
                        FileData = fileData,
                        ContentType = contentType
                    };
                }
            }

            return null;
        }
        
        public Bitmap CropImage(Bitmap source, Rectangle section)
        {
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }
        
        public async Task<List<GroupStatsViewDto>> GetTopViewChannelsTwoWeek(int numberChannel)
        {
            return ObjectMapper.Map<List<GroupStatsView>, List<GroupStatsViewDto>>(await _groupDomainService.GetTopViewChannelsTwoWeek(numberChannel));
        }

        public async Task<TikTokMCNDto> GetMCNGDLTikTok()
        {
            var mcnGDLTikTok = _tikTokMcnRepository.FirstOrDefault(_ => _.MCNType == TikTokMCNType.MCNGdl);
            if (mcnGDLTikTok != null)
            {
                return ObjectMapper.Map<TikTokMCN, TikTokMCNDto>(mcnGDLTikTok);
            }
            return new TikTokMCNDto();
        }
    }
}