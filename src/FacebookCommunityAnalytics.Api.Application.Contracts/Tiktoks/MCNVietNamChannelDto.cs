using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.Tiktoks
{
    public class MCNVietNamChannelDto : AuditedEntity<Guid>
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public decimal Followers { get; set; }
        public DateTime? CreatedDateTime { get; set; }
    }

    public class MCNVietNamChannelApiRequest
    {
        public List<MCNVietNamChannelDto> MCNVietNamChannels { get; set; }
    }
}