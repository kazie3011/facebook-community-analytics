using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.UserWaves
{
    public class UserWaveDto : FullAuditedEntityDto<Guid>
    {
        public WaveType WaveType { get; set; }
        public int TotalPostCount { get; set; }
        public int TotalReactionCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public int ShareCount { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? PayrollId { get; set; }
    }
}