using System;
using FacebookCommunityAnalytics.Api.Core.Enums;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace FacebookCommunityAnalytics.Api.UserSalaryConfigurations
{
    public class UserSalaryConfiguration : FullAuditedEntity<Guid>
    {
        public Guid? UserId { get; set; }
        public Guid? TeamId { get; set; }
        public UserPosition UserPosition { get; set; }
        public decimal Salary { get; set; }
        public string Description { get; set; }
    }
}