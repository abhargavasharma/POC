namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class AvailablePaymentOptionsViewModel
    {
        public bool IsDirectDebitAvailable { get; set; }
        public bool IsCreditCardAvailable { get; set; }
        public bool IsSuperAvailable { get; set; }
    }
}