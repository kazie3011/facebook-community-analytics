using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Payrolls
{
    public class TeamBonuses
    {
        public string TeamName { get; set; }
        public PayrollBonusType PayrollBonusType { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}