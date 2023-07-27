using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.Users
{
    public class AppOrganizationUnit : FullAuditedAggregateRoot<Guid>
    {
        public Guid? ParentId { get; set; }

        public string Code { get; set; }

        public string DisplayName { get; set; }

        public AppOrganizationUnit()
        {
        }
    }
}