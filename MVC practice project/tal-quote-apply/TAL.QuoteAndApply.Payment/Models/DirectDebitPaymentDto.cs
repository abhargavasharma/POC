using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Payment;

namespace TAL.QuoteAndApply.Payment.Models
{
    public class DirectDebitPaymentDto : DbItem, IDirectDebit, IDirectDebitPayment
    {
        public string AccountName { get; set; }
        public string BSBNumber { get; set; }
        public string AccountNumber { get; set; }
        public int PolicyPaymentId { get; set; }
    }
}