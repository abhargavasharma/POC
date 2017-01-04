using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Policy.Models;
using State = TAL.QuoteAndApply.DataModel.Personal.State;
using Title = TAL.QuoteAndApply.DataModel.Personal.Title;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IRiskPersonalDetailsResultConverter
    {
        RiskPersonalDetailsResult CreateFrom(IRisk risk, IParty party, IPartyConsent partyConsent);
    }

    public class RiskPersonalDetailsResultConverter : IRiskPersonalDetailsResultConverter
    {
        private readonly IPartyConsentParamConverter _partyConsentParamConverter;

        public RiskPersonalDetailsResultConverter(IPartyConsentParamConverter partyConsentParamConverter)
        {
            _partyConsentParamConverter = partyConsentParamConverter;
        }

        public RiskPersonalDetailsResult CreateFrom(IRisk risk, IParty party, IPartyConsent partyConsent)
        {
            var partyConsentResult = _partyConsentParamConverter.CreateFrom(partyConsent);
            var returnVal = new RiskPersonalDetailsResult
            {
                RiskId = risk.Id
            };

            if (party != null)
            {
                returnVal.Title = MapTitle(party.Title);
                returnVal.FirstName = party.FirstName;
                returnVal.Surname = party.Surname;
                returnVal.Address = party.Address;
                returnVal.Suburb = party.Suburb;
                returnVal.State = MapState(party.State);
                returnVal.Postcode = party.Postcode;
                returnVal.MobileNumber = party.MobileNumber;
                returnVal.HomeNumber = party.HomeNumber;
                returnVal.EmailAddress = party.EmailAddress;
                returnVal.ExternalCustomerReference = party.ExternalCustomerReference;
                returnVal.PartyConsentParam = new PartyConsentParam(partyConsentResult.Consents, partyConsentResult.ExpressConsent);
            }

            return returnVal;
        }

        private string MapState(State state)
        {
            return state.ToString().ToUpper();
        }

        private string MapTitle(Title title)
        {
            return title.ToString();
        }
    }

}
