using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace FacebookCommunityAnalytics.Api.ScheduledPosts
{
    public class ScheduledPost : FullAuditedAggregateRoot<Guid>,IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }
        public string Content { get; set; }
        public bool IsAutoPost { get; set; }
        public DateTime? ScheduledPostDateTime { get; set; }
        public DateTime? PostedAt { get; set; }
        public string GroupIds { get; set; }
        public List<string> Images { get; set; }
        public List<string> Videos { get; set; }
        public PostContentType PostContentType { get; set; }
        public PostCopyrightType PostCopyrightType { get; set; }
        public Guid? CategoryId { get; set; }
        public string Note { get; set; }
        public bool IsPosted { get; set; }
        public string Url { get; set; }
        public Guid? AppUserId { get; set; }
    }
}
