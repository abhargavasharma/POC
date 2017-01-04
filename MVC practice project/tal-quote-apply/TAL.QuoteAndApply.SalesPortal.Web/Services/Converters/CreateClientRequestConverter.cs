
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface ICreateClientRequestConverter
    {
        CreateQuoteParam From(CreateClientRequest createClientRequest, string brand);
    }
    public class CreateClientRequestConverter : ICreateClientRequestConverter
    {
        private readonly IRatingFactorsRequestConverter _ratingFactorsRequestConverter;

        public CreateClientRequestConverter(IRatingFactorsRequestConverter ratingFactorsRequestConverter)
        {
            _ratingFactorsRequestConverter = ratingFactorsRequestConverter;
        }

        public CreateQuoteParam From(CreateClientRequest createClientRequest, string brand)
        {
            return new CreateQuoteParam(_ratingFactorsRequestConverter.From(createClientRequest.PolicyOwner.RatingFactors),
                PersonalInformationParamFrom(createClientRequest.PolicyOwner.PersonalDetails), createClientRequest.PolicyOwner.ValidateResidency, PolicySource.SalesPortal, brand);
        }

        private static PersonalInformationParam PersonalInformationParamFrom(
            PersonalDetailsRequest personalDetailsRequest)
        {
            return new PersonalInformationParam(
                personalDetailsRequest.Title,
                personalDetailsRequest.FirstName.EmptyOrWhiteSpaceToNull(),
                personalDetailsRequest.Surname.EmptyOrWhiteSpaceToNull(),
                personalDetailsRequest.MobileNumber.EmptyOrWhiteSpaceToNull(),
                personalDetailsRequest.HomeNumber.EmptyOrWhiteSpaceToNull(),                
                personalDetailsRequest.State,
                personalDetailsRequest.EmailAddress.EmptyOrWhiteSpaceToNull(),
                personalDetailsRequest.LeadId,
                personalDetailsRequest.Suburb,
                personalDetailsRequest.Address,
                personalDetailsRequest.Postcode,
                personalDetailsRequest.ExternalCustomerReference);
        }
    }
}