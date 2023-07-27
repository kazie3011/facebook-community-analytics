using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Partners
{
    public class PartnerUpdateDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Code { get; set; }
        public PartnerType PartnerType { get; set; }
        public CompanyInfo CompanyInfo { get; set; }
        public PersonalInfo PersonalInfo { get; set; }
        public PaymentInfo PaymentInfo { get; set; }
        public List<Comment> Comments { get; set; }
        public bool IsActive { get; set; }
        public List<Guid> PartnerUserIds { get; set; }

        public PartnerUpdateDto()
        {
            Comments = new List<Comment>();
            PartnerUserIds = new List<Guid>();
        }
    }
}