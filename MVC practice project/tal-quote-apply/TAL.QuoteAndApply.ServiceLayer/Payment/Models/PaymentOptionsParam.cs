using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Payment.Models
{
    public class PaymentOptionsParam
    {
        public bool IsSuperFundAvailable { get; set; }
        public bool IsDirectDebitAvailable { get; set; }
        public bool IsCreditCardAvailable { get; set; }
        public bool IsSelfManagedSuperFundAvailable { get; set; }

        public SuperannuationPaymentParam SuperannuationPayment { get; set; }
        public DirectDebitPaymentParam DirectDebitPayment { get; set; }
        public CreditCardPaymentParam CreditCardPayment { get; set; }
        public SelfManagedSuperFundPaymentParam SelfManagedSuperFundPayment { get; internal set; }

        public bool IsComplete { get; set; }
    }
}