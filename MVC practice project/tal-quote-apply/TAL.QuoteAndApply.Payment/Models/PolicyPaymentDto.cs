using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Payment;

namespace TAL.QuoteAndApply.Payment.Models
{
    public class PolicyPaymentDto : DbItem
    {
        public int PolicyId { get; set; }
        public PaymentType PaymentType { get; set; }
    }
}