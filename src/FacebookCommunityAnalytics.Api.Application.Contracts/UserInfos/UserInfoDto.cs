using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Const;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.UserInfos
{
    public class UserInfoDto : FullAuditedEntityDto<Guid>
    {
        public string Code { get; set; }
        public string IdentityNumber { get; set; }
        public string Facebook { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? JoinedDateTime { get; set; }
        public DateTime? PromotedDateTime { get; set; }
        public DateTime? LeaderPromotedDateTime { get; set; }
        public double AffiliateMultiplier { get; set; }
        public double SeedingMultiplier { get; set; }
        public ContentRoleType ContentRoleType { get; set; }
        public bool IsGDLStaff { get; set; }
        public virtual bool IsSystemUser { get; set; }
        public bool EnablePayrollCalculation { get; set; }
        public bool IsActive { get; set; }
        public Guid? AppUserId { get; set; }
        public List<UserInfoAccount> Accounts { get; set; }
        public UserPosition UserPosition { get; set; }
        public Guid? AvatarMediaId { get; set; }
        public Guid? MainTeamId { get; set; }
        public UserInfoDto()
        {
            Accounts = new List<UserInfoAccount>();
        }

        public string GetDateOfBirth()
        {
            if (DateOfBirth.HasValue)
            {
                return DateOfBirth.Value.ToString(GlobalConsts.DateFormat);
            }

            return string.Empty;
        }
    }
}