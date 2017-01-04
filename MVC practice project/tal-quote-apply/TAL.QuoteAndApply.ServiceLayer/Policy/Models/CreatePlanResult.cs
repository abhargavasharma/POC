using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class CreatePlanResult
    {
        public readonly string QuoteReference;
        public readonly int CoverAmount;
        public readonly IDictionary<ValidationKey, string> Errors;

        public bool HasErrors
        {
            get { return Errors != null && Errors.Any(); }
        }


        public CreatePlanResult(string quoteReference, int coverAmount)
        {
            QuoteReference = quoteReference;
            CoverAmount = coverAmount;
        }

        public CreatePlanResult(IDictionary<ValidationKey, string> errors)
        {
            Errors = errors;
        }

    }
}
