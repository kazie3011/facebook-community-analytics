using FacebookCommunityAnalytics.Api.Core.Enums;
using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.UserPayrollBonuses;
using FacebookCommunityAnalytics.Api.UserPayrollCommissions;
using FacebookCommunityAnalytics.Api.Users;
using FacebookCommunityAnalytics.Api.UserWaves;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.UserPayrolls
{
    public class UserPayrollDto : FullAuditedEntityDto<Guid>
    {
        public string Code { get; set; }
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public ContentRoleType ContentRoleType { get; set; }
        public double AffiliateMultiplier { get; set; }
        public double SeedingMultiplier { get; set; }
        public string Description { get; set; }
        public decimal WaveAmount { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid? PayrollId { get; set; }
        public Guid? AppUserId { get; set; }
        
        public UserPayrollDto()
        {
            SeedingWaves = new List<UserWaveDto>();
            SeedingBonuses = new List<UserPayrollBonusDto>();
            AffiliateWaves = new List<UserWaveDto>();
            AffiliateBonuses = new List<UserPayrollBonusDto>();
            Commissions = new List<UserPayrollCommissionDto>();
            CommunityBonuses = new List<UserPayrollBonusDto>();
        }

        public AppUserDto User { get; set; }
        public UserInfoDto UserInfo { get; set; }

        public List<UserWaveDto> SeedingWaves { get; set; }
        public List<UserPayrollBonusDto> SeedingBonuses { get; set; }

        public List<UserWaveDto> AffiliateWaves { get; set; }
        public List<UserPayrollBonusDto> AffiliateBonuses { get; set; }

        public List<UserPayrollBonusDto> CommunityBonuses { get; set; }
        public List<UserPayrollCommissionDto> Commissions { get; set; }
    }
}