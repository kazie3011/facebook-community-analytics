using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;

namespace FacebookCommunityAnalytics.Api.Groups
{
    public class GetGroupApiRequest
    {
        public GetGroupApiRequest()
        {
            PartnerUserIds = new List<Guid>();
        }
        private string _groupFid;

        public string GroupFid
        {
            get => _groupFid.IsNotNullOrEmpty() ? _groupFid.Trim().ToLower(): string.Empty;
            set => _groupFid = value;
        }

        public List<Guid> PartnerUserIds { get; set; }
        public GroupSourceType? GroupSourceType { get; set; }
        public GroupCategoryType? GroupCategoryType { get; set; }
        public GroupOwnershipType? GroupOwnershipType { get; set; }
        public TikTokContractStatus? ContractStatus { get; set; }
        
        public Guid? PartnerId { get; set; }
    }
}