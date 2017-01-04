using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Party.Leads;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IGetLeadResultConverter
    {
        PersonalInformationParam From(GetLeadResult adobeLeadResult, long? adobeId);
        RatingFactorsParam From(RatingFactorsParam currentRatingFactorsParam, GetLeadResult adobeLeadResult);
    }

    public class GetLeadResultConverter : IGetLeadResultConverter
    {
        public PersonalInformationParam From(GetLeadResult adobeLeadResult, long? adobeId)
        {
            return new PersonalInformationParam(adobeLeadResult.Title,
                adobeLeadResult.FirstName.EmptyOrWhiteSpaceToNull(),
                adobeLeadResult.Surname.EmptyOrWhiteSpaceToNull(),
                adobeLeadResult.MobileNumber.EmptyOrWhiteSpaceToNullAndStripSpaces(),
                adobeLeadResult.HomeNumber.EmptyOrWhiteSpaceToNullAndStripSpaces(),
                adobeLeadResult.State,
                adobeLeadResult.EmailAddress.EmptyOrWhiteSpaceToNull(),
                adobeId,
                adobeLeadResult.Suburb,
                adobeLeadResult.Address,
                adobeLeadResult.Postcode,
                adobeLeadResult.ExternalCustomerReference
                );
        }

        public RatingFactorsParam From(RatingFactorsParam currentRatingFactorsParam, GetLeadResult adobeLeadResult)
        {
            return new RatingFactorsParam(char.Parse(adobeLeadResult.Gender), 
                adobeLeadResult.DateOfBirth, 
                currentRatingFactorsParam.IsAustralianResident, 
                currentRatingFactorsParam.SmokerStatus, 
                currentRatingFactorsParam.OccupationCode, 
                currentRatingFactorsParam.IndustryCode, 
                currentRatingFactorsParam.Income);
        }
    }
}