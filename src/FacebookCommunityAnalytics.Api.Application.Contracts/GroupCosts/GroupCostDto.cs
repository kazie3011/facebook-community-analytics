using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace FacebookCommunityAnalytics.Api.GroupCosts
{
    public class GroupCostDto : EntityDto<Guid>
    {
        public string  GroupName { get; set; }
        public decimal Cost      { get; set; }
        public bool    Disable   { get; set; }
    }
}