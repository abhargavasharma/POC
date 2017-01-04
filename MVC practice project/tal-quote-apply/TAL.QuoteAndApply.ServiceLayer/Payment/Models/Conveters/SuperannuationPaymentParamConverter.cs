using TAL.QuoteAndApply.Payment.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Payment.Models.Conveters
{
    public interface ISuperannuationPaymentParamConverter
    {
        SuperannuationPaymentParam From(ISuperAnnuationPayment payment);
        SuperAnnuationPaymentDto From(SuperannuationPaymentParam payment);
        SuperannuationPaymentParam From(ISuperAnnuationPayment payment, bool isValidForInforce);
    }

    public class SuperannuationPaymentParamConverter : ISuperannuationPaymentParamConverter
    {
        public SuperannuationPaymentParam From(ISuperAnnuationPayment payment)
        {
            return new SuperannuationPaymentParam
            {
                FundUSI = payment.FundUSI,
                FundName = payment.FundName,
                FundABN = payment.FundABN,
                MembershipNumber = payment.MembershipNumber,
                TaxFileNumber = payment.TaxFileNumber,
                FundProduct = payment.FundProduct
            };
        }

        public SuperannuationPaymentParam From(ISuperAnnuationPayment payment, bool isValidForInforce)
        {
            return new SuperannuationPaymentParam
            {
                FundUSI = payment.FundUSI,
                FundName = payment.FundName,
                FundABN = payment.FundABN,
                MembershipNumber = payment.MembershipNumber,
                TaxFileNumber = payment.TaxFileNumber,
                FundProduct = payment.FundProduct,
                IsValidForInforce = isValidForInforce
            };
        }

        public SuperAnnuationPaymentDto From(SuperannuationPaymentParam payment)
        {
            return new SuperAnnuationPaymentDto
            {
                FundUSI = payment.FundUSI,
                FundName = payment.FundName,
                FundABN = payment.FundABN,
                MembershipNumber = payment.MembershipNumber,
                TaxFileNumber = payment.TaxFileNumber,
                FundProduct = payment.FundProduct
            };
        }
    }
}