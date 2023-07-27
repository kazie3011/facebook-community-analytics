using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Shared;

namespace FacebookCommunityAnalytics.Api.Groups
{
    public class GroupLookupRequestDto : LookupRequestDto
    {
        public GroupSourceType? GroupSourceType { get; set; }
    }
}