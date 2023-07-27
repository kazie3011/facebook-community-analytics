using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Partners
{
    public class PartnerCreateDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Code { get; set; }
        public PartnerType PartnerType { get; set; }
        public bool IsActive { get; set; }
        public List<Guid> PartnerUserIds { get; set; }

        public PartnerCreateDto()
        {
            PartnerUserIds = new List<Guid>();
            IsActive = true;
        }
    }
}