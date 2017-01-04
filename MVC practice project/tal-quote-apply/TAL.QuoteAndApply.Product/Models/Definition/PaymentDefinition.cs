using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Payment;

namespace TAL.QuoteAndApply.Product.Models.Definition
{
    public interface IPaymentDefinition
    {
        PaymentType PaymentType { get; }
    }

    public class CreditCardPaymentDefinition : IPaymentDefinition
    {
        public PaymentType PaymentType => PaymentType.CreditCard;

        public List<CreditCardType> AvailableCreditCardTypes { get; set; }
    }

    public class SuperannuationPaymentDefinition : IPaymentDefinition
    {
        public PaymentType PaymentType => PaymentType.SuperAnnuation;
    }

    public class DirectDebitPaymentDefinition : IPaymentDefinition
    {
        public PaymentType PaymentType => PaymentType.DirectDebit;
    }

    public class SelfManagementSuperFundPaymentDefinition : IPaymentDefinition
    {
        public PaymentType PaymentType => PaymentType.SelfManagedSuperFund;
    }
}
