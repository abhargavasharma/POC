using System.Collections.Generic;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IPersonalDetailsRequestConverter
    {
        RiskPersonalDetailsParam From(int riskId, PersonalDetailsRequest personalDetailsRequest);
        PersonalDetailsRequest From(RiskPersonalDetailsResult risk);
        RiskPersonalDetailsParam From(int riskId, LifeInsuredDetailsRequest personalDetailsRequest);
    }

    public class PersonalDetailsRequestConverter : IPersonalDetailsRequestConverter
    {
        public PersonalDetailsRequest From(RiskPersonalDetailsResult risk)
        {
            return new PersonalDetailsRequest
            {
                Title = risk.Title,
                FirstName = risk.FirstName,
                Surname = risk.Surname,
                Address = risk.Address,
                Suburb = risk.Suburb,
                State = risk.State,
                Postcode = risk.Postcode,
                MobileNumber = risk.MobileNumber,
                HomeNumber = risk.HomeNumber,
                EmailAddress = risk.EmailAddress,
                ExternalCustomerReference = risk.ExternalCustomerReference,
                IsCompleted = risk.IsPersonalDetailsValidForInforce,
                PartyConsents = risk.PartyConsentParam.Consents,
                ExpressConsent = risk.PartyConsentParam.ExpressConsent
            };
        }

        public RiskPersonalDetailsParam From(int riskId, PersonalDetailsRequest personalDetailsRequest)
        {
            return new RiskPersonalDetailsParam
            {
                RiskId = riskId,
                Title = personalDetailsRequest.Title,
                FirstName = personalDetailsRequest.FirstName.EmptyOrWhiteSpaceToNull(),
                Surname = personalDetailsRequest.Surname.EmptyOrWhiteSpaceToNull(),
                Address = personalDetailsRequest.Address.EmptyOrWhiteSpaceToNull(),
                Suburb = personalDetailsRequest.Suburb.EmptyOrWhiteSpaceToNull(),
                State = personalDetailsRequest.State,
                Postcode = personalDetailsRequest.Postcode.EmptyOrWhiteSpaceToNull(),
                MobileNumber = personalDetailsRequest.MobileNumber.EmptyOrWhiteSpaceToNull(),
                HomeNumber = personalDetailsRequest.HomeNumber.EmptyOrWhiteSpaceToNull(),
                EmailAddress = personalDetailsRequest.EmailAddress.EmptyOrWhiteSpaceToNull(),
                ExternalCustomerReference = personalDetailsRequest.ExternalCustomerReference.EmptyOrWhiteSpaceToNull(),
                PartyConsentParam = new PartyConsentParam(personalDetailsRequest.PartyConsents, personalDetailsRequest.ExpressConsent)
            };
        }

        public RiskPersonalDetailsParam From(int riskId, LifeInsuredDetailsRequest personalDetailsRequest)
        {
            return new RiskPersonalDetailsParam
            {
                RiskId = riskId,
                Title = personalDetailsRequest.Title,
                FirstName = personalDetailsRequest.FirstName.EmptyOrWhiteSpaceToNull(),
                Surname = personalDetailsRequest.Surname.EmptyOrWhiteSpaceToNull(),

                Postcode = personalDetailsRequest.Postcode,


                PartyConsentParam = new PartyConsentParam(new List<string>(), personalDetailsRequest.ExpressConsent)
            };
        }
    }
}