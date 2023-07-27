using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Shared
{
    public class LookupRequestDto : PagedResultRequestDto
    {
        public string Filter { get; set; }

        public Guid? CreatorId { get; set; }
        
        public LookupRequestDto()
        {
            MaxResultCount = MaxMaxResultCount;
        }
    }
}