using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public class PostConvertRequest
    {
        public string Usercode { get; set; }
        
        public DateTime FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        
        public PostContentType? FromContentType { get; set; }
        public PostContentType? ToContentType { get; set; }        
        public PostCopyrightType? FromCopyrightType { get; set; }
        public PostCopyrightType? ToCopyrightType { get; set; }
    }


    public class PostConvertResponse
    {
        public int Count { get; set; }
        public List<string> Urls { get; set; }
    }
}