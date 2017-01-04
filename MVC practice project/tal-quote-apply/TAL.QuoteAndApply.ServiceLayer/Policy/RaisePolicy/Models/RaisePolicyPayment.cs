using TAL.QuoteAndApply.ServiceLayer.Payment.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models
{
    public class RaisePolicyPayment
    {
        public string PaymentFrequency { get; set; }
        public PaymentType PaymentType { get; set; }
        public RaisePolicyDirectDebit DirectDebit { get; set; }
        public RaisePolicyCreditCard CreditCard { get; set; }
        public RaisePolicySuperannuation Superannuation { get; set; }
        public RaisePolicySelfManagedSuperFund SelfManagedSuperFund { get; set; }
    }
}