using System;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public class TiktokExportRow
    {
        public int Index { get; set; }
        public string Channel { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }
        public string UID { get; set; }
        public string Fid { get; set; }
        // public string Total { get; set; }
        // public string Like { get; set; }
        // public string Comment { get; set; }
        // public string Share { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }

}