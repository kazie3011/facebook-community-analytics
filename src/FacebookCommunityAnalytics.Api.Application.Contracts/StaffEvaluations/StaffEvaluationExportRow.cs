namespace FacebookCommunityAnalytics.Api.StaffEvaluations
{
    public class StaffEvaluationExportRow
    {
        public string Team { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Month { get; set; }
        public string TotalPoint { get; set; }

        public string QuantityKPI { get; set; }

        public string QualityKPI { get; set; }
        
        public string ReviewPoint { get; set; }
        public string DirectorReview { get; set; }
        public string StaffEvaluationStatus { get; set; }
        public string QuantityKPIDescription { get; set; }
        public string QualityKPIDescription { get; set; }
        public string SummaryNote { get; set; }
        public string SaleKPIAmount { get; set; }
        public string BonusAmount { get; set; }
        public string BonusDescription { get; set; }
        public string AssignedTasks { get; set; }
        public string FinesAmount  { get; set; }
        public string FinesDescription  { get; set; }
    }
}