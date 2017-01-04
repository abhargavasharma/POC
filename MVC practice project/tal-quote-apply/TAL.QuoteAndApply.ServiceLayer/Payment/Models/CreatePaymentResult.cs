using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Payment.Models
{
    public class CreatePaymentResult
    {
        public string QuoteReference { get; private set; }
        public string UnderwritingId { get; private set; }
        public List<ValidationError> Errors { get; private set; }
        public bool IsComplete { get; set; }

        public bool HasErrors
        {
            get { return Errors != null && Errors.Any(); }
        }

        public CreatePaymentResult(string quoteReference, string underwritingId)
        {
            QuoteReference = quoteReference;
            UnderwritingId = underwritingId;
        }

        public CreatePaymentResult(IEnumerable<ValidationError> errors)
        {
            Errors = errors.ToList();
        }
    }
}
