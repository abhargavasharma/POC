namespace TAL.QuoteAndApply.Product.Models.Definition
{
    public interface IAvailability
    {
        FeatureRule RuleDefinition { get; }
        string Code { get; }

        string Name { get; }
    }
}