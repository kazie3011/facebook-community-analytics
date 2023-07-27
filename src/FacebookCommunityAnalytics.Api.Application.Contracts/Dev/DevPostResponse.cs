using System.Collections.Generic;


namespace FacebookCommunityAnalytics.Api.Dev
{

    public class DevResponse<T>
    {
        public int Count { get; set; }
        public string Message { get; set; }
        public List<T> Payload { get; set; }

        public DevResponse()
        {
            Payload = new List<T>();
        }
    }
    
    
    public class DuplicateSeedingAccountModel
    {
        public string Fid { get; set; }
        public List<string> Usernames { get; set; }

        public DuplicateSeedingAccountModel()
        {
            Usernames = new List<string>();
        }
    }
    
    public class ShortLinkPost
    {
        public string Username { get; set; }
        public string Url { get; set; }
        public string ShortLink { get; set; }
    }
}
