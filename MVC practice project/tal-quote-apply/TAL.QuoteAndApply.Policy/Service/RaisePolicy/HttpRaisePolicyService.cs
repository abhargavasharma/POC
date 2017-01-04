using System;
using System.Net;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.Policy.Configuration;

namespace TAL.QuoteAndApply.Policy.Service.RaisePolicy
{
    public interface IHttpRaisePolicyService
    {
        bool Submit(string quoteReferenceNumber, PolicyNewBusinessOrderProcess_Type oolicyUberObject);
    }

    public class HttpRaisePolicyService : IHttpRaisePolicyService
    {
        private readonly IHttpClientService _clientService;
        private readonly IRaisePolicyConfigurationProvider _configuration;
        private readonly ILoggingService _loggingService;

        public HttpRaisePolicyService(IHttpClientService clientService, IRaisePolicyConfigurationProvider configuration, ILoggingService loggingService)
        {
            _clientService = clientService;
            _configuration = configuration;
            _loggingService = loggingService;
        }

        public bool Submit(string quoteReferenceNumber, PolicyNewBusinessOrderProcess_Type policyUberObject)
        {
            var queryString = "/api/raise";
            var endPointUrl = _configuration.ConnectionString + queryString;

            var postRequest = new PutOrPostRequest(new Uri(endPointUrl), policyUberObject)
                .WithSourceType(SourceType.Xml)
                .WithCredentials(new NetworkCredential(_configuration.UserName, _configuration.Password, _configuration.Domain));

            try
            {
                var raisePolicyPost = _clientService.PostAsync<bool>(postRequest).Result;
                return raisePolicyPost;
            }
            catch (Exception ex)
            {
                _loggingService.Error($"RaisePolicy Service error for quote: {quoteReferenceNumber}", ex);
                throw;
            }
            
        }
    }
}
