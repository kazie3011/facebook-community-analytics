using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;
using System;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public class GetPostsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public PostContentType? PostContentType { get; set; }
        public PostCopyrightType? PostCopyrightType { get; set; }
        public string Url { get; set; }
        public string ShortUrl { get; set; }
        public int? LikeCountMin { get; set; }
        public int? LikeCountMax { get; set; }
        public int? CommentCountMin { get; set; }
        public int? CommentCountMax { get; set; }
        public int? ShareCountMin { get; set; }
        public int? ShareCountMax { get; set; }
        public int? TotalCountMin { get; set; }
        public int? TotalCountMax { get; set; }
        public string Hashtag { get; set; }
        public string Fid { get; set; }
        public bool? IsNotAvailable { get; set; }
        public bool? IsValid { get; set; }
        public PostStatus? Status { get; set; }     
        public PostSourceType? PostSourceType { get; set; }
        public string Note { get; set; }
        
        public int? ClientOffsetInMinutes { get; set; }
        public DateTime? CreatedDateTimeMin { get; set; }
        public DateTime? CreatedDateTimeMax { get; set; }
        public DateTime? LastCrawledDateTimeMin { get; set; }
        public DateTime? LastCrawledDateTimeMax { get; set; }
        public DateTime? SubmissionDateTimeMin { get; set; }
        public DateTime? SubmissionDateTimeMax { get; set; }
        
        public Guid? CategoryId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? CampaignId { get; set; }
        public Guid? PartnerId { get; set; }
        

        public GetPostsInput()
        {

        }
    }
    
    public class GetPostsInputExtend : GetPostsInput
    {
        public IEnumerable<Guid> AppUserIds { get; set; }
        public IEnumerable<Guid> GroupIds { get; set; }
        public IEnumerable<Guid> CampaignIds { get; set; }
        public RelativeDateTimeRange RelativeDateTimeRange { get; set; }
        public IEnumerable<PostSourceType> PostSourceTypes { get; set; }
    }
}