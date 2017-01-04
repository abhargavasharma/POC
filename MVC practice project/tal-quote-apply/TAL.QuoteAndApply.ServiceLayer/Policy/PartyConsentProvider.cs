using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPartyConsentProvider
    {
        PartyConsentResult GetFor(int riskId);
    }

    public class PartyConsentProvider : IPartyConsentProvider
    {
        private readonly IRiskService _riskService;
        private readonly IPartyConsentService _partyConsentService;
        private readonly IPartyConsentResultConverter _partyConsentResultConverter;

        public PartyConsentProvider(IRiskService riskService, IPartyConsentService partyConsentService, IPartyConsentResultConverter partyConsentResultConverter)
        {
            _riskService = riskService;
            _partyConsentService = partyConsentService;
            _partyConsentResultConverter = partyConsentResultConverter;
        }

        public PartyConsentResult GetFor(int riskId)
        {
            var risk = _riskService.GetRisk(riskId);
            var party = _partyConsentService.GetPartyConsentByPartyId(risk.PartyId);
            var model = _partyConsentResultConverter.CreateFrom(risk, party);
            return model;
        }
    }
}
