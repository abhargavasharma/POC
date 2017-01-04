using TAL.QuoteAndApply.Configuration.Data;

namespace TAL.QuoteAndApply.Configuration.Service
{
    public interface IConfigurationService
    {
        bool GetConfigurationValue(int brandId, string configKey, bool defaultValue);
        int GetConfigurationValue(int brandId, string configKey, int defaultValue);
        string GetConfigurationValue(int brandId, string configKey, string defaultValue);
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationItemRepository _repository;

        public ConfigurationService(IConfigurationItemRepository repository)
        {
            _repository = repository;
        }

        public bool GetConfigurationValue(int brandId, string configKey, bool defaultValue)
        {
            var value = GetConfigurationValue(brandId, configKey, null);
            bool outResult;
            return value == null || !bool.TryParse(value, out outResult) ? defaultValue : outResult;
        }

        public int GetConfigurationValue(int brandId, string configKey, int defaultValue)
        {
            var value = GetConfigurationValue(brandId, configKey, null);
            int outResult;
            return value == null || !int.TryParse(value, out outResult) ? defaultValue : outResult;
        }

        public string GetConfigurationValue(int brandId, string configKey, string defaultValue)
        {
            return _repository.GetConfigurationItem(brandId, configKey)?.Value ?? defaultValue;
        }
    }
}
