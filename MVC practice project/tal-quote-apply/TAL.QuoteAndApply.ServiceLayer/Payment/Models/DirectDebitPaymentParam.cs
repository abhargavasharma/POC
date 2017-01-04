using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Payment.Models
{
    public class DirectDebitPaymentParam
    {
        public string AccountName { get; set; }
        public string BSBNumber { get; set; }
        public string AccountNumber { get; set; }

        public bool IsValidForInforce { get; set; }
        
        public bool IsComplete { get; set; }
    }
}