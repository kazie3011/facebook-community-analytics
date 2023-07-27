using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;
using System;

namespace FacebookCommunityAnalytics.Api.UserWaves
{
    public class GetUserWavesInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public WaveType? WaveType { get; set; }
        public int? TotalPostCountMin { get; set; }
        public int? TotalPostCountMax { get; set; }
        public int? TotalReactionCountMin { get; set; }
        public int? TotalReactionCountMax { get; set; }
        public int? LikeCountMin { get; set; }
        public int? LikeCountMax { get; set; }
        public int? CommentCountMin { get; set; }
        public int? CommentCountMax { get; set; }
        public int? ShareCountMin { get; set; }
        public int? ShareCountMax { get; set; }
        public decimal? AmountMin { get; set; }
        public decimal? AmountMax { get; set; }
        public string Description { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? PayrollId { get; set; }

        public GetUserWavesInput()
        {

        }
    }
}