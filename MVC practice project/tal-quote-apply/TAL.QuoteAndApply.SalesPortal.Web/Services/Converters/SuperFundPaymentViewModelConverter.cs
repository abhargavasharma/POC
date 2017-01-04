using System.Monads;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface ISuperFundPaymentViewModelConverter
    {
        SuperFundPaymentViewModel From(SuperannuationPaymentParam superannuationParam);
        SuperannuationPaymentParam From(SuperFundPaymentViewModel superFundParam);
    }

    public class SuperFundPaymentViewModelConverter : ISuperFundPaymentViewModelConverter
    {
        public SuperFundPaymentViewModel From(SuperannuationPaymentParam superannuationParam)
        {
            return superannuationParam.IsNotNull()
                ? new SuperFundPaymentViewModel
                {
                    FundUSI = superannuationParam.FundUSI,
                    FundABN = superannuationParam.FundABN,
                    FundName = superannuationParam.FundName,
                    MembershipNumber = superannuationParam.MembershipNumber,
                    TaxFileNumber = superannuationParam.TaxFileNumber,
                    FundProduct = superannuationParam.FundProduct,
                    IsValidForInforce = superannuationParam.IsValidForInforce
                }
                : new SuperFundPaymentViewModel();
        }

        public SuperannuationPaymentParam From(SuperFundPaymentViewModel superFundParam)
        {
            return new SuperannuationPaymentParam
            {
                FundUSI = superFundParam.FundUSI,
                FundABN = superFundParam.FundABN,
                FundName = superFundParam.FundName,
                MembershipNumber = superFundParam.MembershipNumber,
                TaxFileNumber = superFundParam.TaxFileNumber,
                FundProduct = superFundParam.FundProduct,
                IsValidForInforce = superFundParam.IsValidForInforce
            };
        }
    }
}