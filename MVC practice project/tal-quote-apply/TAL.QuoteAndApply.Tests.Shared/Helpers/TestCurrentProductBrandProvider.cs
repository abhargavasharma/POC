using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Models;

namespace TAL.QuoteAndApply.Tests.Shared.Helpers
{
    public class TestCurrentProductBrandProvider : ICurrentProductBrandProvider
    {
        private ProductBrand _current;

        public TestCurrentProductBrandProvider()
        {
            _current =  new ProductBrand(1, "TAL", 1);
        }

        public ProductBrand GetCurrent()
        {
            return _current;
        }

        public void Set(ProductBrand newValue)
        {
            _current = newValue;
        }
    }
}