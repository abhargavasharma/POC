using System.Monads;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface ISelfManagedSuperFundPaymentViewModelConverter
    {
        SelfManagedSuperFundPaymentViewModel From(SelfManagedSuperFundPaymentParam selfManagedSuperFundPaymentParam);
        SelfManagedSuperFundPaymentParam From(SelfManagedSuperFundPaymentViewModel selfManagedSuperFundPaymentParam);
    }

    public class SelfManagedSuperFundPaymentViewModelConverter : ISelfManagedSuperFundPaymentViewModelConverter
    {
        public SelfManagedSuperFundPaymentViewModel From(SelfManagedSuperFundPaymentParam selfManagedSuperFundPaymentParam)
        {
            return selfManagedSuperFundPaymentParam.IsNotNull()
                ? new SelfManagedSuperFundPaymentViewModel
                {
                    AccountNumber = selfManagedSuperFundPaymentParam.AccountNumber,
                    AccountName = selfManagedSuperFundPaymentParam.AccountName,
                    BSBNumber = selfManagedSuperFundPaymentParam.BSBNumber,
                    IsValidForInforce = selfManagedSuperFundPaymentParam.IsValidForInforce
                }
                : new SelfManagedSuperFundPaymentViewModel();
        }

        public SelfManagedSuperFundPaymentParam From(SelfManagedSuperFundPaymentViewModel directDebitPaymentParam)
        {
            return new SelfManagedSuperFundPaymentParam
            {
                AccountNumber = directDebitPaymentParam.AccountNumber,
                AccountName = directDebitPaymentParam.AccountName,
                BSBNumber = directDebitPaymentParam.BSBNumber,
                IsValidForInforce = directDebitPaymentParam.IsValidForInforce
            };
        }
    }
}