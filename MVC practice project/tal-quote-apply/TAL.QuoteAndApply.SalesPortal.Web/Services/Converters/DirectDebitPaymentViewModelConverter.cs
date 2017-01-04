using System.Monads;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IDirectDebitPaymentViewModelConverter
    {
        DirectDebitPaymentViewModel From(DirectDebitPaymentParam directDebitPaymentParam);
        DirectDebitPaymentParam From(DirectDebitPaymentViewModel directDebitPaymentParam);
    }

    public class DirectDebitPaymentViewModelConverter : IDirectDebitPaymentViewModelConverter
    {
        public DirectDebitPaymentViewModel From(DirectDebitPaymentParam directDebitPaymentParam)
        {
            return directDebitPaymentParam.IsNotNull()
                ? new DirectDebitPaymentViewModel
                {
                    AccountNumber = directDebitPaymentParam.AccountNumber,
                    AccountName = directDebitPaymentParam.AccountName,
                    BsbNumber = directDebitPaymentParam.BSBNumber,
                    IsValidForInforce = directDebitPaymentParam.IsValidForInforce
                }
                : new DirectDebitPaymentViewModel();
        }

        public DirectDebitPaymentParam From(DirectDebitPaymentViewModel directDebitPaymentParam)
        {
            return new DirectDebitPaymentParam
            {
                AccountNumber = directDebitPaymentParam.AccountNumber,
                AccountName = directDebitPaymentParam.AccountName,
                BSBNumber = directDebitPaymentParam.BsbNumber,
                IsValidForInforce = directDebitPaymentParam.IsValidForInforce
            };
        }
    }
}