using System;
using System.Collections.Generic;
using System.Text;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public class ExportExcelPostInput
    {
        public Guid? GroupId { get; set; }
        public string GroupFids { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public Guid? UserId { get; set; }
        public PostContentType? PostContentType { get; set; }
        public RelativeDateTimeRange RelativeDateTimeRange { get; set; }
        public Guid? OrganizationUnitId { get; set; }
        public int RequiredQuantity { get; set; }
    }
    
    public class ExportExcelPostInputExtend: GetPostsInputExtend{}

    public class ExportExcelTopReactionPostInput
    {
        public PostContentType? PostContentType { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int RequiredQuantity { get; set; }
    }
}
