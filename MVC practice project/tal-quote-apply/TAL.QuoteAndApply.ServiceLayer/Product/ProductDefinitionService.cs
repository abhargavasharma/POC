using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models.Definition;

namespace TAL.QuoteAndApply.ServiceLayer.Product
{
    public interface IProductDefinitionService
    {
        ProductDefinition GetProductDefinition();
    }

    public class ProductDefinitionService : IProductDefinitionService
    {
        private readonly IProductDefinitionBuilder _productDefinitionBuilder;
        private readonly ICurrentProductBrandProvider _currentProductBrandProvider;

        public ProductDefinitionService(IProductDefinitionBuilder productDefinitionBuilder, ICurrentProductBrandProvider currentProductBrandProvider)
        {
            _productDefinitionBuilder = productDefinitionBuilder;
            _currentProductBrandProvider = currentProductBrandProvider;
        }

        public ProductDefinition GetProductDefinition()
        {
            var currentBrand = _currentProductBrandProvider.GetCurrent();
            return _productDefinitionBuilder.BuildProductDefinition(currentBrand.BrandCode);
        }
    }
}
