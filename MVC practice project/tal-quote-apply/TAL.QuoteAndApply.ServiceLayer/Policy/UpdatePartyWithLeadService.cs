using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Party.Leads;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IUpdatePartyWithLeadService
    {
        void Update(long leadId, CreateQuoteParam createQuoteParam, IParty partyDto, PolicySource policySource);
    }

    public class UpdatePartyWithLeadService : IUpdatePartyWithLeadService
    {

        private readonly IGetLeadService _getLeadService;
        private readonly IGetLeadResultConverter _getLeadResultConverter;
        private readonly IPartyDtoUpdater _partyDtoUpdater;
        private readonly IPartyService _partyService;

        public UpdatePartyWithLeadService(IGetLeadService getLeadService, IGetLeadResultConverter getLeadResultConverter, IPartyDtoUpdater partyDtoUpdater, IPartyService partyService)
        {
            _getLeadService = getLeadService;
            _getLeadResultConverter = getLeadResultConverter;
            _partyDtoUpdater = partyDtoUpdater;
            _partyService = partyService;
        }

        public void Update(long leadId, CreateQuoteParam createQuoteParam, IParty partyDto, PolicySource policySource)
        {
            var adobeLeadAndCommPrefResult = _getLeadService.Get(createQuoteParam.PersonalInformation.LeadId.Value);
            var personalInformation = _getLeadResultConverter.From(adobeLeadAndCommPrefResult, createQuoteParam.PersonalInformation.LeadId.Value);
            var ratingFactorsParam = _getLeadResultConverter.From(createQuoteParam.RatingFactors, adobeLeadAndCommPrefResult);
            var updatePartyWithLeadDto = _partyDtoUpdater.UpdateFrom(partyDto, personalInformation);
            updatePartyWithLeadDto = _partyDtoUpdater.UpdateFrom(updatePartyWithLeadDto, ratingFactorsParam);
            _partyService.UpdateParty(updatePartyWithLeadDto, policySource);
        }
    }
}
