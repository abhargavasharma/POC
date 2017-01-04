using System.Configuration;

namespace TAL.QuoteAndApply.Payment.Configuration
{
    public interface ISuperannuationConfigurationProvider
    {
        string SuperannuationServiceBaseUrl { get; }
    }

    public class SuperannuationConfigurationProvider : ISuperannuationConfigurationProvider
    {
        public string SuperannuationServiceBaseUrl
        {
            get { return ConfigurationManager.AppSettings["Superannuation.ServiceBaseUrl"]; }
        }
    }
}
