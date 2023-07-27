using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace FacebookCommunityAnalytics.Api.AutoPosts
{
    public class AutoPostFacebook : Entity<Guid>
    {
        public string Url { get; set; }
        public int TotalLike { get; set; }
        public int CurrentLike { get; set; }
        
        public int TotalComment { get; set; }
        public int CurrentComment { get; set; }
        
        public List<string> Comments { get; set; }
        public bool IsDone { get; set; }
    }

    public class AutoPostFacebookNotDone
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public int TotalLike { get; set; }
        public int CurrentLike { get; set; }
        
        public int TotalComment { get; set; }
        public int CurrentComment { get; set; }
        
        public List<string> Comments { get; set; }
        public bool IsDone { get; set; }
        public int NeedLike  { get; set; }
        public int NeedComment { get; set; }
    }
}