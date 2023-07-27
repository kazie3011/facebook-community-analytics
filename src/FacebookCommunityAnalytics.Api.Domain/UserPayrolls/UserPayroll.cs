using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.Users;
using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.UserPayrollBonuses;
using FacebookCommunityAnalytics.Api.UserPayrollCommissions;
using FacebookCommunityAnalytics.Api.UserWaves;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.UserPayrolls
{
    public class UserPayroll : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [CanBeNull]
        public virtual string Code { get; set; }

        [CanBeNull]
        public virtual string OrganizationId { get; set; }
        [CanBeNull]
        public virtual string OrganizationName { get; set; }

        public virtual ContentRoleType ContentRoleType { get; set; }

        public virtual double AffiliateMultiplier { get; set; }

        public virtual double SeedingMultiplier { get; set; }

        [CanBeNull]
        public virtual string Description { get; set; }

        public virtual decimal WaveAmount { get; set; }

        public virtual decimal BonusAmount { get; set; }

        public virtual decimal TotalAmount { get; set; }
        public Guid? PayrollId { get; set; }
        public Guid? AppUserId { get; set; }

        public UserPayroll()
        {
            SeedingWaves = new List<UserWave>();
            SeedingBonuses = new List<UserPayrollBonus>();
            AffiliateWaves = new List<UserWave>();
            AffiliateBonuses = new List<UserPayrollBonus>();
            Commissions = new List<UserPayrollCommission>();
            CommunityBonuses = new List<UserPayrollBonus>();
        }

        public UserPayroll(Guid id, string code, string organizationId, ContentRoleType contentRoleType, double affiliateMultiplier, double seedingMultiplier, string description, decimal waveAmount, decimal bonusAmount, decimal totalAmount)
        {
            Id = id;
            Code = code;
            OrganizationId = organizationId;
            ContentRoleType = contentRoleType;
            AffiliateMultiplier = affiliateMultiplier;
            SeedingMultiplier = seedingMultiplier;
            Description = description;
            WaveAmount = waveAmount;
            BonusAmount = bonusAmount;
            TotalAmount = totalAmount;

            SeedingWaves = new List<UserWave>();
            SeedingBonuses = new List<UserPayrollBonus>();
            AffiliateWaves = new List<UserWave>();
            AffiliateBonuses = new List<UserPayrollBonus>();
            Commissions = new List<UserPayrollCommission>();
            CommunityBonuses = new List<UserPayrollBonus>();
        }

        public AppUser User { get; set; }
        public UserInfo UserInfo { get; set; }
        
        public List<UserWave> SeedingWaves { get; set; }
        public List<UserPayrollBonus> SeedingBonuses { get; set; }

        public List<UserWave> AffiliateWaves { get; set; }
        public List<UserPayrollBonus> AffiliateBonuses { get; set; }

        public List<UserPayrollBonus> CommunityBonuses { get; set; }
        public List<UserPayrollCommission> Commissions { get; set; }
        public AffiliateConversionStatsModel AffiliateConversionStats { get; set; }
    }
    public class AffiliateConversionStatsModel
    {
        public string OrganizationName { get; set; }
        public Guid LeaderUserId { get; set; }
        public int TotalClick { get; set; }
        public int TotalConversion { get; set; }
        public double ConversionRate { get; set; }
        public int StaffCount { get; set; }
        
    }
}