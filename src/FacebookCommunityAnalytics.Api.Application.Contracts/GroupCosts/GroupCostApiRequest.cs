using System;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.GroupCosts
{
    public class GroupCostApiRequest
    {
        public List<GroupCostDto> GroupCosts { get; set; }
    }

    public class DeleteGroupCostRequest
    {
        public List<Guid> Ids { get; set; }
    }
}