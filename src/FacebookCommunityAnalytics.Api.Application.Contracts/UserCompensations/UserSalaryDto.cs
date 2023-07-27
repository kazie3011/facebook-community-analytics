using System;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public class GetUserSalariesInput : PagedResultRequestDto
    {
        public override int MaxResultCount { get; set; } = 1000;
    }
    
    public class UserSalaryDto : CreationAuditedEntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
    
    public class CreateUpdateUserSalaryDto
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}