using TAL.QuoteAndApply.Party.Leads.LeadsService;

namespace TAL.QuoteAndApply.Party.Leads
{
    public interface IGetLeadService
    {
        GetLeadResult Get(long leadId);
        GetPreferredCommunicationResult GetCommunicationPreferences(long leadId);
    }

    public class GetLeadService : IGetLeadService
    {
        private readonly IHttpLeadsService _leadsService;
        private readonly IAdobeServiceResultConverter _adobeServiceResultConverter;

        public GetLeadService(IHttpLeadsService leadsService,
            IAdobeServiceResultConverter adobeServiceResultConverter)
        {
            _leadsService = leadsService;
            _adobeServiceResultConverter = adobeServiceResultConverter;
        }

        public GetLeadResult Get(long leadId)
        {
            var lead = _leadsService.RetrieveLead(leadId);
            if (lead != null)
            {
                return _adobeServiceResultConverter.From(lead);
            }
            return null;
        }

        public GetPreferredCommunicationResult GetCommunicationPreferences(long leadId)
        {
            var communicationPreferences = _leadsService.GetCommunicationPreferences(leadId);
            if (communicationPreferences != null)
            {
                return _adobeServiceResultConverter.From(communicationPreferences);
            }
            return null;
        }
    }
}
