using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.GroupStatsHistories;

namespace FacebookCommunityAnalytics.Api.Groups
{
    public class GroupUpdateDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Fid { get; set; }
        [Required]
        public string Name { get; set; }
        public string AltName { get; set; }
        [Required]
        public string Url { get; set; }
        public  string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        
        [Range(0,double.MaxValue)]
        public double Point { get; set; }

        public List<Guid> ModeratorIds { get; set; } = new();
        public GroupCategoryType GroupCategoryType { get; set; } = ((GroupCategoryType[])Enum.GetValues(typeof(GroupCategoryType)))[0];
        public GroupSourceType GroupSourceType { get; set; } = ((GroupSourceType[])Enum.GetValues(typeof(GroupSourceType)))[0];
        public GroupOwnershipType GroupOwnershipType { get; set; } = ((GroupOwnershipType[])Enum.GetValues(typeof(GroupOwnershipType)))[0];
        public GroupStatsDto Stats { get; set; }
        public TikTokTargetDto TikTokTarget { get; set; }
        public TikTokContractStatus ContractStatus { get; set; }

        public Guid? McnId { get; set; }
        public string ThumbnailImage { get; set; }
        public string TikTokUserId { get; set; }
        public GroupUpdateDto()
        {
            TikTokTarget = new TikTokTargetDto();
        }
    }
}