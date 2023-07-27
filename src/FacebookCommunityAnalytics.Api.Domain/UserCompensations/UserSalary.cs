using System;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public class UserSalary : AuditedEntity<Guid>
    {
        public Guid? UserId { get; set; }
        public Guid? TeamId { get; set; }
        public UserPosition UserPosition { get; set; }
        public decimal Salary { get; set; }
        public string Description { get; set; }
    }
}