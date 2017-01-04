using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Payment;

namespace TAL.QuoteAndApply.Payment.Models
{
    public class SuperAnnuationPaymentDto : DbItem, ISuperAnnuationFund, ISuperAnnuationPayment
    {
        public string FundName { get; set; }
        public string FundABN { get; set; }
        public string FundUSI { get; set; }
        public string FundProduct { get; set; }
        public string MembershipNumber { get; set; }
        public string TaxFileNumber { get; set; }
        public int PolicyPaymentId { get; set; }
    }
}