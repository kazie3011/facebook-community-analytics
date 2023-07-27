using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FacebookCommunityAnalytics.Api.UserInfos
{
    public class UserInfoCreateDto
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
        [Required]
        public ContentRoleType ContentRoleType { get; set; } = ((ContentRoleType[])Enum.GetValues(typeof(ContentRoleType)))[0];
        public bool IsGDLStaff { get; set; }
        public bool EnablePayrollCalculation { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsSystemUser { get; set; }
        public Guid? AppUserId { get; set; }
        public List<UserInfoAccount> Accounts { get; set; }
        public UserPosition UserPosition { get; set; } = UserPosition.Unknown;
        public Guid? MainTeamId { get; set; }
        public UserInfoCreateDto()
        {
            Accounts = new List<UserInfoAccount>();
        }
    }
}