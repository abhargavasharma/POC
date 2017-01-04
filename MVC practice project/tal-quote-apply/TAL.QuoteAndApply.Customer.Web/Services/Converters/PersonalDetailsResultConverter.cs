using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.Customer.Web.Services.Converters
{
    public interface IPersonalDetailsResultConverter
    {
        RiskPersonalDetailsResult From(PersonalDetailsViewModel personalDetails, int riskId, bool dncSelection);
        PersonalDetailsViewModel From(RiskPersonalDetailsResult personalDetails, RiskOverviewResult riskOverview);
    }

    public class PersonalDetailsResultConverter : IPersonalDetailsResultConverter
    {
        private const string Unknown = "unknown";

        public RiskPersonalDetailsResult From(PersonalDetailsViewModel personalDetails, int riskId, bool dncSelection)
        {
            return new RiskPersonalDetailsResult()
            {
                Address = personalDetails.ResidentialAddress,
                FirstName = personalDetails.FirstName,
                Surname = personalDetails.LastName,
                MobileNumber = personalDetails.MobileNumber,
                HomeNumber = personalDetails.HomeNumber,
                EmailAddress = personalDetails.EmailAddress,
                Title = (personalDetails.Title.ToLower() == Unknown) ? null : personalDetails.Title,
                Suburb = personalDetails.Suburb,
                Postcode = personalDetails.Postcode,
                State = (personalDetails.State.ToLower() == Unknown) ? null : personalDetails.State,
                RiskId = riskId,
                PartyConsentParam = new  PartyConsentParam(dncSelection ? new List<string>
                {
                    ConsentType.DncEmail.ToString().ToCamelCase(),
                    ConsentType.DncMobile.ToString().ToCamelCase(),
                    ConsentType.DncHomeNumber.ToString().ToCamelCase(),
                    ConsentType.DncPostalMail.ToString().ToCamelCase()
                } : new List<string>(), true) //Express Consent is always true here as this set when customer consents when passing save quote gate
            };
        }

        public PersonalDetailsViewModel From(RiskPersonalDetailsResult personalDetails, RiskOverviewResult riskOverview)
        {
            return new PersonalDetailsViewModel()
            {
                ResidentialAddress = personalDetails.Address,
                FirstName = personalDetails.FirstName,
                LastName = personalDetails.Surname,
                MobileNumber = personalDetails.MobileNumber,
                HomeNumber = personalDetails.HomeNumber,
                EmailAddress = personalDetails.EmailAddress,
                Title = (personalDetails.Title.ToLower() == Unknown) ? null : personalDetails.Title,
                Suburb = personalDetails.Suburb,
                Postcode = personalDetails.Postcode,
                State = (personalDetails.State.ToLower() == Unknown) ? null : personalDetails.State,
                DateOfBirth = riskOverview.DateOfBirth.ToString("dd/MM/yyyy")
            };
        }
    }
}