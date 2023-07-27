using Volo.Abp.Application.Dtos;
using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Groups
{
    public class GetGroupsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Title { get; set; }
        public string Fid { get; set; }
        public string Name { get; set; }
        public string AltName { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public double Point { get; set; }
        public GroupCategoryType? GroupCategoryType { get; set; }
        public GroupSourceType? GroupSourceType { get; set; }
        public GroupOwnershipType? GroupOwnershipType { get; set; }

        public TikTokContractStatus? ContractStatus { get; set; }

        public TikTokMCNType? TikTokMcnType { get; set; }
        public Guid? PartnerUserId { get; set; }

        public GetGroupsInput()
        {
        }
    }
}