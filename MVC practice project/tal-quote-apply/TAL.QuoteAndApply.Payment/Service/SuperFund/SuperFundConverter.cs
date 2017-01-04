using System;
using System.Linq;
using TAL.Enterprise.Superstream.Acord.Binary;

namespace TAL.QuoteAndApply.Payment.Service.SuperFund
{
    public interface ISuperFundConverter
    {
        Models.SuperFund From(InvestmentInquiry_Type investmentInquiry);
    }

    public class SuperFundConverter : ISuperFundConverter
    {
        public Models.SuperFund From(InvestmentInquiry_Type investmentInquiry)
        {
            var retVal = new Models.SuperFund();
            retVal.FundName = investmentInquiry.Organization.First().OrganizationName.First().FullName;
            retVal.FundABN = investmentInquiry.Organization.First()
                    .ExternalIdentifier
                    .First(ei => ei.TypeCode.Value.Name.Equals("ABN", StringComparison.OrdinalIgnoreCase))
                    .Id;
            retVal.FundProduct = investmentInquiry.InvestmentAccount.First()
                    .SuperannuationSection.First()
                    .ProductReferences.First()
                    .AssignedIdentifier.First(
                        ai =>
                            ai.IdentifierDescription.Equals("Superannuation product name",
                                StringComparison.OrdinalIgnoreCase))
                    .Id;
            retVal.FundUSI = investmentInquiry.key;
            return retVal;
        }
    }
}
