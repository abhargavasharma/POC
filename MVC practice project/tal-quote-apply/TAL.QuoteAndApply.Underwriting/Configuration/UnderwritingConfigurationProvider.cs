
using System.Configuration;

namespace TAL.QuoteAndApply.Underwriting.Configuration
{
    public interface IUnderwritingConfigurationProvider
    {
        string UnderwritingApiBaseUrl { get; }
        string TalusUiBaseUrl { get; }

        string TemplateName { get; }
        string FullWorkflow { get; }
    }

    public class UnderwritingConfigurationProvider : IUnderwritingConfigurationProvider
    {
        public string UnderwritingApiBaseUrl
        {
            get { return ConfigurationManager.AppSettings["Underwriting.BaseUrl"]; }
        }

        public string TalusUiBaseUrl
        {
            get { return ConfigurationManager.AppSettings["Underwriting.TalusUiUrl"]; }
        }

        public string TemplateName
        {
            get { return ConfigurationManager.AppSettings["Underwriting.TemplateName"]; }
        }

        public string FullWorkflow
        {
            get { return ConfigurationManager.AppSettings["Underwriting.FullWorkflow"]; }
        }
    }
}
