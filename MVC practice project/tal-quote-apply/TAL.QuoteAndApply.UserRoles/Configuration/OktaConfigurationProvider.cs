using System.Configuration;
using TAL.QuoteAndApply.Infrastructure.Crypto;

namespace TAL.QuoteAndApply.UserRoles.Configuration
{
    public interface IOktaConfigurationProvider
    {
        string ApiToken { get; }
        string BaseUri { get; }
        string ProxyUri { get; }
    }

    public class OktaConfigurationProvider : IOktaConfigurationProvider
    {
        private readonly ISecurityService _securityService;

        public OktaConfigurationProvider(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        public string ApiToken
        {
            get { return _securityService.Decrypt(ConfigurationManager.AppSettings["Okta.EncryptedApiToken"]); }
        }

        public string BaseUri => ConfigurationManager.AppSettings["Okta.BaseUri"];

        public string ProxyUri => ConfigurationManager.AppSettings["Okta.ProxyUri"];
    }
}
