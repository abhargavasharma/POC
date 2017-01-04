using System.Collections.Generic;
using TAL.QuoteAndApply.Payment.Models;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Payment.Models.Conveters
{
    public interface ISelfManagedSuperFundPaymentParamConverter
    {
        SelfManagedSuperFundPaymentParam From(ISelfManagedSuperFundPayment payment, bool isValidForInforce);
        SelfManagedSuperFundPaymentDto From(SelfManagedSuperFundPaymentParam payment);
    }

    public class SelfManagedSuperFundPaymentParamConverter : ISelfManagedSuperFundPaymentParamConverter
    {
        public SelfManagedSuperFundPaymentParam From(ISelfManagedSuperFundPayment payment, bool isValidForInforce)
        {
            return new SelfManagedSuperFundPaymentParam
            {
                BSBNumber = payment.BSBNumber,
                AccountNumber = payment.AccountNumber,
                AccountName = payment.AccountName,
                IsValidForInforce = isValidForInforce
            };
        }

        public SelfManagedSuperFundPaymentDto From(SelfManagedSuperFundPaymentParam payment)
        {
            return new SelfManagedSuperFundPaymentDto
            {
                BSBNumber = payment.BSBNumber,
                AccountNumber = payment.AccountNumber,
                AccountName = payment.AccountName
            };
        }
    }
}