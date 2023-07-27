using System;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public class GetUserBonusConfigsInput : PagedResultRequestDto
    {
        public string FilterText { get; set; }
    }
    public class UserBonusConfigDto : FullAuditedEntityDto<Guid>
    {
        public BonusType BonusType { get; set; }
        public decimal BonusAmount { get; set; }
    }
    public class CreateUpdateUserBonusConfigDto
    {
        public BonusType BonusType { get; set; }
        public decimal BonusAmount { get; set; }
    }
}