using Volo.Abp.Application.Dtos;
using System;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Partners
{
    public class GetPartnersInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Code { get; set; }
        public PartnerType? PartnerType { get; set; }
        public bool? IsActive { get; set; }
        public Guid? PartnerUserId { get; set; }
        public GetPartnersInput()
        {

        }
    }
}