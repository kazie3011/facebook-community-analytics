using System;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class StaffEvaluationDto : FullAuditedEntityDto<Guid>
    {
        public Guid? TeamId { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? CommunityId { get; set; }
        public int Month  { get; set; }
        public int Year  { get; set; }
        public decimal QuantityKPI { get; set; }
        public decimal QualityKPI { get; set; }
        public decimal ReviewPoint { get; set; }
        public string DirectorReview { get; set; }
        public decimal TotalPoint { get; set; }
        
        public string QuantityKPIDescription { get; set; }
        
        public string QualityKPIDescription { get; set; }
        public StaffEvaluationStatus StaffEvaluationStatus { get; set; }
        public string SummaryNote { get; set; }
        public decimal SaleKPIAmount { get; set; }
        public decimal BonusAmount { get; set; }
        public string BonusDescription { get; set; }
        public string AssignedTasks { get; set; }
        public decimal FinesAmount { get; set; }
        public string FinesDescription { get; set; }
    }

    public class StaffEvaluationRequest
    { 
        public int Month  { get; set; }
        public int Year  { get; set; }
        public bool IsEvaluateNoModChannel { get; set; }
        public TeamType? TeamType { get; set; }
    }
}