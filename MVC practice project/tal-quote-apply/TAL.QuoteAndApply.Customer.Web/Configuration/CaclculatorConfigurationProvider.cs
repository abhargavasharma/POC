using System.Configuration;

namespace TAL.QuoteAndApply.Customer.Web.Configuration
{
    public interface ICaclculatorConfigurationProvider
    {
        string CoverCalculatorScriptsUrl { get; }
        string CoverCalculatorBaseUrl { get; }
        bool IsEnabled { get; }
    }

    public class CaclculatorConfigurationProvider : ICaclculatorConfigurationProvider
    {
        public bool IsEnabled => bool.Parse(ConfigurationManager.AppSettings["CoverCalculator.IsCalculatorEnabled"]);

        public string CoverCalculatorScriptsUrl => ConfigurationManager.AppSettings["CoverCalculator.ScriptsUrl"];

        public string CoverCalculatorBaseUrl => ConfigurationManager.AppSettings["CoverCalculator.BaseUrl"];

    }
}
