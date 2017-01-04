using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class UpdateRiskRatingFactorsResult
    {
        public List<ValidationError> Errors { get; private set; }

        public bool HasErrors
        {
            get { return Errors != null && Errors.Any(); }
        }

        public UpdateRiskRatingFactorsResult(IEnumerable<ValidationError> errors)
        {
            Errors = errors.ToList();
        }

        public bool IsRatingFactorsValidForInforce { get; private set; }

        public UpdateRiskRatingFactorsResult(bool isRatingFactorsValidForInforce)
        {
            IsRatingFactorsValidForInforce = isRatingFactorsValidForInforce;
        }
    }
}