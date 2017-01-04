using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Product.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IRiskRatingFactorsProvider
    {
        RatingFactorsResult GetFor(string quoteReferenceNumber, int riskId);
    }

    public class RiskRatingFactorsProvider : IRiskRatingFactorsProvider
    {
        private readonly IRatingFactorsResultConverter _riskRatingFactorsResultConverter;
        private readonly IProductDefinitionBuilder _productDefinitionBuilder;
        private readonly IRiskProductDefinitionConverter _riskProductDefinitionConverter;
        private readonly IPolicyService _policyService;
        private readonly IRiskService _riskService;

        public RiskRatingFactorsProvider(IPolicyService policyService, 
            IRiskService riskService, 
            IRatingFactorsResultConverter riskRatingFactorsResultConverter,
            IProductDefinitionBuilder productDefinitionBuilder,
            IRiskProductDefinitionConverter riskProductDefinitionConverter)
        {
            _policyService = policyService;
            _riskService = riskService;
            _riskRatingFactorsResultConverter = riskRatingFactorsResultConverter;
            _productDefinitionBuilder = productDefinitionBuilder;
            _riskProductDefinitionConverter = riskProductDefinitionConverter;
        }

        public RatingFactorsResult GetFor(string quoteReferenceNumber, int riskId)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReferenceNumber);
            var risk = _policyService.GetRiskForPolicy(policy, riskId);

            var model = _riskRatingFactorsResultConverter.CreateFrom(risk);

            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(policy.BrandKey);
            var riskProductDefinition = _riskProductDefinitionConverter.CreateFrom(productDefinition);
            model.IsRatingFactorsValidForInforce = _riskService.IsRiskValidForInforce(riskProductDefinition, risk);

            return model;
        }
    }
}