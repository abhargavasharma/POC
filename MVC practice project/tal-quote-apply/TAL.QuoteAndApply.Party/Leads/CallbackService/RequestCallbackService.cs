using System;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Infrastructure.Http.Exceptions;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.Infrastructure.Url;
using TAL.QuoteAndApply.Party.Configuration;

namespace TAL.QuoteAndApply.Party.Leads.CallbackService
{
    public interface IRequestCallbackService
    {
        Task<CallbackResponse> RequestCallback(string leadId, string phoneNumber);
    }

    public class RequestCallbackService : IRequestCallbackService
    {
        private readonly ILeadConfigurationProvider _leadConfigurationProvider;
        private readonly IHttpClientService _clientService;
        private readonly ILoggingService _loggingService;
        private readonly IUrlUtilities _urlUtilities;
        private string _origin;

        public RequestCallbackService(ILeadConfigurationProvider leadConfigurationProvider, IHttpClientService clientService, ILoggingService loggingService, IUrlUtilities urlUtilities)
        {
            _leadConfigurationProvider = leadConfigurationProvider;
            _clientService = clientService;
            _loggingService = loggingService;
            _urlUtilities = urlUtilities;

            _origin = _urlUtilities.UrlEncode("Call me now");
        }

        public async Task<CallbackResponse> RequestCallback(string leadId, string phoneNumber)
        {
            var endPointUrl = $"{_leadConfigurationProvider.ConnectionString}/api/CallImmediate/{leadId}?phoneNumber={phoneNumber}&origin={_origin}";

            var request = new PutOrPostRequest(new Uri(endPointUrl), null)
                .WithSourceType(SourceType.Xml)
                .WithHeader("api-version", _leadConfigurationProvider.Version)
                .WithTimeout(_leadConfigurationProvider.Timeout);

            try
            {
                var response = await _clientService.PostAsync<CallbackResponse>(request);
                return response;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyServiceException("Leads request callback service failed with leadId: " + leadId, ex);
                _loggingService.Error(exception);
                throw exception;
            }
        }
    }
}
