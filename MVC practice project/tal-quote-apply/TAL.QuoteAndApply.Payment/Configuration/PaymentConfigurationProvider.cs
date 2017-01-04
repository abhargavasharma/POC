
using System;
using System.Collections.Specialized;
using System.Configuration;

namespace TAL.QuoteAndApply.Payment.Configuration
{
    public interface IPaymentConfigurationProvider
    {
        string ConnectionString { get; }
        string TokenisationBaseUrl { get; }
        string TokenisationIdentityUser { get; }
        string TokenisationThumbPrint { get; }
        string TokenisationSourceSystemCode { get; }
    }

    public class PaymentConfigurationProvider : IPaymentConfigurationProvider
    {
        private readonly Lazy<NameValueCollection> _tokenLazy =
            new Lazy<NameValueCollection>(
                () => (NameValueCollection)ConfigurationManager.GetSection("talCreditCardTokenisationServiceConfiguration"));

        private NameValueCollection TokenizationSettings
        {
            get { return _tokenLazy.Value; }
        }

        public string ConnectionString
        {
            get { return ConfigurationManager.AppSettings["Payment.SqlConnectionString"]; }
        }

        public string TokenisationBaseUrl
        {
            get { return TokenizationSettings["ServiceBaseUrl"]; }
        }

        public string TokenisationIdentityUser
        {
            get { return TokenizationSettings["IdentityUser"]; }
        }

        public string TokenisationThumbPrint
        {
            get { return TokenizationSettings["Thumbprint"]; }
        }

        public string TokenisationSourceSystemCode
        {
            get { return TokenizationSettings["SourceSystemCode"]; }
        }
    }
}
