using System.Configuration;

namespace TAL.QuoteAndApply.Infrastructure.Configuration
{
    public interface IPasEncryptionConfigurationProvider
    {
        string PasEncryptionServiceBaseUrl { get; }
        string PasEncryptionServiceUser { get; }
        string PasEncryptionServicePassword { get; }
        string PasEncryptionServiceDomain { get; }
    }

    public class PasEncryptionConfigurationProvider : IPasEncryptionConfigurationProvider
    {
        public string PasEncryptionServiceBaseUrl
        {
            get { return ConfigurationManager.AppSettings["Payment.PasEncryptionServiceBaseUrl"]; }
        }

        public string PasEncryptionServiceUser
        {
            get { return ConfigurationManager.AppSettings["Payment.PasEncryptionServiceUser"]; }
        }

        public string PasEncryptionServicePassword
        {
            get { return ConfigurationManager.AppSettings["Payment.PasEncryptionServicePassword"]; }
        }

        public string PasEncryptionServiceDomain
        {
            get { return ConfigurationManager.AppSettings["Payment.PasEncryptionServiceDomain"]; }
        }
    }
}
