using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class StaffEvaluation : AuditedEntity<Guid>
    {
        public Guid? TeamId { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? CommunityId { get; set; }
        public int Month  { get; set; }
        public int Year  { get; set; }
        
        public decimal QuantityKPI { get; set; }
        public decimal QualityKPI { get; set; }
        public string QuantityKPIDescription { get; set; }
        public string QualityKPIDescription { get; set; }
        public decimal ReviewPoint { get; set; }
        public string DirectorReview { get; set; }
        public decimal TotalPoint { get; set; }
        
        public StaffEvaluationStatus StaffEvaluationStatus { get; set; }
        public string SummaryNote { get; set; }
        public decimal SaleKPIAmount { get; set; }
        public decimal BonusAmount { get; set; }
        public string BonusDescription { get; set; }
        public string AssignedTasks { get; set; }
        public decimal FinesAmount { get; set; }
        public string FinesDescription { get; set; }

        public StaffEvaluation(Guid teamId, Guid? appUserId, int year, int month)
        {
            TeamId = teamId;
            AppUserId = appUserId;
            Year = year;
            Month = month;
        }
    }
}