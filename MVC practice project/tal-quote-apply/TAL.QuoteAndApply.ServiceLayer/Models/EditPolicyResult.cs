using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Models
{
    public class EditPolicyResult
    {
        public readonly int MaxCoverAmount;
        public readonly int MinCoverAmount;
        public int CoverAmount;
        public readonly List<ValidationError> Errors;
        public readonly List<ValidationError> Warnings;
        public readonly bool IsEligible;

        public bool HasErrors
        {
            get { return Errors != null && Errors.Any(); }
        }

        public bool HasWarnings
        {
            get { return Warnings != null && Warnings.Any(); }
        }

        public bool HasErrorsOrWarnings => HasErrors || HasWarnings;

        public EditPolicyResult(int maxCoverAmount, int minCoverAmount, int coverAmount, List<ValidationError> warnings, bool isEligible)
        {
            MaxCoverAmount = maxCoverAmount;
            MinCoverAmount = minCoverAmount;
            CoverAmount = coverAmount;
            Warnings = warnings;
            IsEligible = isEligible;
        }

        public EditPolicyResult(IEnumerable<ValidationError> errors, bool isEligible)
        {
            IsEligible = isEligible;
            Errors = errors.ToList();
        }
    }
}
