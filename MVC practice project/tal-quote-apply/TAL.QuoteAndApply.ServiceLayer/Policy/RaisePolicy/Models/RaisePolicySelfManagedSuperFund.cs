using TAL.QuoteAndApply.Payment.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models
{
    public class RaisePolicySelfManagedSuperFund : ISelfManagedSuperFundPayment
    {
        public string AccountName { get; set; }
        public string BSBNumber { get; set; }
        public string BSBNumberFirstThree { get; set; }
        public string BSBNumberSecondThree { get; set; }
        public string AccountNumber { get; set; }
        public int Id { get; set; }
        public int PolicyPaymentId { get; set; }
    }
}