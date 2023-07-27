using System;
using System.ComponentModel.DataAnnotations;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.TikTokMCNs
{
    public class TikTokMCNDto : AuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string HashTag { get; set; }
        public TikTokMCNType MCNType { get; set; }
    }
    public class CreateUpdateTikTokMCNDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string HashTag { get; set; }

        public TikTokMCNType MCNType { get; set; } = TikTokMCNType.MCNVietNam;
    }

    public class GetTikTokMCNsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }
    }
}