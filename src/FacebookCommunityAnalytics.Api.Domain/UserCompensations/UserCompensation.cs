using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.UserInfos;
using FacebookCommunityAnalytics.Api.Users;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public class UserCompensation : FullAuditedEntity<Guid>
    {
        public UserCompensation()
        {
            Bonuses = new List<UserCompensationBonus>();
        }
        public Guid PayrollId { get; set; }
        public Guid UserId { get; set; }
        public string Team { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<UserCompensationBonus> Bonuses { get; set; }
        public decimal SalaryAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Description { get; set; }
    }
    
    public class UserCompensationBonus
    {
        public Guid UserId { get; set; }
        public BonusType? BonusType { get; set; }
        public decimal BonusAmount { get; set; }
        public string Description { get; set; }
        public decimal FinesAmount { get; set; }
        public string FinesDescription { get; set; }
    }
}