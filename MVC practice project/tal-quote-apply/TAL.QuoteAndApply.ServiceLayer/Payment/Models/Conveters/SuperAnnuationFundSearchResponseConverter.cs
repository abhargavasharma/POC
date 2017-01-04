using TAL.QuoteAndApply.DataModel.Payment;

namespace TAL.QuoteAndApply.ServiceLayer.Payment.Models.Conveters
{
    public interface ISuperannuationFundSearchResponseConverter
    {
        SuperannuationFundSearchResponse From(ISuperAnnuationFund fund);
    }

    public class SuperannuationFundSearchResponseConverter : ISuperannuationFundSearchResponseConverter
    {
        public SuperannuationFundSearchResponse From(ISuperAnnuationFund fund)
        {
            var retVal = new SuperannuationFundSearchResponse()
            {
                FundUSI = fund.FundUSI,
                FundABN = fund.FundABN,
                FundName = fund.FundName,
                FundProduct = fund.FundProduct
            };
            return retVal;
        }
    }
}