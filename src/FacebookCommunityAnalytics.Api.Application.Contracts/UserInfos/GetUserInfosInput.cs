using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;
using System;

namespace FacebookCommunityAnalytics.Api.UserInfos
{
    public class GetUserInfosInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Code { get; set; }
        public string IdentityNumber { get; set; }
        public string Facebook { get; set; }
        public DateTime? DateOfBirthMin { get; set; }
        public DateTime? DateOfBirthMax { get; set; }
        public DateTime? JoinedDateTimeMin { get; set; }
        public DateTime? JoinedDateTimeMax { get; set; }
        public DateTime? PromotedDateTimeMin { get; set; }
        public DateTime? PromotedDateTimeMax { get; set; }
        public double? AffiliateMultiplierMin { get; set; }
        public double? AffiliateMultiplierMax { get; set; }
        public double? SeedingMultiplierMin { get; set; }
        public double? SeedingMultiplierMax { get; set; }
        public ContentRoleType? ContentRoleType { get; set; }
        public bool? IsGDLStaff { get; set; }
        public bool? IsSystemUser { get; set; }
        public bool? IsActive { get; set; }
        public bool? EnablePayrollCalculation { get; set; }
        public Guid? AppUserId { get; set; }
        public UserPosition? UserPosition { get; set; }
        public bool? HasMainTeam { get; set;}
        public GetUserInfosInput()
        {

        }
    }
}