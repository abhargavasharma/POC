using System;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Infrastructure.Features;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Infrastructure.Http.Exceptions;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.Infrastructure.Url;
using TAL.QuoteAndApply.Party.Configuration;

namespace TAL.QuoteAndApply.Party.Leads.LeadsService
{
    public interface IHttpLeadsService
    {
        LeadResponse CreateLead(MarketingInquiryProcess leadRequest);
        LeadResponse UpdateLead(MarketingInquiryProcess leadRequest);
        PartyCommunicationInquiryNotify GetCommunicationPreferences(long? leadId);
        CommunicationPreferenceUpdateResponse UpdateCommunicationPreferences(
            PartyCommunicationProcess partyCommunicationProcess);
        MarketingInquiryProcessResult RetrieveLead(long? leadId);
    }

    public class HttpLeadsService : IHttpLeadsService
    {
        private readonly ILeadConfigurationProvider _leadsConfigurationProvider;
        private readonly IHttpClientService _clientService;
        private readonly IUrlUtilities _urlUtilities;
        private readonly ILoggingService _loggingService;
        private readonly IFeatures _features;

        public HttpLeadsService(ILeadConfigurationProvider leadsConfigurationProvider, IHttpClientService clientService, IUrlUtilities urlUtilities, ILoggingService loggingService, IFeatures features)
        {
            _leadsConfigurationProvider = leadsConfigurationProvider;
            _clientService = clientService;
            _urlUtilities = urlUtilities;
            _loggingService = loggingService;
            _features = features;
        }

        public MarketingInquiryProcessResult RetrieveLead(long? leadId)
        {
            if (!_features.AdobeLeadsServiceTrigger) return null;
            var endPointUrl = $"{_leadsConfigurationProvider.ConnectionString}/api/leadretrieve?leadId={leadId}";

            var getRequest = new GetRequest(new Uri(endPointUrl))
                .WithSourceType(SourceType.Xml)
                .WithHeader("api-version", _leadsConfigurationProvider.Version)
                .WithTimeout(_leadsConfigurationProvider.Timeout);

            try
            {
                var retrieveLead = _clientService.GetAsync<MarketingInquiryProcessResult>(getRequest).Result;
                return retrieveLead;
            }
            catch (Exception ex)
            {
                var exception =
                    new ThirdPartyServiceException("Retrieval with LeadId of leads from adobe service failed", ex);
                _loggingService.Error(exception);
                return null;
            }
        }

        public MarketingInquiryProcessResult RetrieveLeads(RetrieveLeadRequest retrieveLeadRequest)
        {
            if (!_features.AdobeLeadsServiceTrigger) return null;
            var querystring = GetQueryString(retrieveLeadRequest);

            var endPointUrl = $"{_leadsConfigurationProvider.ConnectionString}/api/leadretrieve?{querystring}";

            var getRequest = new GetRequest(new Uri(endPointUrl))
                .WithSourceType(SourceType.Xml)
                .WithHeader("api-version", "1")
                .WithTimeout(_leadsConfigurationProvider.Timeout);

            try
            {
                var retrieveLeads = _clientService.GetAsync<MarketingInquiryProcessResult>(getRequest).Result;
                return retrieveLeads;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyServiceException("Retrieval with RetrieveLeadRequest of leads from adobe service failed", ex);
                _loggingService.Error(exception);
                return null;
            }
        }

        public LeadResponse CreateLead(MarketingInquiryProcess leadRequest)
        {
            if (!_features.AdobeLeadsServiceTrigger) return null;
            var endPointUrl = $"{_leadsConfigurationProvider.ConnectionString}/api/leadcreate";

            var postRequest = new PutOrPostRequest(new Uri(endPointUrl), leadRequest)
                .WithSourceType(SourceType.Xml)
                .WithHeader("api-version", _leadsConfigurationProvider.Version)
                .WithTimeout(_leadsConfigurationProvider.Timeout);

            try
            {
                var createLeadResponse = _clientService.PostAsync<LeadResponse>(postRequest).Result;

                if (createLeadResponse.Status.Equals("ok", StringComparison.InvariantCultureIgnoreCase))
                {
                    return createLeadResponse;
                }

                throw new ApplicationException($"CreateLead exception with error code: {createLeadResponse.ErrorCode}");
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyServiceException("Creation of lead with the adobe service failed", ex);
                _loggingService.Error(exception);
                return null;
            }
        }

        public LeadResponse UpdateLead(MarketingInquiryProcess leadRequest)
        {
            if (!_features.AdobeLeadsServiceTrigger) return null;
            var endPointUrl = $"{_leadsConfigurationProvider.ConnectionString}/api/leadupdate";

            var postRequest = new PutOrPostRequest(new Uri(endPointUrl), leadRequest)
                .WithSourceType(SourceType.Xml)
                .WithHeader("api-version", _leadsConfigurationProvider.Version)
                .WithTimeout(_leadsConfigurationProvider.Timeout);

            try
            {
                var createLeadResponse = _clientService.PutAsync<LeadResponse>(postRequest).Result;

                if (createLeadResponse.Status.Equals("ok", StringComparison.InvariantCultureIgnoreCase))
                {
                    return createLeadResponse;
                }

                throw new ApplicationException($"CreateLead exception. {createLeadResponse.ToXml()}");
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyServiceException("Update of lead with the adobe service failed", ex);
                _loggingService.Error(exception);
                return null;
            }
        }

        public PartyCommunicationInquiryNotify GetCommunicationPreferences(long? leadId)
        {
            if (!_features.AdobeLeadsServiceTrigger) return null;
            var endPointUrl = $"{_leadsConfigurationProvider.ConnectionString}/api/CommunicationPreferenceRetrieve/{leadId}";

            var getRequest = new GetRequest(new Uri(endPointUrl))
                .WithSourceType(SourceType.Xml)
                .WithHeader("api-version", _leadsConfigurationProvider.Version)
                .WithTimeout(_leadsConfigurationProvider.Timeout);

            try
            {
                var getCommunicationPreferencesResponse = _clientService.GetAsync<PartyCommunicationInquiryNotify>(getRequest).Result;
                return getCommunicationPreferencesResponse;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyServiceException("Retrievald of communication preferences with the adobe service failed", ex);
                _loggingService.Error(exception);
                return null;
            }
        }

        public CommunicationPreferenceUpdateResponse UpdateCommunicationPreferences(PartyCommunicationProcess partyCommunicationProcess)
        {
            if (!_features.AdobeLeadsServiceTrigger) return null;
            var endPointUrl = $"{_leadsConfigurationProvider.ConnectionString}/api/CommunicationPreferenceUpdate";

            var putOrPostRequest = new PutOrPostRequest(new Uri(endPointUrl), partyCommunicationProcess)
                .WithSourceType(SourceType.Xml)
                .WithHeader("api-version", _leadsConfigurationProvider.Version)
                .WithTimeout(_leadsConfigurationProvider.Timeout);

            try
            {
                var updateCommunicationPreferencesResponse = _clientService.PutAsync<CommunicationPreferenceUpdateResponse>(putOrPostRequest).Result;

                if (updateCommunicationPreferencesResponse.RecipientUpdateResponses.All(x => x.UpdateStatus.Equals("ok", StringComparison.InvariantCultureIgnoreCase)))
                {
                    return updateCommunicationPreferencesResponse;
                }

                throw new ApplicationException($"UpdateCommunicationPreferences exception. {updateCommunicationPreferencesResponse.ToXml()}");
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyServiceException("Update of communication preferences with the adobe service failed", ex);
                _loggingService.Error(exception);
                return null;
            }
        }


        private string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + _urlUtilities.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }
    }
}
