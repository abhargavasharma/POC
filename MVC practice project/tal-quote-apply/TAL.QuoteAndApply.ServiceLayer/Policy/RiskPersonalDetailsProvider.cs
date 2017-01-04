using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IRiskPersonalDetailsProvider
    {
        RiskPersonalDetailsResult GetFor(int riskId);
    }

    public class RiskPersonalDetailsProvider : IRiskPersonalDetailsProvider
    {
        private readonly IRiskService _riskService;
        private readonly IPartyService _partyService;
        private readonly IRiskPersonalDetailsResultConverter _riskPersonalDetailsResultConverter;
        private readonly IPartyConsentService _partyConsentService;

        public RiskPersonalDetailsProvider(IPartyService partyService, IRiskPersonalDetailsResultConverter riskPersonalDetailsResultConverter, IRiskService riskService, IPartyConsentService partyConsentService)
        {
            _partyService = partyService;
            _riskPersonalDetailsResultConverter = riskPersonalDetailsResultConverter;
            _riskService = riskService;
            _partyConsentService = partyConsentService;
        }

        public RiskPersonalDetailsResult GetFor(int riskId)
        {
            var risk = _riskService.GetRisk(riskId);
            var party = _partyService.GetParty(risk.PartyId);
            var partyConsent = _partyConsentService.GetPartyConsentByPartyId(risk.PartyId);

            var model =  _riskPersonalDetailsResultConverter.CreateFrom(risk, party, partyConsent);
            model.IsPersonalDetailsValidForInforce = _partyService.IsPartyValidForInforce(party);

            return model;
        }
    }
}
