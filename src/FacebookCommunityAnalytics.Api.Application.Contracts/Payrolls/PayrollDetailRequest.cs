using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.UserPayrollBonuses;
using FacebookCommunityAnalytics.Api.UserPayrollCommissions;
using FacebookCommunityAnalytics.Api.UserPayrolls;

namespace FacebookCommunityAnalytics.Api.Payrolls
{
    public class PayrollDetailRequest
    {
        public Guid? PayrollId { get; set; }
    }
    
    public class PayrollDetailResponse
    {
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
        public List<UserPayrollDto> UserPayrolls { get; set; }
        public List<UserPayrollBonusDto> CommunityBonuses { get; set; }
        public List<TeamBonuses> TeamBonuses { get; set; }
        public List<UserPayrollCommissionDto> Commissions { get; set; }
        
        public bool IsHappyDay { get; set; }

        public PayrollDetailResponse()
        {
            UserPayrolls = new List<UserPayrollDto>();
            CommunityBonuses = new List<UserPayrollBonusDto>();
            TeamBonuses = new List<TeamBonuses>();
            Commissions = new List<UserPayrollCommissionDto>();
        }
    }
}