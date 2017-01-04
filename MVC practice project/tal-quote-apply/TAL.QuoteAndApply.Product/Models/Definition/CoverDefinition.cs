using TAL.QuoteAndApply.DataModel.Underwriting;

namespace TAL.QuoteAndApply.Product.Models.Definition
{
    public class CoverDefinition : IAvailability
    {
        public CoverDefinition(string code, string name, bool isRateableCover, FeatureRule rule)
        {
            Name = name;
            IsRateableCover = isRateableCover;
            Code = code;
            RuleDefinition = rule;
            UnderwritingBenefitCode = code;
            SupportedLoadingTypes = new[] { LoadingType.Variable, LoadingType.PerMille };
        }

        public CoverDefinition(string code, string name, bool isRateableCover)
            : this(code, name, isRateableCover, null)
        {
        }
        
        public string Code { get; }
        public string CoverFor { get; private set; }
        public string Name { get; }
        public bool IsRateableCover { get; }
        public FeatureRule RuleDefinition { get; }
        public string UnderwritingBenefitCode { get; private set; }
        /// <summary>
        /// Defaulted to Variable and PerMille, can be overriden
        /// </summary>
        public LoadingType[] SupportedLoadingTypes { get; private set; }

        public CoverDefinition WithUnderwritingCode(string underwritingCode)
        {
            UnderwritingBenefitCode = underwritingCode;
            return this;
        }

        public CoverDefinition AsRiderCodeFor(string coverCode)
        {
            CoverFor = coverCode;
            return this;
        }

        public CoverDefinition WithSupportedLoadingTypes(LoadingType[] supportedLoadingTypes)
        {
            SupportedLoadingTypes = supportedLoadingTypes;
            return this;
        }
    }
}

