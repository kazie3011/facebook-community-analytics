using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Services;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public class CompensationResponse 
    {
        public CompensationResponse()
        {
            UserCompensations = new List<UserCompensation>();
            CommunityBonuses = new List<UserCompensationBonus>();
        }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
        public List<UserCompensation> UserCompensations { get; set; }
        public List<UserCompensationBonus> CommunityBonuses { get; set; }
        public bool IsHappyDay { get; set; }

    }
}