using System;
using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Partners
{
    public class PartnerInfo
    {
        
    }
    
    public class CompanyInfo
    {
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string TaxNumber { get; set; }
        public string Note { get; set; }
    }
    public class PersonalInfo
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Note { get; set; }
    }
    public class PaymentInfo
    {
        public string FullName { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string BankBranchName { get; set; }
        public string AccountNumber { get; set; }
        public string Note { get; set; }
    }

    public class Comment
    {
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Note { get; set; }
        public bool IsConfirmed { get; set; }
    }
}