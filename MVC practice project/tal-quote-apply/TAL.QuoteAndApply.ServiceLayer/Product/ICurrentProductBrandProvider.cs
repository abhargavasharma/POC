using TAL.QuoteAndApply.ServiceLayer.Product.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Product
{
    public interface ICurrentProductBrandProvider
    {
        ProductBrand GetCurrent();
    }
}
