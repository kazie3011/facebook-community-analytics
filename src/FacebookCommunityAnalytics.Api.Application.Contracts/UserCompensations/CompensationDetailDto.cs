using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Payrolls;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public class CompensationDetailDto
    {
        public CompensationDetailDto()
        {
            TeamCompensations = new List<TeamCompensationDto>();
        }
        public PayrollDto Payroll { get; set; }
        public int TotalStaff { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal BonusAmount { get; set; }
        public List<TeamCompensationDto> TeamCompensations { get; set; }
        public bool IsHappyDay { get; set; }
        public decimal FinesAmount { get; set; }
    }

    public class TeamCompensationDto
    {
        public TeamCompensationDto()
        {
            UserCompensations = new List<UserCompensationNavigationPropertiesDto>();
        }
        public string Team { get; set; }
        
        public decimal TotalSalary { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal FinesAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<UserCompensationNavigationPropertiesDto> UserCompensations { get; set; }
    }
    
    public class CompensationAffiliateDto
    {
        public Guid? AppUserId { get; set; }
        public string Shortlink { get; set; }
        public int Click { get; set; }
        public int Conversions { get; set; }
    }
    
    public class CompensationAffiliateExport
    {
        public string Shortlink { get; set; }
        public int Click { get; set; }
        public int Conversions { get; set; }
    }
}