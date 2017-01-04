using TAL.QuoteAndApply.Payment.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy
{
    public class RaisePolicySuperannuation : ISuperAnnuationPayment
    {
        public string FundName { get; set; }
        public string FundABN { get; set; }
        public string FundUSI { get; set; }
        public string FundProduct { get; set; }
        public string MembershipNumber { get; set; }
        public string TaxFileNumber { get; set; }
        public int Id { get; set; }
        public int PolicyPaymentId { get; set; }
    }
}