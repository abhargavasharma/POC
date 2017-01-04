using TAL.QuoteAndApply.DataModel.Payment;

namespace TAL.QuoteAndApply.Payment.Models
{
    public class SuperFund : ISuperAnnuationFund
    {
        public string FundName { get; set; }
        public string FundABN { get; set; }
        public string FundUSI { get; set; }
        public string FundProduct { get; set; }
        public string MembershipNumber { get; set; }
    }
}