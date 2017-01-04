using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class CreateQuoteResult
    {
        public string QuoteReference { get; private set; }
        public string UnderwritingId { get; private set; }
        public List<ValidationError> Errors { get; private set; }

        public bool HasErrors
        {
            get { return Errors != null && Errors.Any(); }
        }
        
        public CreateQuoteResult(string quoteReference, string underwritingId)
        {
            QuoteReference = quoteReference;
            UnderwritingId = underwritingId;
        }

        public CreateQuoteResult(IEnumerable<ValidationError> errors)
        {
            Errors = errors.ToList();
        }

    }
}
