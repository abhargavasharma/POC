using System;
using System.Net;
using TAL.QuoteAndApply.Infrastructure.Configuration;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Infrastructure.Http.Exceptions;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.Infrastructure.Url;

namespace TAL.QuoteAndApply.Infrastructure.Crypto
{
    public interface IPasEncryptionHttpService
    {
        string Encrypt(string value);
    }

    public class PasEncryptionHttpService : IPasEncryptionHttpService
    {
        private readonly IPasEncryptionConfigurationProvider _pasEncryptionConfigurationProvider;
        private readonly IHttpClientService _clientService;
        private readonly IUrlUtilities _urlUtilities;
        private readonly ILoggingService _loggingService;

        public PasEncryptionHttpService(IPasEncryptionConfigurationProvider pasEncryptionConfigurationProvider, IHttpClientService clientService, IUrlUtilities urlUtilities, ILoggingService loggingService)
        {
            _pasEncryptionConfigurationProvider = pasEncryptionConfigurationProvider;
            _clientService = clientService;
            _urlUtilities = urlUtilities;
            _loggingService = loggingService;
        }

        public string Encrypt(string value)
        {
            var endPointUrl = $"{_pasEncryptionConfigurationProvider.PasEncryptionServiceBaseUrl}/api/PasEncrypt/{_urlUtilities.UrlEncode(value)}";

            Console.WriteLine(endPointUrl);

            var postRequest = new PutOrPostRequest(new Uri(endPointUrl), null)
                .WithSourceType(SourceType.Xml)
                .WithCredentials(new NetworkCredential(_pasEncryptionConfigurationProvider.PasEncryptionServiceUser,
                    _pasEncryptionConfigurationProvider.PasEncryptionServicePassword,
                    _pasEncryptionConfigurationProvider.PasEncryptionServiceDomain));
            
            try
            {
                var encryptedValue = _clientService.PostAsync<string>(postRequest).Result;
                return encryptedValue;
            }
            catch (Exception ex)
            {
                var exception = new ThirdPartyServiceException("Issue encrypting value", ex);
                _loggingService.Error(exception);
                throw exception;
            }

        }
    }
}
