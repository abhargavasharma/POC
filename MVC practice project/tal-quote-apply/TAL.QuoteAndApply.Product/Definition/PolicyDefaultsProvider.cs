namespace TAL.QuoteAndApply.Product.Definition
{
    public interface IPolicyDefaultsProvider
    {
        PolicyDefaults GetPolicyDefaults(int brandId, string brandKey, int organisationId);
    }

    public class PolicyDefaultsProvider : IPolicyDefaultsProvider
    {
        private readonly IProductDefinitionBuilder _productDefinitionBuilder;

        public PolicyDefaultsProvider(IProductDefinitionBuilder productDefinitionBuilder)
        {
            _productDefinitionBuilder = productDefinitionBuilder;
        }

        public PolicyDefaults GetPolicyDefaults(int brandId, string brandKey, int organisationId)
        {
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(brandKey);
            return new PolicyDefaults(productDefinition.DefaultPremiumFrequency, brandId, brandKey, organisationId);
        }
    }
}
