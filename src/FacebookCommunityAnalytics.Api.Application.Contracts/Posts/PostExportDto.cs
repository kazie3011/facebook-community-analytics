using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public class PostExportOutput
    {
        public List<PostDetailExportRow> Rows { get; set; }
    }

    public class PostDetailExportRow
    {
        public string Group { get; set; }
        public string Author { get; set; }
        public string Url { get; set; }
        public string ShortUrl { get; set; }
        public string PostContentType { get; set; }
        public string Like { get; set; }
        public string Comment { get; set; }
        public string Share { get; set; }
        public string Total { get; set; }
        public string Hashtag { get; set; }
        public string IsNotAvailable { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? LastCrawledDateTime { get; set; }
        public DateTime? SubmissionDateTime { get; set; }
    }

    public class PostExportRow
    {
        public string Author { get; set; }
        public string Url { get; set; }
        public string Group { get; set; }
        public string PostContentType { get; set; }
        public int Total { get; set; }
        public int Like { get; set; }
        public int Comment { get; set; }
        public int Share { get; set; }
    }
}
