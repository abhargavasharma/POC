using System.Collections.Generic;
using TAL.QuoteAndApply.Payment.Models;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Payment.Models.Conveters
{
    public interface IDirectDebitPaymentParamConverter
    {
        DirectDebitPaymentParam From(IDirectDebitPayment payment, bool isValidForInforce);
        DirectDebitPaymentDto From(DirectDebitPaymentParam payment);
    }

    public class DirectDebitPaymentParamConverter : IDirectDebitPaymentParamConverter
    {
        public DirectDebitPaymentParam From(IDirectDebitPayment payment, bool isValidForInforce)
        {
            return new DirectDebitPaymentParam
            {
                BSBNumber = payment.BSBNumber,
                AccountNumber = payment.AccountNumber,
                AccountName = payment.AccountName,
                IsValidForInforce = isValidForInforce
            };
        }

        public DirectDebitPaymentDto From(DirectDebitPaymentParam payment)
        {
            return new DirectDebitPaymentDto
            {
                BSBNumber = payment.BSBNumber,
                AccountNumber = payment.AccountNumber,
                AccountName = payment.AccountName
            };
        }
    }
}