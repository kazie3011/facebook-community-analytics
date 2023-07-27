using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.Partners;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace FacebookCommunityAnalytics.Api.CmsGdl
{
    public interface ICmsGdlDomainService
    {
        Task<CmsGdlLandingPageModel> GetLandingPageModel();
        Task<CmsGdlCommunityGroup> GetCommunityDetails(GroupCategoryType groupCategoryType);
    }

    public class CmsGdlDomainService : BaseDomainService, ICmsGdlDomainService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IPartnerRepository _partnerRepository;

        public CmsGdlDomainService(IGroupRepository groupRepository, IPartnerRepository partnerRepository)
        {
            _groupRepository = groupRepository;
            _partnerRepository = partnerRepository;
        }

        public async Task<CmsGdlLandingPageModel> GetLandingPageModel()
        {
            var model = new CmsGdlLandingPageModel();

            const int dayPerMonth = 30;

            var groups = await _groupRepository.GetListAsync(x => x.GroupOwnershipType == GroupOwnershipType.GDLInternal || x.GroupOwnershipType == GroupOwnershipType.HappyDay);

            model.GroupCount = groups.Count(x => x.GroupSourceType == GroupSourceType.Group);
            model.PageCount = groups.Count(x => x.GroupSourceType == GroupSourceType.Page);
            model.PartnerCount = _partnerRepository.Count();

            // BRANDING COMMUNITIES
            var vnoGroupFid = "2931569376876318";
            var vnoPageFid = "110456680648297";

            var vnoPage = groups.FirstOrDefault(x => x.Fid == vnoPageFid);
            var vnoGroup = groups.FirstOrDefault(x => x.Fid == vnoGroupFid);
            if (vnoPage == null || vnoGroup == null) { return model; }

            var vnoAvgGrowthPercentage = new[] { vnoGroup.Stats?.GrowthPercent ?? 0.3m, vnoPage.Stats?.GrowthPercent ?? 0.3m }.Average();
            var vnoBrandingCommunities = new CmsGdlLandingPageModel.BrandingCommunity
            {
                Fid = vnoGroupFid,
                GroupSourceType = GroupSourceType.Group,
                Name = vnoGroup.Title,
                MemberCount = (vnoPage.Stats?.GroupMembers ?? 0) + (vnoGroup.Stats?.GroupMembers ?? 0),
                GrowthPercentagePerMonth = vnoAvgGrowthPercentage * dayPerMonth,
                TotalReactionsPerMonth = new[] { vnoPage.Stats?.TotalInteractions ?? 0, vnoGroup.Stats?.TotalInteractions ?? 0 }.Sum() * dayPerMonth,
                Url = $"https://www.facebook.com/groups/{vnoGroup.Fid}"
            };

            var dalatBrandingCommunities = groups.Where(x => x.GroupCategoryType == GroupCategoryType.Dalat).ToList();
            if (dalatBrandingCommunities.IsNullOrEmpty()) { return model; }

            var dalatAvgGrowthPercentage = dalatBrandingCommunities.Select(x => x.Stats.GrowthPercent ?? 0.3m).Average();
            var ghienDalatGroupFid = "956982674510656";
            var dalatCommunity = new CmsGdlLandingPageModel.BrandingCommunity
            {
                Fid = ghienDalatGroupFid,
                GroupSourceType = GroupSourceType.Group,
                Name = "Ghiền Đà Lạt",
                MemberCount = dalatBrandingCommunities.Sum(x => x.Stats?.GroupMembers ?? 0),
                GrowthPercentagePerMonth = dalatAvgGrowthPercentage * dayPerMonth,
                TotalReactionsPerMonth = dalatBrandingCommunities.Sum(x => x.Stats?.TotalInteractions ?? 0) * dayPerMonth,
                Url = $"https://www.facebook.com/groups/{ghienDalatGroupFid}"
            };

            model.BrandingCommunities.Add(vnoBrandingCommunities);
            model.BrandingCommunities.Add(dalatCommunity);

            // FEATURE COMMUNITY
            model.FeatureCommunities.Add
            (
                new CmsGdlFeatureCommunityGroup
                {
                    CategoryName = GroupCategoryType.FnB.ToString(),
                    GroupCategoryType = GroupCategoryType.FnB,
                }
            );
            model.FeatureCommunities.Add
            (
                new CmsGdlFeatureCommunityGroup
                {
                    CategoryName = GroupCategoryType.Beauty.ToString(),
                    GroupCategoryType = GroupCategoryType.Beauty,
                }
            );
            model.FeatureCommunities.Add
            (
                new CmsGdlFeatureCommunityGroup
                {
                    CategoryName = GroupCategoryType.TravelAndSport.ToString(),
                    GroupCategoryType = GroupCategoryType.TravelAndSport,
                }
            );
            model.FeatureCommunities.Add
            (
                new CmsGdlFeatureCommunityGroup
                {
                    CategoryName = GroupCategoryType.Dalat.ToString(),
                    GroupCategoryType = GroupCategoryType.Dalat,
                }
            );

            return model;
        }

        public async Task<CmsGdlCommunityGroup> GetCommunityDetails(GroupCategoryType groupCategoryType)
        {
            var defaultGrowthPercent = 0.3m;
            const int dayPerMonth = 30;

            var groups = await _groupRepository.GetListAsync
            (
                x => x.Point > 0
                     && (x.GroupOwnershipType == GroupOwnershipType.GDLInternal
                         || x.GroupOwnershipType == GroupOwnershipType.HappyDay)
            );

            var communities = new List<Group>();
            switch (groupCategoryType)
            {
                case GroupCategoryType.FnB:
                case GroupCategoryType.Beauty:
                case GroupCategoryType.TravelAndSport:
                case GroupCategoryType.Dalat:
                    communities = groups.Where(x => x.GroupCategoryType == groupCategoryType).ToList();
                    break;

                case GroupCategoryType.Architecture: break;
                case GroupCategoryType.General: break;
                case GroupCategoryType.HappyDay: break;

                default: throw new ArgumentOutOfRangeException(nameof(groupCategoryType), groupCategoryType, null);
            }

            if (communities.IsNullOrEmpty()) { return new CmsGdlCommunityGroup(); }

            var communityGroup = new CmsGdlCommunityGroup();
            var levelGroups = communities.OrderByDescending(x => x.Point).ToList();
            levelGroups.Reverse();
            var groupStack = new Stack<Group>(levelGroups);

            var popArray = new[] { 1, 2, 4 };
            foreach (var entry in popArray)
            {
                var levelList = new List<CmsGdlCommunityGroup.Item>();
                for (int i = 0; i < entry; i++)
                {
                    var canPop = groupStack.TryPop(out Group item);
                    if (canPop)
                    {
                        levelList.Add
                        (
                            new CmsGdlCommunityGroup.Item
                            {
                                Fid = item.Fid,
                                GroupSourceType = item.GroupSourceType,
                                Title = item.Title,
                                ShortDescription = item.Description.IsNotNullOrEmpty()? item.Description : "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua",
                                MemberCount = item.Stats?.GroupMembers ?? 0,
                                GrowthPercentagePerMonth = (item.Stats?.GrowthPercent ?? defaultGrowthPercent) * dayPerMonth,
                                TotalReactionsPerMonth = item.Stats?.TotalInteractions ?? 0 * dayPerMonth,
                                Url = GetFacebookUrl(item)
                            }
                        );
                    }
                }

                communityGroup.AllLevels.Add(levelList);
            }

            communityGroup.Level1 = communityGroup.AllLevels.FirstOrDefault();
            communityGroup.Level2 = communityGroup.AllLevels.Skip(1).FirstOrDefault() ?? new List<CmsGdlCommunityGroup.Item>();
            communityGroup.Level3 = communityGroup.AllLevels.Skip(2).FirstOrDefault() ?? new List<CmsGdlCommunityGroup.Item>();

            communityGroup.AllLevels = null;
            return communityGroup;
        }

        private string GetFacebookUrl(Group group)
        {
            switch (@group.GroupSourceType)
            {
                case GroupSourceType.Group: return $"https://www.facebook.com/groups/{@group.Fid}";
                case GroupSourceType.Page: return $"https://www.facebook.com/{@group.Fid}";
                default: return "#";
            }
        }
    }
}