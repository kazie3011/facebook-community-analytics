using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Payrolls;
using FacebookCommunityAnalytics.Api.UserPayrollBonuses;
using FacebookCommunityAnalytics.Api.UserPayrollCommissions;
using FacebookCommunityAnalytics.Api.UserPayrolls;
using Volo.Abp.Identity;

namespace FacebookCommunityAnalytics.Api.Services
{
    public class PayrollDetail
    {
        public PayrollDetail()
        {
            UserPayrolls = new List<UserPayroll>();
            Commissions = new List<UserPayrollCommission>();
            SeedingBonuses = new List<UserPayrollBonus>();
            AffiliateBonuses = new List<UserPayrollBonus>();
            CommunityBonuses = new List<UserPayrollBonus>();
        }

        public Payroll Payroll { get; set; }
        public List<UserPayroll> UserPayrolls { get; set; }
        public List<UserPayrollCommission> Commissions { get; set; }

        public List<UserPayrollBonus> SeedingBonuses { get; set; }

        public List<UserPayrollBonus> AffiliateBonuses { get; set; }

        public List<UserPayrollBonus> CommunityBonuses { get; set; }
    }

    public class PayrollRequest
    {
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
        public bool AutoSave { get; set; }
        public bool IsTempPayroll { get; set; } = true;
        public bool IsHappyDay { get; set; }
        public bool CalculateCommunityBonus { get; set; }
        public List<Guid> UserIds { get; set; }

        public PayrollRequest()
        {
            UserIds = new List<Guid>();
        }
    }

    public class PayrollResponse
    {
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
        public List<UserPayroll> UserPayrolls { get; set; }
        public List<UserPayrollBonus> CommunityBonuses { get; set; }
        public List<TeamBonuses> TeamBonuses { get; set; }
        public List<UserPayrollCommission> Commissions { get; set; }
        public bool IsHappyDay { get; set; }

        public PayrollResponse()
        {
            UserPayrolls = new List<UserPayroll>();
            CommunityBonuses = new List<UserPayrollBonus>();
            Commissions = new List<UserPayrollCommission>();
            TeamBonuses = new List<TeamBonuses>();
        }
    }
    
    
    public class UserInfoMultiplier
    {
        public decimal AffiliateMultiplier { get; set; }
        public decimal SeedingMultiplier { get; set; }
        public int PromotionDay { get; set; }
    }

    public class TeamPerformance
    {
        public List<IdentityUser> LeaderUsers { get; set; }
        public string TeamName { get; set; }
        public decimal WaveAverageAmount { get; set; }
        public int MemberCount { get; set; }
    }

    public class BestTeamPerformance
    {
        public string OrganizationName { get; set; }
        public Guid LeaderUserId { get; set; }
        public decimal AverageTotalAmount { get; set; }
        public decimal SumTotalAmount { get; set; }
        public int StaffCount { get; set; }
    }

    public class UserAffiliateConversionModel
    {
        public Guid? AppUserId { get; set; }
        public int AffiliateCount { get; set; }
        public int ConversionCount { get; set; }
        public decimal ConversionAmount { get; set; }
    }

    public class PayrollTeamStat
    {
        public string Team { get; set; }
        public double Reaction_Avg { get; set; }
        public long Reaction_Count_Via { get; set; }
        public long Reaction_Count_Seeding { get; set; }
        public long Reaction_Count_Share { get; set; }
        public long Reaction_Count_Comment { get; set; }
    }
}