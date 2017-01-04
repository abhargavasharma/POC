using System.Net;
using RestSharp;
using TAL.QuoteAndApply.Customer.Web.Configuration;
using TAL.QuoteAndApply.Infrastructure.Configuration;
using TAL.QuoteAndApply.Infrastructure.Logging;

namespace TAL.QuoteAndApply.Customer.Web.Services
{
    public interface ICaptchaService
    {
        bool Verify(string recaptchaResponse);
    }

    public class GoogleCaptchaService : ICaptchaService
    {
        private readonly ICaptchaConfigurationProvider _configuration;
        private readonly ILoggingService _loggingService;
        private readonly IRestProxyProvider _proxyProvider;

        public GoogleCaptchaService(ICaptchaConfigurationProvider configuration, ILoggingService loggingService, IRestProxyProvider proxyProvider)
        {
            _configuration = configuration;
            _loggingService = loggingService;
            _proxyProvider = proxyProvider;
        }

        public bool Verify(string recaptchaResponse)
        {
            if (!_configuration.IsEnabled)
                return true; // Always good ;)

            if (string.IsNullOrEmpty(recaptchaResponse))
                return false;

            var client = new RestClient(_configuration.GoogleCaptchaBaseUrl) { Proxy = _proxyProvider.DefaultProxy };
            var request = new RestRequest("siteverify");
            request.AddHeader("Content-Type", "application/json");
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("secret", _configuration.GoogleCaptchaPrivateKey);
            request.AddParameter("response", recaptchaResponse);

            var response = client.Post<GoogleCaptchaResponse>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _loggingService.Error("Google did not return a 200 OK");
                return false;
            }

            var googleResponse = response.Data;
            
            if (!googleResponse.Success)
            {
                _loggingService.Error(googleResponse.ToString());
                return false;
            }

            _loggingService.Info(googleResponse.ToString());
            return true;
        }
    }
}