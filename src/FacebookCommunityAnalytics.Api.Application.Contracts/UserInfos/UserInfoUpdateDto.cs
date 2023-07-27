using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FacebookCommunityAnalytics.Api.Users;

namespace FacebookCommunityAnalytics.Api.UserInfos
{
    public class UserInfoUpdateDto
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
        public bool IsGDLStaff { get; set; }
        public bool IsSystemUser { get; set; }
        public bool EnablePayrollCalculation { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public ContentRoleType ContentRoleType { get; set; }
        public Guid? AppUserId { get; set; }
        public AppUserDto AppUser { get; set; }
        public List<UserInfoAccount> Accounts { get; set; }
        public UserPosition UserPosition { get; set; }
        public Guid? MainTeamId { get; set; }
        public UserInfoUpdateDto()
        {
            AppUser = new AppUserDto();
            Accounts = new List<UserInfoAccount>();
        }
    }
}