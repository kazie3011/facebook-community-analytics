using System;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using Volo.Abp.Domain.Entities.Auditing;

namespace FacebookCommunityAnalytics.Api.Integrations.Tiki.TikiAffiliates
{
    public class TikiAffiliateConversion : AuditedEntity<Guid>
    {
        public string UtmTerm { get; set; }
        public string OrderCode { get; set; }
        public string SubOrderCode { get; set; }
        public string OfferName { get; set; }
        public string OfferId { get; set; }
        public string Status { get; set; }
        public string EventId { get; set; }
        public int ProductAmount { get; set; }
        public string ProductName { get; set; }
        public double Discount { get; set; }
        public int Quantity { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string BrandId { get; set; }
        public string BrandName { get; set; }
        public string SellerId { get; set; }
        public string SellerName { get; set; }
        public decimal CommissionFee { get; set; }
        public decimal SponsorCommissionFee { get; set; }
        public decimal NewCustomer { get; set; }
        public string DateKey { get; set; }
    }
}