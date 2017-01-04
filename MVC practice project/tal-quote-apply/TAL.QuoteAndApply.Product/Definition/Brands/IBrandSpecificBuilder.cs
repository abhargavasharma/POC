using TAL.QuoteAndApply.Product.Models.Definition;

namespace TAL.QuoteAndApply.Product.Definition.Brands
{
    internal interface IBrandSpecificBuilder
    {
        ProductDefinition ApplyBrandSpecifics(ProductDefinition baseProductDefinition);
    }
}
