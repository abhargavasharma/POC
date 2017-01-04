using TAL.QuoteAndApply.ServiceLayer.Payment.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy
{
    public interface IRaisePaymentConverter
    {
        RaisePolicyPayment From(PaymentOptionsParam option);
    }

    public class RaisePaymentConverter : IRaisePaymentConverter
    {
        public RaisePolicyPayment From(PaymentOptionsParam paymentOptions)
        {
            var paymentType = new PaymentType();
            if (paymentOptions.CreditCardPayment.IsValidForInforce)
            {
                paymentType = PaymentType.CreditCard;
            }
            if (paymentOptions.DirectDebitPayment.IsValidForInforce)
            {
                paymentType = PaymentType.DirectDebit;
            }
            if (paymentOptions.SuperannuationPayment.IsValidForInforce)
            {
                paymentType = PaymentType.SuperAnnuation;
            }
            if (paymentOptions.SelfManagedSuperFundPayment.IsValidForInforce)
            {
                paymentType = PaymentType.SelfManagedSuperFund;
            }
            var retObj = new RaisePolicyPayment()
            {
                CreditCard = new RaisePolicyCreditCard()
                {
                    NameOnCard = paymentOptions.CreditCardPayment?.NameOnCard,
                    CardNumber = paymentOptions.CreditCardPayment?.CardNumber,
                    ExpiryYear = paymentOptions.CreditCardPayment?.ExpiryYear,
                    ExpiryMonth = paymentOptions.CreditCardPayment?.ExpiryMonth,
                    Token = paymentOptions.CreditCardPayment?.Token
                },
                DirectDebit = new RaisePolicyDirectDebit()
                {
                    AccountName = paymentOptions.DirectDebitPayment?.AccountName,
                    AccountNumber = paymentOptions.DirectDebitPayment?.AccountNumber,
                    BSBNumber = paymentOptions.DirectDebitPayment?.BSBNumber
                },
                Superannuation = new RaisePolicySuperannuation()
                {
                    FundABN = paymentOptions.SuperannuationPayment?.FundABN,
                    FundName = paymentOptions.SuperannuationPayment?.FundName,
                    FundProduct = paymentOptions.SuperannuationPayment?.FundProduct,
                    FundUSI = paymentOptions.SuperannuationPayment?.FundUSI,
                    MembershipNumber = paymentOptions.SuperannuationPayment?.MembershipNumber,
                    TaxFileNumber = paymentOptions.SuperannuationPayment?.TaxFileNumber
                },
                SelfManagedSuperFund = new RaisePolicySelfManagedSuperFund()
                {
                    AccountName = paymentOptions.SelfManagedSuperFundPayment?.AccountName,
                    AccountNumber = paymentOptions.SelfManagedSuperFundPayment?.AccountNumber,
                    BSBNumber = paymentOptions.SelfManagedSuperFundPayment?.BSBNumber
                },
                PaymentType = paymentType
            };
            return retObj;
        }
    }
}
