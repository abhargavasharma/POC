using System.Collections.Generic;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class PurchaseAndPremiumResponse
    {
        public decimal TotalPremium { get; set; }
        public string PaymentFrequency { get; set; }
        public IEnumerable<PurchaseResponse> RiskPurchaseResponses { get; set; }
    }

    public class PurchaseResponse : PurchaseRequest
    {
        public int RiskId { get; set; }
    }
}