
using TAL.QuoteAndApply.ServiceLayer.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class CreateQuoteParam
    {
        public RatingFactorsParam RatingFactors { get; private set; }
        public PersonalInformationParam PersonalInformation { get; private set; }
        public bool ValidateResidency { get; private set; }
        public PolicySource Source { get; private set; }
        public string Brand { get; private set; }

        public CreateQuoteParam(RatingFactorsParam ratingFactors, PersonalInformationParam personalInformation,
            bool validateResidency) : this(ratingFactors, personalInformation, validateResidency, PolicySource.CustomerPortalBuildMyOwn, "TAL")
        {
        }

        public CreateQuoteParam(RatingFactorsParam ratingFactors, PersonalInformationParam personalInformation,
            bool validateResidency, PolicySource source, string brand)
        {
            RatingFactors = ratingFactors;
            PersonalInformation = personalInformation;
            ValidateResidency = validateResidency;
            Source = source;
            Brand = brand;
        }
    }
}
