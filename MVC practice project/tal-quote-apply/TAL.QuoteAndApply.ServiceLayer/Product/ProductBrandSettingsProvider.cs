using TAL.QuoteAndApply.Configuration.Service;
using TAL.QuoteAndApply.Product.Contracts;

namespace TAL.QuoteAndApply.ServiceLayer.Product
{
    public class ProductBrandSettingsProvider : IProductBrandSettingsProvider
    {
        private readonly IConfigurationService _configurationService;
        private readonly IProductBrandProvider _productBrandProvider;

        public ProductBrandSettingsProvider(IConfigurationService configurationService, IProductBrandProvider productBrandProvider)
        {
            _configurationService = configurationService;
            _productBrandProvider = productBrandProvider;
        }

        public string GetSetting(string brand, string key, string defaultValue)
        {
            var brandId = _productBrandProvider.GetBrandIdByKey(brand);
            return _configurationService.GetConfigurationValue(brandId, key, defaultValue);
        }

        public int GetSetting(string brand, string key, int defaultValue)
        {
            var brandId = _productBrandProvider.GetBrandIdByKey(brand);
            return _configurationService.GetConfigurationValue(brandId, key, defaultValue);
        }

        public bool GetSetting(string brand, string key, bool defaultValue)
        {
            var brandId = _productBrandProvider.GetBrandIdByKey(brand);
            return _configurationService.GetConfigurationValue(brandId, key, defaultValue);
        }
    }
}
