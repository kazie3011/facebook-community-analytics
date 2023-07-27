using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using FacebookCommunityAnalytics.Api.Core.Extensions;

namespace FacebookCommunityAnalytics.Api.Posts
{
    public class PostImportInput
    {
        public PostImportInput()
        {
            Items = new List<PostImportDto>();
        }
        public List<PostImportDto> Items { get; set; }
    }

    public class PostImportDto
    {
        [Column(1)]
        public int Stt { get; set; }
        [Column(2)]
        public string AuthorName { get; set; }
        [Column(3)]
        public string Email { get; set; }
        [Column(4)]
        public string Phone { get; set; }
        [Column(5)]
        public string Url { get; set; }
        [Column(6)]
        public string Group { get; set; }
        [Column(7)]
        public string SourcePostContentType { get; set; }
        [Column(8)]
        public string SourcePostCopyrightType { get; set; }

        [Column(9)]
        public string Category { get; set; }
        [Column(10)]
        public string Leader { get; set; }

        public string GroupFid { get; set; }
        public string PostFid { get; set; }

        public PostContentType PostContentType
        {
            get
            {
                return PostContentType.Unknown;
            }
        }

        public PostCopyrightType PostCopyrightType
        {
            get
            {
                return PostCopyrightType.Unknown;
            }
        }

        public bool IsValidPost()
        {
            if (SourcePostContentType.IsNullOrEmpty()
                || SourcePostCopyrightType.IsNullOrEmpty()
                || Phone.IsNullOrEmpty()
                || Url.IsNullOrEmpty()
            )
            {
                return false;
            }

            return FacebookHelper.IsValidGroupPostUrl(Url);
        }

        public string GetPhone()
        {
            if (Phone.IsNullOrEmpty()) return string.Empty;

            var phone = Phone.Trim();
            return phone.StartsWith("0") ? phone : $"0{phone}";
        }
    }
}
