namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class PaymentOptionsViewModel
    {
        public bool IsSuperFundAvailable { get; set; }
        public bool IsDirectDebitAvailable { get; set; }
        public bool IsCreditCardAvailable { get; set; }
        public bool IsSelfManagedSuperFundAvailable { get; set; }
        public SuperFundPaymentViewModel SuperannuationPayment { get; set; }
        public DirectDebitPaymentViewModel DirectDebitPayment { get; set; }
        public CreditCardPaymentViewModel CreditCardPayment { get; set; }
        public SelfManagedSuperFundPaymentViewModel SelfManagedSuperFundPayment { get; set; }

        public bool IsComplete { get; set; }
    }
}