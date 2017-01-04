using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Party.Leads.CallbackService;
using TAL.QuoteAndApply.Party.Services;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface ICustomerCallbackService
    {
        Task<bool> RequestCallback(int partyId, string phoneNumber);
    }

    public class CustomerCallbackService : ICustomerCallbackService
    {

        private readonly IPartyService _partyService;
        private readonly IRequestCallbackService _requestCallbackService;

        public CustomerCallbackService(IRequestCallbackService requestCallbackService, IPartyService partyService)
        {
            _requestCallbackService = requestCallbackService;
            _partyService = partyService;
        }

        public async Task<bool> RequestCallback(int partyId, string phoneNumber)
        {
            var party = _partyService.GetParty(partyId);

            if(party == null)
                throw new Exception($"Unable to find part for id: {partyId}");

            if (!party.LeadId.HasValue)
                throw new Exception("Lead Id is no assigned to party yet.");

            var response = await _requestCallbackService.RequestCallback(party.LeadId.Value.ToString(), phoneNumber);
            return response != null && response.ErrorCode == 0;
        }
    }
}
