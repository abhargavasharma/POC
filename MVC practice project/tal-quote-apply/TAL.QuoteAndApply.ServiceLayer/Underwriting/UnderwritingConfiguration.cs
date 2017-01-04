using TAL.QuoteAndApply.Underwriting.Configuration;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting
{
    public interface IUnderwritingConfiguration
    {
        string TalusUiBaseUrl { get; }
    }

    public class UnderwritingConfiguration : IUnderwritingConfiguration
    {
        private readonly IUnderwritingConfigurationProvider _underwritingConfigurationProvider;

        public UnderwritingConfiguration(IUnderwritingConfigurationProvider underwritingConfigurationProvider)
        {
            _underwritingConfigurationProvider = underwritingConfigurationProvider;
        }

        public string TalusUiBaseUrl => _underwritingConfigurationProvider.TalusUiBaseUrl;
    }
}
