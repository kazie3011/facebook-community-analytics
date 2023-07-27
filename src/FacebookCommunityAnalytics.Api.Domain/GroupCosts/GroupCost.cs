using System;
using Volo.Abp.Domain.Entities;

namespace FacebookCommunityAnalytics.Api.GroupCosts
{
    public class GroupCost : Entity<Guid>
    {
        public string  GroupName { get; set; }
        public decimal Cost      { get; set;}
        public bool    Disable    { get; set; }
    }
}