using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk
{
    public class UpdatePartyService : IDateOfBirthChangeObserver, IGenderChangeObserver
    {
        private readonly IPartyService _partyService;
        private readonly IPolicySourceProvider _policySourceProvider;

        public UpdatePartyService(IPartyService partyService, IPolicySourceProvider policySourceProvider)
        {
            _partyService = partyService;
            _policySourceProvider = policySourceProvider;
        }

        public void Update(UpdateDateOfBirthParam updateDateOfBirthParam)
        {
            var party = _partyService.GetParty(updateDateOfBirthParam.PartyId);
            party.DateOfBirth = updateDateOfBirthParam.DateOfBirth;

            var policySource = _policySourceProvider.From(updateDateOfBirthParam.RiskId);

            _partyService.UpdateParty(party, policySource);
        }

        public void Update(UpdateGenderParam updateGenderParam)
        {
            var party = _partyService.GetParty(updateGenderParam.PartyId);
            party.Gender = updateGenderParam.Gender.MapToGender();

            var policySource = _policySourceProvider.From(updateGenderParam.RiskId);

            _partyService.UpdateParty(party, policySource);
        }

    }
}
