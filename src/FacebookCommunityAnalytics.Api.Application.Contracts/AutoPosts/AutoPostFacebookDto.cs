using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.AutoPosts
{
    public class AutoPostFacebookDto : EntityDto<Guid>
    {
        public string Url { get; set; }
        public int TotalLike { get; set; }
        public int CurrentLike { get; set; }
        
        public int TotalComment { get; set; }
        public int CurrentComment { get; set; }
        
        public List<string> Comments { get; set; }
        public bool IsDone { get; set; }
    }
    
    public class AutoPostFacebookNotDoneDto : AutoPostFacebookDto
    {
        public int NeedLike { get; set; }
        public int NeedComment { get; set; }
    }
}