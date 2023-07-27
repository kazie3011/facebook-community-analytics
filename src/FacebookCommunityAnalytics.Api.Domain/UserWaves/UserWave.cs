using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.Payrolls;
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.UserWaves
{
    public class UserWave : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        public virtual PostContentType PostContentType { get; set; }

        public virtual WaveType WaveType { get; set; }

        public virtual int TotalPostCount { get; set; }

        public virtual int TotalReactionCount { get; set; }

        public virtual int LikeCount { get; set; }

        public virtual int CommentCount { get; set; }

        public virtual int ShareCount { get; set; }

        public virtual decimal Amount { get; set; }

        [CanBeNull]
        public virtual string Description { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? PayrollId { get; set; }

        public UserWave()
        {

        }

        public UserWave(Guid id, WaveType waveType, int totalPostCount, int totalReactionCount, int likeCount, int commentCount, int shareCount, decimal amount, string description)
        {
            Id = id;
            WaveType = waveType;
            TotalPostCount = totalPostCount;
            TotalReactionCount = totalReactionCount;
            LikeCount = likeCount;
            CommentCount = commentCount;
            ShareCount = shareCount;
            Amount = amount;
            Description = description;
        }
    }
}