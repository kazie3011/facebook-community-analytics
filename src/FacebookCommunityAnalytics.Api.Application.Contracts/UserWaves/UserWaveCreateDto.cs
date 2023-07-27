using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace FacebookCommunityAnalytics.Api.UserWaves
{
    public class UserWaveCreateDto
    {
        [Required]
        public WaveType WaveType { get; set; } = ((WaveType[])Enum.GetValues(typeof(WaveType)))[0];
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