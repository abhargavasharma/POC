using TAL.QuoteAndApply.Product.Contracts;

namespace TAL.QuoteAndApply.Tests.Shared.Mocks
{
    public class MockProductBrandSettingsProvider : IProductBrandSettingsProvider
    {
        public string GetSetting(string brand, string key, string defaultValue)
        {
            return defaultValue;
        }

        public int GetSetting(string brand, string key, int defaultValue)
        {
            return defaultValue;
        }

        public bool GetSetting(string brand, string key, bool defaultValue)
        {
            return defaultValue;
        }
    }
}