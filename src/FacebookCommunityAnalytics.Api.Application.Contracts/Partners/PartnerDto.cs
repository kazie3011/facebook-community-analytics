using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Partners
{
    public class PartnerDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Code { get; set; }
        public PartnerType PartnerType { get; set; }
        public CompanyInfo CompanyInfo { get; set; }
        public PersonalInfo PersonalInfo { get; set; }
        public PaymentInfo PaymentInfo { get; set; }
        public List<Comment> Comments { get; set; }
        public int TotalCampaigns { get; set; }
        public bool IsActive { get; set; }
        public List<Guid> PartnerUserIds { get; set; }
        public PartnerDto()
        {
            CompanyInfo = new CompanyInfo();
            PersonalInfo = new PersonalInfo();
            PaymentInfo = new PaymentInfo();
            Contracts = new List<ContractWithNavigationPropertiesDto>();
            Comments = new List<Comment>();
        }
        
        public List<ContractWithNavigationPropertiesDto> Contracts { get; set; }
    }
}